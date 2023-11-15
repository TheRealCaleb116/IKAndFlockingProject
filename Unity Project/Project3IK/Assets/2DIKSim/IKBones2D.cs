using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBone2D 
{
    public Vector2 root;
    public float length;
    public float angle;

    public Vector2 endEff;

    public IKBone2D parent;

    //Defaults to true, is set to false if the bone gets children through a set parent
    public bool isEnd = true;

    public Vector2 direction;

    public List<IKBone2D> children;

    //Maximum step in degrees
    public float maxStep = 1.0f * Mathf.Deg2Rad;

    //The cumulative angle of the proceeding joint
    public float prevAngle = 0.0f;

    public float bonewidth = 0.025f;


    //Angles
    public bool doAngleLimit = false;
    public float minAngle = 0.0f;
    public float maxAngle = 360.0f;

    public IKBone2D(Vector2 root, float length, float angle)
    {
        //Set initial variables
        this.root = root;
        this.length = length;
        this.angle = angle;

        //Set the initial direction vector 
        direction = Vector2.up;

        //Init children
        children = new List<IKBone2D>();

    }

    public void setParent(IKBone2D parent)
    {
        //Set initial postion bosed on the parent and the parents starting direction
        this.parent = parent;

        root = parent.root + (parent.direction * parent.length);

        //Add to children
        parent.children.Add(this);
        parent.isEnd = false;

    }

    public void setAngleLimt(float min, float max)
    {
        this.minAngle = min;
        this.maxAngle = max;

        this.doAngleLimit= true;
    }

}


public class IKArmiture2D
{
    public Vector2 root;
    public List<IKBone2D> bones;
    public IKBone2D baseBone;
    public IKBone2D endBone;

    public float fudge = 0.01f;

    public Vector2 goalPoint;

    public IKArmiture2D(Vector2 root, List<IKBone2D> bones)
    {
        //Set bones array
        this.bones = bones;

        //Set first and end bone
        baseBone = bones[0];
        endBone = bones[bones.Count - 1];

    }

    public bool solveArmiture()
    {
        //Iterate from the end to the base
        doIK(endBone);
        return true;
    }

    public void doIK(IKBone2D bone)
    {
        Vector2 startToGoal = goalPoint - bone.root;
        Vector2 startToEndEffector = endBone.endEff - bone.root;

        float dotProd = Vector2.Dot(startToGoal.normalized, startToEndEffector.normalized);

        dotProd = Mathf.Clamp(dotProd, -1.0f, 1.0f);
        float step = 0.0f;

        if (Vec2Cross(startToGoal, startToEndEffector) < 0)
        {
            step = Mathf.Acos(dotProd);
        }
        else
        {
            step = -Mathf.Acos(dotProd);

        }

        if (bone.doAngleLimit == true && IKGM.GetAngleConstraintState() == true)
        {
            //Debug.Log("Angle: " + bone.angle);
            float clampedEnd = Mathf.Clamp(step + bone.angle, (bone.minAngle - 90.0f) * Mathf.Deg2Rad, (bone.maxAngle - 90.0f) * Mathf.Deg2Rad);
            float delta = (step + bone.angle) - clampedEnd;
            step -= delta;
        }

        //Constrain movement to maxStep
        step = Mathf.Clamp(step, -bone.maxStep, bone.maxStep);


        //Add colision detection just line for now
        //Calc potential next pos
        Vector2 dir = new Vector2(Mathf.Cos(bone.prevAngle + bone.angle + step), Mathf.Sin(bone.prevAngle + bone.angle + step));

        //Calc potental next root
        Vector2 nextRoot = bone.root + (dir * bone.length);


        //Test for collision 
       // LinePrimitive l = new LinePrimitive(bone.root, nextRoot);
        bool doMove = true;
        foreach (CircleObst c in IKGM.circleObstacales)
        {
            //Run a collision detection on each
            if (IsCollding(bone,nextRoot) == true) //If this is true then there is a collision
            {
                //The movement causes a collision
                doMove = false;
            }
        }


        if (doMove)
        {
            bone.angle += step;
        }

        bool result = doFK(baseBone, root, 0.0f);
        if (result == true)
        {
            bone.angle -= step;
            doFK(baseBone, root, 0.0f);
        }

        if (bone != baseBone)
        {
            doIK(bone.parent);
        }

    }

    public bool doFK(IKBone2D bone, Vector3 root, float prevAngle)
    {

        //Update root
        bone.root = root;
        bone.prevAngle = prevAngle; //for collision


        Vector2 dir = new Vector2(Mathf.Cos(prevAngle + bone.angle), Mathf.Sin(prevAngle + bone.angle));

        //Calculate next root
        Vector2 nextRoot = bone.root + (dir * bone.length);


        //Check collision and cancel the entire motion if it is colliding
        if (IsCollding(bone, nextRoot))
        {
            return true;
        }


        //Update direction and End Effector
        bone.direction = (nextRoot - bone.root).normalized;
        bone.endEff = nextRoot;


        //Iterate through each child and propogqate rotation through children
        foreach (IKBone2D child in bone.children)
        {
            bool val = doFK(child, nextRoot, prevAngle + bone.angle);
            if (val == true)
            {
                return true;
            }
        }

        return false;
    }

    //Does a simple check to see if the bone is colding with any spheres
    public bool IsCollding(IKBone2D bone, Vector2 newNextRoot)
    {

        //Make a line at where the center line will be after the angle change
        LinePrimitive l = new LinePrimitive(bone.root, newNextRoot);

        foreach (CircleObst c in IKGM.circleObstacales)
        {
            //Get the closest point on the line
            Vector2 p1 = l.end;
            Vector2 p2 = l.start;
            Vector2 p3 = c.col.center;

            Vector2 delta = p2 - p1;
            float u = ((p3.x - p1.x) * delta.x + (p3.y - p1.y) * delta.y) / (delta.x * delta.x + delta.y * delta.y);

            Vector2 closestPoint;
            if (u < 0.0f)
            {
                closestPoint = p1;
            }
            else if (u > 1.0f)
            {
                closestPoint = p2;
            }
            else
            {
                closestPoint = new Vector2(p1.x + u * delta.x, p1.y + u * delta.y);
            }

            //Find the overlap distance and hit vector
            Vector2 hitVector = c.col.center - closestPoint;
            float overlapDist = (hitVector.magnitude - (c.col.radius + fudge));

            if (overlapDist < 0.0f)
            {
                return true;
            }

        }
        return false;
    }

    //Returns a singed value useful for direciton testing
    public float Vec2Cross(Vector3 a, Vector3 b)
    {
        return a.x * b.y - a.y * b.x;
    }


    //Takes in a bone, tests for collisions, returns the new target angle
    //While techncaly a more 'correct' option, this was proving more unstable, The calculations might be flawed
    /*
    public float HandelBoneCollision(IKBone2D bone, float newAngle, Vector2 newNextRoot)
    {
        //Create a line for the bone in its new spot
        //Calc potential next pos

        //Make a line at where the center line will be after the angle change
        LinePrimitive l = new LinePrimitive(bone.root, newNextRoot);

        foreach (CircleObst c in IKGM.circleObstacales)
        {
            //Run a collision detection on each
            if (IntersectionUtility.CircleIntersectsLine(c.col, l)) //If this is true then there is a collision
            {
                //Get the closest point on the line
                Vector2 p1 = l.end;
                Vector2 p2 = l.start;
                Vector2 p3 = c.col.center;

                Vector2 delta = p2 - p1;
                float u = ((p3.x - p1.x) * delta.x + (p3.y - p1.y) * delta.y) / (delta.x * delta.x + delta.y * delta.y);

                Vector2 closestPoint;
                if (u < 0.0f)
                {
                    closestPoint = p1;
                }
                else if (u > 1.0f)
                {
                    closestPoint = p2;
                }
                else
                {
                    closestPoint = new Vector2(p1.x + u * delta.x, p1.y + u * delta.y);
                }

                //Find the overlap distance and hit vector
                Vector2 hitVector = c.col.center - closestPoint;
                float overlapDist = (hitVector.magnitude - (c.col.radius));


                Debug.DrawRay(closestPoint, hitVector, Color.yellow);
                Debug.DrawRay(bone.endEff, hitVector * overlapDist, Color.blue);

                //Get the point on the edge of the circle where our line should rest
                Vector2 corrPoint = closestPoint + (hitVector * overlapDist);

                //Get the line between that point and the bones root
                Vector2 desiredDir = (corrPoint - bone.root);

                Debug.DrawRay(bone.root, desiredDir * 1.0f, Color.magenta);

                //Calcualte the angle between our current position and the new target position and return that new angle in radians
                return (Vector2.SignedAngle(bone.direction.normalized, desiredDir.normalized)) * Mathf.Deg2Rad;

            }
        }

        //Otherwise
        return newAngle;
    }
    */

}

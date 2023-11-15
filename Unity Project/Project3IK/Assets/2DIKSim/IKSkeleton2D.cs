using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSkeleton2D : MonoBehaviour
{
    public List<IKArmiture2D> mIKArmitures = new List<IKArmiture2D>();

    public GameObject goalObj1;
    public GameObject goalObj2;

    public List<Transform> children;

    public List<IKBone2D> allBones;

    public float rootLen;
    public float c1Length = 0.3f;
    public float defaultArmLen;
    public float arm1Len;
    public float armEndLen;

    public float rootAngle = 0.0f;
    public float c1Angle = 0.0f;
    public float defaultArmAngle = 0.0f;
    public float armEndAngle = 0.0f;


    void Awake()
    {
        //Build a multi arm skeleton with two armitures

        //Skell root Node
        IKBone2D root = new IKBone2D(transform.position, rootLen, rootAngle);
        //root.setAngleLimt(0.0f, 90.0f);
        root.setAngleLimt(135.0f, 225.0f);

        IKBone2D c1 = new IKBone2D(transform.position, c1Length, c1Angle);
        c1.setParent(root);
        c1.setAngleLimt(20.0f - 90.0f, 340.0f - 90.0f);


        IKBone2D c2 = new IKBone2D(transform.position, 0.4f, defaultArmAngle);
        c2.setParent(c1);
        c2.setAngleLimt(20.0f - 90.0f, 340.0f - 90.0f);


        //Right arm
        IKBone2D r1 = new IKBone2D(transform.position, arm1Len, defaultArmAngle);
        r1.setParent(c2);
        r1.setAngleLimt(20.0f - 90.0f, 170.0f - 90.0f);



        IKBone2D r2 = new IKBone2D(transform.position, defaultArmLen, defaultArmAngle);
        r2.setParent(r1);

        IKBone2D r3 = new IKBone2D(transform.position, defaultArmLen - 0.1f, defaultArmAngle);
        r3.setParent(r2);
        //r3.setAxis(Vector3.down);

        IKBone2D r4 = new IKBone2D(transform.position, armEndLen , armEndAngle);
        r4.setParent(r3);
        r4.setAngleLimt(20.0f - 90.0f, 340.0f - 90.0f);


        //Left arm
        IKBone2D l1 = new IKBone2D(transform.position, arm1Len, defaultArmAngle);
        l1.setParent(c2);
        l1.setAngleLimt(20.0f + 90.0f, 170.0f + 90.0f);


        IKBone2D l2 = new IKBone2D(transform.position, defaultArmLen, defaultArmAngle);
        l2.setParent(l1);

        IKBone2D l3 = new IKBone2D(transform.position, defaultArmLen - 0.1f, defaultArmAngle);
        l3.setParent(l2);
        //l3.setAxis(Vector3.down);

        IKBone2D l4 = new IKBone2D(transform.position, armEndLen, armEndAngle);
        l4.setParent(l3);
        l4.setAngleLimt(20.0f - 90.0f, 340.0f - 90.0f);


        //Build bone lists
        List<IKBone2D> list1 = new List<IKBone2D>() { root, c1, c2, l1, l2, l3, l4};
        List<IKBone2D> list2 = new List<IKBone2D>() { root, c1, c2, r1, r2, r3, r4};
        allBones = new List<IKBone2D>() { root, c1, c2, l1, l2, l3, l4, r1, r2, r3, r4 };


        //Build the armitures
        IKArmiture2D arm1 = new IKArmiture2D(transform.position, list1);
        IKArmiture2D arm2 = new IKArmiture2D(transform.position, list2);


        mIKArmitures.Add(arm1);
        mIKArmitures.Add(arm2);

        //Get a list of all children

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            children.Add(transform.GetChild(0).GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {

        //Update target position
        mIKArmitures[0].goalPoint = goalObj1.transform.position;
        mIKArmitures[1].goalPoint = goalObj2.transform.position;


        foreach (IKArmiture2D arm in mIKArmitures)
        {
            arm.solveArmiture();
        }

        //Draw armiture
        for (int i = 0; i < children.Count; i++)
        {
            //Check if we are out of bounds
            if (i > allBones.Count - 1) { break; }
            //Debug.Log(allBones[i].root);

            children[i].position = allBones[i].root + (allBones[i].direction * (allBones[i].length / 2.0f));

            children[i].localScale = new Vector2(allBones[i].bonewidth, allBones[i].length);

            children[i].transform.up = allBones[i].endEff - (Vector2) children[i].transform.position;
        }

    }


    public void OnDrawGizmos()
    {

        if (Application.isPlaying)
        {
            foreach (IKArmiture2D arm in mIKArmitures)
            {
                foreach (IKBone2D bone in arm.bones)
                {
                    if (bone.isEnd)
                    {
                        Gizmos.color = Color.yellow;
                        //Draw last bone
                        Gizmos.DrawLine(bone.root, arm.endBone.endEff);
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(bone.root, new Vector3(0.01f, 0.01f, 0.01f));

                    }
                    else
                    {
                        Gizmos.color = Color.red;

                        //Draw link to children 
                        Gizmos.DrawLine(bone.root, bone.children[0].root);
                        Gizmos.color = Color.black;
                        Gizmos.DrawSphere(bone.root, 0.01f);
                    }
                }

            }
        }



    }

}

using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;

    public float mass;

    public float rotationSpeed;
    public float maxSpeed;
    public float minSpeed;
    public float closingDistance;
    public float stopDistance;
    public float dragCof = 0.2f;

    public bool stopped = false;

    //Tuning Params
    public float sepForceMaxD = 4.0f;
    public float seperationScale = 2.0f;

    public float attrForceMaxD = 4.0f;
    public float attrScale = 2.0f;

    public float alignForceMaxD = 4.0f;
    public float alignScale = 2.0f;

    public float collisionForceMaxD = 4.0f;
    public float collisionScale = 20.0f;

    public float goalForceScale = 20.0f;


    public float minRandStartVel = 0.0f;
    public float maxRandStartVel = 10.0f;

    public float boundsForceScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Register self
        GameManager.registerAgent(this);

        //Set initial velocity
        if (GameManager.SeekTowardsGoal == false)
        {
            Vector2 randomVel2d = new Vector2(Random.Range(minRandStartVel, maxRandStartVel), Random.Range(minRandStartVel, maxRandStartVel));
            velocity = new Vector3(randomVel2d.x, 0.0f, randomVel2d.y);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Update position based on velocity
        transform.position += velocity * Time.deltaTime;

        //Rotate Towards velocity
        if (!stopped)
        {
            //Rotation
            Vector3 dir = velocity.normalized;


            //Face towards velocity
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0.0f, 90.0f, 0.0f) * Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
        }
        else
        {
            //velocity = velocity * 0.9f;

        }


        //Calculate forces (Note these are remapped so int he 3d scene its x, z)
        Vector3 SeperationForce = Vector3.zero;
        Vector3 AlignmentForce = Vector3.zero;
        Vector3 CohesionForce = Vector3.zero;
        Vector3 ObstacleForce = Vector3.zero;

        //precalc some values
        Vector2 agentPos = new Vector2(transform.position.x, transform.position.z);


        //Calculate Basic BOID forces
        Vector2 avgPosition = Vector2.zero;
        int avgPosCount = 0;

        Vector3 avgVelocity = Vector3.zero;
        int avgVelCount = 0;

        for (int i = 0; i < GameManager.agents.Count; i++)
        {
            //Skip Self
            if (GameManager.agents[i] == this) { continue; }

            //Calculate the 2d agent pos
            Vector2 otherAgentPos = new Vector2(GameManager.agents[i].transform.position.x, GameManager.agents[i].transform.position.z);
            float distance = Vector2.Distance(otherAgentPos, agentPos);


            //Seperation
            if (distance > 0.0001f && distance < sepForceMaxD)
            {
                //Calculate and add Seperation Force
                Vector2 sForce = agentPos - otherAgentPos;
                sForce = sForce.normalized * (seperationScale / Mathf.Pow(distance, 2));
                SeperationForce += new Vector3(sForce.x, 0.0f, sForce.y);
            }


            //Cohesion
            if (distance > 0.0001f && distance < attrForceMaxD)
            {
                //Add to avg to calculate after the loop
                avgPosition += otherAgentPos;
                avgPosCount++;
            }

            //Alignment
            if (distance > 0.0001f && distance < alignForceMaxD)
            {
                //Add to avg to calculate after the loop
                avgVelocity += GameManager.agents[i].velocity;
                avgVelCount++;
            }
        }



        //Calculate the final Cohesion Force
        avgPosition = avgPosition / avgPosCount;
        Vector2 attrForce = avgPosition - agentPos;
        attrForce = attrForce.normalized * attrScale;
        CohesionForce = new Vector3(attrForce.x, 0.0f, attrForce.y);

        //Calculate the final Alignment Force
        avgVelocity = avgVelocity / avgVelCount;
        Vector3 towards = avgVelocity - velocity;
        towards = towards.normalized * alignScale;
        AlignmentForce = new Vector3(towards.x, 0.0f, towards.y);


        if (GameManager.AvoidObstacles == true)
        {
            //Obstacle Force
            for (int i = 0; i < GameManager.obstacles.Count; i++)
            {
                Transform obst = GameManager.obstacles[i].transform;
                //Only do this in 2d
                Vector2 obstPos = new Vector2(obst.position.x, obst.position.z);

                float d = Vector2.Distance(obstPos, agentPos);

                if (d > collisionForceMaxD + GameManager.obstacles[i].radius) continue;

                //Calculate collision force
                Vector2 dir = agentPos - obstPos;
                //Debug.Log(Vector2.Dot(new Vector2(velocity.x, velocity.z).normalized, dir.normalized));

                //Only apply if obst is in front?
                if (Vector2.Dot(new Vector2(velocity.x, velocity.z).normalized, dir.normalized) > 0.2f)
                {
                    continue;
                }

                Debug.DrawRay(obst.position, new Vector3(dir.x, 0.0f, dir.y).normalized * 10.0f, Color.yellow);

                Vector2 perpVec = new Vector2(dir.y, -dir.x);

                Vector2 perpPoint = obstPos + perpVec.normalized * GameManager.obstacles[i].radius;
                Vector2 vPoint = agentPos + new Vector2(velocity.x, velocity.z);

                //Decide Sidedness
                if (isOnSameSide(agentPos, obstPos, perpPoint, vPoint) == false)
                {
                    perpVec = -perpVec;
                }

                Debug.DrawRay(obst.position, new Vector3(perpVec.x, 0.0f, perpVec.y).normalized * 10.0f, Color.red);

                Vector2 oForce = perpVec.normalized * (collisionScale / Mathf.Pow(d, 2));
                Debug.DrawRay(transform.position, new Vector3(oForce.x, 0.0f, oForce.y), Color.magenta);
                ObstacleForce += new Vector3(oForce.x, 0.0f, oForce.y);

            }
        }

        //Calculate total accleration
        Vector3 acceleration = ((SeperationForce + AlignmentForce + CohesionForce  + ObstacleForce) / mass);

        //Calculate goal attraction forces
        //Goal Force -- A force that directs the boid towards a goal point
        if (GameManager.SeekTowardsGoal == true)
        {
            Vector3 GoalForce = Vector3.zero;
            Vector3 gPos = GameManager.GetCurrentGoal();
            Vector2 g = new Vector2(gPos.x, gPos.z);

            //Unstop agent if conditions are met 
            if (Vector2.Distance(g, new Vector2(avgPosition.x, avgPosition.y)) > closingDistance || Vector2.Distance(g, agentPos) > closingDistance)
            {
                stopped = false;
            }


            if (Vector2.Distance(g, agentPos) > closingDistance)
            {
                //Scale dist 
                float dist = Vector2.Distance(g, agentPos);

                Vector2 gForce = g - agentPos;
                gForce = gForce.normalized * goalForceScale + gForce.normalized * (goalForceScale / Mathf.Pow(dist, 2));

                //Apply force
                GoalForce += new Vector3(gForce.x, 0.0f, gForce.y);


            }
            else
            {
                velocity = velocity.normalized * (velocity.magnitude * 0.99f);
                stopped = true;
            }

            //Add to acceleration
            acceleration += (GoalForce / mass);
        }

        //Test bounds and reflect velocity if we we cross them
        Vector4 bounds = GameManager.getBounds();//Top==x, Bottom==y, Left==z, Right==w 

        //Z direction
        if (agentPos.y > bounds.x)
        {
            //Debug.Log("Top");
            float distPast = (agentPos.y - bounds.x);
            acceleration += new Vector3(0.0f, 0.0f, -1.0f) * boundsForceScale * distPast;
        }
        else if (agentPos.y < bounds.y)
        {
            //Debug.Log("Bottom");
            float distPast = (bounds.y - agentPos.y);
            acceleration += new Vector3(0.0f, 0.0f, 1.0f) * boundsForceScale * distPast;
        }

        //X Direction
        if (agentPos.x < bounds.z)
        {
            //Debug.Log("Left");

            float distPast = (bounds.z - agentPos.x);
            acceleration += new Vector3(1.0f, 0.0f, 0.0f) * boundsForceScale * distPast;

        }
        else if (agentPos.x > bounds.w)
        {
           // Debug.Log("Right");

            float distPast = (agentPos.x - bounds.w);
            acceleration += new Vector3(-1.0f, 0.0f, 0.0f) * boundsForceScale * distPast;

        }

        //Calculate velcoity
        velocity += acceleration * Time.deltaTime;

        //Limit y movement
        velocity.y = 0.0f;

        //cap speed
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }else if (GameManager.SeekTowardsGoal == false && velocity.magnitude < minSpeed)
        {
            velocity = velocity.normalized * minSpeed;
        }

        //If we are seeking towards a goal we need friction/dampening
        if (GameManager.SeekTowardsGoal == true)
        {
            //Friction/Dampening
            velocity -= velocity.normalized * (velocity.magnitude * dragCof) * Time.deltaTime;

            //Stop if velcoity drops below threshold
            if (stopped == true && velocity.magnitude < stopDistance)
            {
                velocity = Vector3.zero;
            }

        }

        //Draw debug vector
        Debug.DrawRay(transform.position, velocity);


    }


    public static bool isOnSameSide(Vector2 lineStart, Vector2 lineEnd, Vector2 p1, Vector2 p2)
    {
        float cp1 = CollisionLib.Cross(lineEnd - lineStart, p1 - lineStart);
        float cp2 = CollisionLib.Cross(lineEnd - lineStart, p2 - lineStart);

        return cp1 * cp2 >= 0;
    }





}

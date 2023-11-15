using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton pattern 
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public static List<Agent> agents = new List<Agent>();
    public static List<Obstacle> obstacles = new List<Obstacle>();

    public static bool SeekTowardsGoal = false;
    public static bool AvoidObstacles = false;

    public static Vector3 GetCurrentGoal()
    {
        return _instance.goal.transform.position;
    }


    public static void registerAgent(Agent a)
    {
        agents.Add(a);
    }


    public static void registerObstacle(Obstacle o)
    {
        obstacles.Add(o);
    }

    public static Vector4 getBounds()
    {
        //Top, Bottom, Left, Right
        return new Vector4(
            _instance.Top.transform.position.z,
            _instance.Bottom.transform.position.z,
            _instance.Left.transform.position.x,
            _instance.Right.transform.position.x);

    }


    public GameObject goal;
    public GameObject boidPrefab;
    public GameObject agentsContainer;


    //Bounds Objects
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Left;
    public GameObject Right;


    //Gameobject Instance Methods
    public void Awake()
    {
        //Singleton pattern checks
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }


    }

    public void Start()
    {
        ToggleObstacles(false);
    }

    public void SwitchToIK()
    {
        SceneManager.LoadScene(0);
    }

    public void ToggleObstacles(bool b)
    {
        GameManager.AvoidObstacles = b;


        //Toggle on and off the obstacle objects
        if (AvoidObstacles == true)
        {
            //Turn them on
            foreach (Obstacle o in obstacles)
            {
                o.gameObject.SetActive(true);
            }
        }
        else
        {
            //turn them off
            foreach(Obstacle o in obstacles)
            {
                o.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleSeekTowardsGoal(bool b)
    {
        GameManager.SeekTowardsGoal = b;
    }


    private void Update()
    {
        //Right click for new boid
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);
                GameObject b = Instantiate(boidPrefab, agentsContainer.transform, false);
                b.transform.localPosition = new Vector3(hit.point.x, 0.0f, hit.point.z);

            }

        }

        //Left Click for goal pos
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);
                goal.transform.position = hit.point;


                //UnSop all
                foreach (Agent a in agents)
                {
                    a.stopped = false;
                }
            }
        }


    }



}

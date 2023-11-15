using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IKGM : MonoBehaviour
{
    //Singleton pattern 
    private static IKGM _instance;
    public static IKGM Instance { get { return _instance; } }

    public static List<CircleObst> circleObstacales = new List<CircleObst>();
    public static bool GetAngleConstraintState()
    {
        return _instance.doAngleConstraints;
    }

    public static void registerCircleObst(CircleObst c)
    {
        circleObstacales.Add(c);
    }

    public bool doAngleConstraints = true;

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




    public void UpdateAngleConstraint(bool val)
    {
        doAngleConstraints = val;
    }

    public void SwitchToCrowdSim()
    {
        SceneManager.LoadScene(1);
    }


    private void Update()
    {

    }



}

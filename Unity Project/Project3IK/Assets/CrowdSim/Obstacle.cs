using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float radius;



    // Start is called before the first frame update
    void Start()
    {
        

        radius = transform.localScale.x / 2.0f;

        GameManager.registerObstacle(this);
    }

}

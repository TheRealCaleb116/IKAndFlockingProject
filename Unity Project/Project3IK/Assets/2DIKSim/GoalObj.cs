using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Goal : MonoBehaviour
{

    private bool dragging = false;
    private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        //hacky drag
        if (dragging)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        }

    }


    private void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragging= true;
    }

    private void OnMouseUp()
    {
        dragging= false;
    }
}

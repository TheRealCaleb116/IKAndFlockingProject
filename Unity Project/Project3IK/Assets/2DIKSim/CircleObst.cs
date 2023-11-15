using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleObst : MonoBehaviour
{
    // Start is called before the first frame update
    public CirclePrimitive col;

    void Awake()
    {
        IKGM.registerCircleObst(this);
        col = new CirclePrimitive(transform.position, transform.localScale.x / 2);
    }

    private bool dragging = false;
    private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        //hacky drag
        if (dragging)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            col.center = transform.position;
        }

    }

    private void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

}

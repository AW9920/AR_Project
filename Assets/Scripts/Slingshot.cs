using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public float scaling;

    private float max_x = 15f;
    private float min_x = 0f;
    private float max_z = 0f;
    private float min_z = -10f;

    private bool isPressed;
    private bool isConncected;

    private Transform proj;
    private Rigidbody rb;
    private SpringJoint sp;
    private Transform[] pillars;

    private Vector3 init_mousePos;
    private Vector3 proj_init_y;
    // Start is called before the first frame update

    private void Awake()
    {
        // Acquire Components
        proj = GetComponent<Transform>();

        rb = GetComponent<Rigidbody>();

        sp = GetComponent<SpringJoint>();

  

        if (scaling == 0.0f) scaling = 100.0f;  //default value
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            DragBall();
        }
    }

    private void DragBall()
    {
        Vector3 current_mousePos = Input.mousePosition;
        Vector3 delta = (current_mousePos - init_mousePos) / scaling;

        Vector3 new_pos = rb.position + new Vector3(-delta.y, 0.0f, delta.x);

        if (new_pos.x <= max_x && new_pos.x >= min_x)
        {
            if (new_pos.z <= max_z && new_pos.z >= min_z)
            {
                rb.position = new_pos;
            }

            else if (new_pos.z > max_z) rb.position = new Vector3(new_pos.x, 0.0f, max_z);

            else rb.position = new Vector3(new_pos.x, 0.0f, min_z);
        }

        else if (new_pos.x > max_x) rb.position = new Vector3(max_x, 0.0f, new_pos.z);

        else rb.position = new Vector3(min_x, 0.0f, new_pos.z);

        Debug.Log(rb.position);
    }

    private void OnMouseDown()
    {
        isPressed = true;
        rb.isKinematic = true;
        init_mousePos = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        isPressed = false;
        rb.isKinematic = false;
        //break spring connection after a certain time

    }
}

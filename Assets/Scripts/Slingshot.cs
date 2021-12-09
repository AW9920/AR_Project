using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public float scaling_boundary = 1.5f;

    private float max_x = 13f;
    private float min_x = 0f;
    private float max_z = 0f;
    private float min_z = -10f;

    private float init_y;

    private bool isPressed;
    private bool isConncected;

    private Transform proj;
    private Rigidbody rb;
    private SpringJoint sp;
    private Transform[] pillars;
    private SphereCollider col;

    private Vector3 init_mousePos;
    private Vector3 proj_init_y;
    private Vector3 release_pos;
    // Start is called before the first frame update

    private void Awake()
    {
        // Acquire Components
        proj = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        sp = GetComponent<SpringJoint>();

        // Default settings
        if(GetComponent<SphereCollider>() != null) this.GetComponent<SphereCollider>().radius = 0.5f;
        //if (scaling == 0.0f) scaling = 100.0f;

        // Set boundaries
        GameObject[] obj_pillars = GameObject.FindGameObjectsWithTag("Pillar");
        pillars = new Transform[obj_pillars.Length]; 
        for(int i = 0; i < pillars.Length; i++) 
            pillars[i] = obj_pillars[i].GetComponent<Transform>();
        
        if(pillars[0].position.z > pillars[1].position.z)
        {
            max_z = pillars[0].position.z * scaling_boundary;
            min_z = pillars[1].position.z * scaling_boundary;
        }
        else
        {
            max_z = pillars[1].position.z * scaling_boundary;
            min_z = pillars[0].position.z * scaling_boundary;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed & sp.connectedBody != null)
        {
            DragBall();
        }
        else if (!isPressed & sp.connectedBody != null)
        {
            ReleaseBall();
        }
    }

    private void DragBall()
    {
        Vector3 current_mousePos = Input.mousePosition;
        Vector3 delta = (current_mousePos - init_mousePos) * Time.deltaTime * Mathf.Exp(1);

        Vector3 new_pos = rb.position + new Vector3(-delta.y, 0f, delta.x);

        if (new_pos.x <= max_x & new_pos.x >= min_x)
        {
            if (new_pos.z <= max_z & new_pos.z >= min_z) rb.position = new_pos;

            else if (new_pos.z > max_z) rb.position = new Vector3(new_pos.x, init_y, max_z);

            else rb.position = new Vector3(new_pos.x, init_y, min_z);
        }

        else if (new_pos.x > max_x) rb.position = new Vector3(max_x, init_y, new_pos.z);

        else rb.position = new Vector3(min_x, init_y, new_pos.z);

        init_mousePos = current_mousePos;

        Debug.Log(rb.position);
    }


    private void ReleaseBall()
    {
        Vector3 current_pos = this.transform.localPosition;

        if(Vector3.Magnitude(current_pos) <= 0.1f * Vector3.Magnitude(release_pos))
        {
            Destroy(sp);
        }
    }

    private void OnMouseDown()
    {
        isPressed = true;
        rb.isKinematic = true;
        rb.useGravity = false;

        init_mousePos = Input.mousePosition;
        init_y = rb.position.y;
        Debug.Log(init_y);
    }

    private void OnMouseUp()
    {
        isPressed = false;
        rb.isKinematic = false;
        rb.useGravity = true;

        init_mousePos = Vector3.zero;

        release_pos = transform.localPosition; // Position at release
        //break spring connection after a certain time

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    private string[] rand_id = new string[]
    {
        "red",
        "yellow",
        "blue"
    };

    public string id;

    public float scaling_boundary = 0.75f;
    public float dragFriction = 0.1f;
    private float offset = 1f;
    private float max_x = 15f;
    private float min_x = 0f;
    private float max_z;
    private float min_z;
    private float startTime;
    private float LifeTimeOfObject = 5.0f;
    private float release_dist;


    private float init_y;

    private bool isPressed;
    private bool isReleased;
    private bool isConncected;

    public Transform parent;
    private Transform proj;
    private Rigidbody rb;
    private SpringJoint sp;
    private Transform[] pillars;
    private SphereCollider col;
    private MainGameLoop css;
    private Vector3 init_mousePos;
    private Vector3 proj_init_y;
    private Collision target;

    private void Awake()
    {
        Debug.Log("Awake initialized");
        
        // Acquire Components
        proj = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        sp = GetComponent<SpringJoint>();
        css = this.GetComponentInParent<MainGameLoop>();
        parent = this.transform.parent.GetComponent<Transform>();
        // Connect new spring joint
        sp.spring = 50;

        // Default settings
        if(GetComponent<SphereCollider>() != null) this.GetComponent<SphereCollider>().radius = 0.5f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
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

        //Access Main loop
        css.proj_exist = true;
        css.connected = true;

        // Assign random id
        int ran_index = Random.Range(0,3);
        id = rand_id[ran_index];
        
        Debug.Log(id);   
    }

    void Update()
    {
        if (isPressed)
        {
            // Get vector towards parent
            Vector3 dir = transform.position - transform.parent.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.forward,dir);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPressed && sp != null)
        {
            DragBall();
        }
        else if (isReleased && sp != null)
        {
            ReleaseBall();
        }

        if (sp == null && rb != null)
        {
            DestroyBall();
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
    }


    private void ReleaseBall()
    {
        // Kill build up momentum of rigidbody
        rb.angularVelocity *= dragFriction;
        
        // Get Current dist to origin
        float current_dist = Vector3.Distance(transform.position, parent.position);

        if(current_dist <= 0.1f * release_dist)
        {
            // start timer
            startTime = Time.time;
            // break connection
            Debug.Log("Spring broken");
            Destroy(sp);
            css.connected = false;
        }
    }

    private void DestroyBall()
    {
        if((Time.time - startTime) > LifeTimeOfObject)
        {
            css.proj_exist = false;
            Destroy(this.transform.gameObject);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked on Ball");
        isPressed = true;
        isReleased = false;
        rb.isKinematic = true;
        rb.useGravity = false;

        init_mousePos = Input.mousePosition;
        init_y = rb.transform.position.y;

        transform.Translate(Vector3.forward * offset, Space.Self);
    }

    private void OnMouseUp()
    {
        Debug.Log("Release Ball");
        isPressed = false;
        isReleased = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        init_mousePos = Vector3.zero;

        release_dist = Vector3.Distance(parent.position, transform.position); // Position at release
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other == target)
        {
            // Reinstante object life time
            startTime = Time.time; 
            css.isHit = true;
        }
    }
}

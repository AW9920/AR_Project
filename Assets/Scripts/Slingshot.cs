using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    private string[] rand_id = new string[]
    {
        "red",
        "green",
        "blue"
    };
    
    [HideInInspector]
    public string id;

    public float scaling_boundary = 1.5f;
    public float dragFriction = 0.0f;
    [SerializeField]
    private float offset = 0.2f;
    private float max_z = 0.0f;
    private float min_z = -2.0f;
    private float max_x;
    private float min_x;
    private float startTime;
    private float LifeTimeOfObject = 3.0f;
    private float release_dist;
    [SerializeField]
    private float speed = 1.0f;


    private float init_y;

    private bool isPressed;
    private bool isReleased;
    private bool isConncected;

    [HideInInspector]
    public Camera cam;
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
    private Touch touch;

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
        sp.spring = 100;
	    rb.useGravity = false;
	    rb.isKinematic = true;

        // Default settings
        if(GetComponent<SphereCollider>() != null) this.GetComponent<SphereCollider>().radius = 0.5f;
        //rb.constraints = RigidbodyConstraints.FreezeRotationX;

        // Set boundaries
        GameObject[] obj_pillars = GameObject.FindGameObjectsWithTag("Pillar");
        pillars = new Transform[obj_pillars.Length]; 
        for(int i = 0; i < pillars.Length; i++) 
            pillars[i] = obj_pillars[i].GetComponent<Transform>();
        
        if(pillars[0].position.x > pillars[1].position.x)
        {
            max_x = pillars[0].position.x * scaling_boundary;
            min_x = pillars[1].position.x * scaling_boundary;
        }
        else
        {
            max_x = pillars[1].position.x * scaling_boundary;
            min_x = pillars[0].position.x * scaling_boundary;
        }
        Debug.Log(new Vector2(min_x,max_x));

        //Access Main loop
        css.proj_exist = true;
        css.connected = true;

        // Assign random id
        int ran_index = Random.Range(0,3);
        id = rand_id[ran_index];
        // Communicate to main loop script
        css.SetTargetID(id);  
    }

    void Update()
    {
        if (isPressed)
        {
            // Get vector towards parent
            Vector3 dir = transform.position - transform.parent.position;
            transform.rotation = Quaternion.FromToRotation(-Vector3.forward, dir);
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
        // Convert mouse movement info relativ to Camera transform
        if(Input.touchCount > 0)
        {
                touch = Input.GetTouch(0);

                Vector3 delta = new Vector3(
                    touch.deltaPosition.x * Time.deltaTime * speed,
                    0.0f,
                    touch.deltaPosition.y * Time.deltaTime * speed
                );

            // Compute new position beforehand
            Vector3 new_pos = rb.position + delta;

            // Check barrier in x-dir
            if (new_pos.x <= max_x & new_pos.x >= min_x)
            {
                rb.position = new Vector3(new_pos.x, init_y, rb.position.z);    
            }

            else if (new_pos.x > max_x) 
            {
                rb.position = new Vector3(max_x, init_y, rb.position.z);
            }

            else 
            {
                rb.position = new Vector3(min_x, init_y, rb.position.z);
            }

            // Check barrier in z-dir
            if (new_pos.z <= max_z & new_pos.z >= min_z) 
            {
                rb.position = new Vector3(rb.position.x, init_y, new_pos.z);
            }

            else if (new_pos.z > max_z)
            {
                rb.position = new Vector3(rb.position.x, init_y, max_z);
            }

            else 
            {
                rb.position = new Vector3(rb.position.x, init_y, min_z);
            }
        }
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
        // Change properties of rigidbody
        Debug.Log("Clicked on Ball");
        rb.constraints = default;
        isPressed = true;
        isReleased = false;
        rb.isKinematic = true;
        rb.useGravity = false;

        init_y = rb.transform.position.y;

        // Default distance to origin on interaction. Prevents unwanted movement.
        transform.Translate(-Vector3.forward * offset, Space.Self);
    }

    private void OnMouseUp()
    {
        // Change properties of rigidbody
        Debug.Log("Release Ball");
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        isPressed = false;
        isReleased = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        // Delete mouse position
        init_mousePos = Vector3.zero;

        // Position at release
        release_dist = Vector3.Distance(parent.position, transform.position);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other == target)
        {
            // Reinstante object life time
            startTime = Time.time; 
            css.isHit = true;
        }

        else if (other != target)
        {
            css.isHit = false;
        }
    }
}

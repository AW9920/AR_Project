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
    private float min_z = 0.0f;
    private float max_z = 2.0f;
    private float max_x;
    private float min_x;
    private float startTime;
    private float LifeTimeOfObject = 3.0f;
    private float release_dist;
    [SerializeField]
    private float speed = 10.0f;


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
        Transform[] pillars = new Transform[obj_pillars.Length]; 
        for(int i = 0; i < pillars.Length; i++) 
            pillars[i] = obj_pillars[i].GetComponent<Transform>();
        
        // Compute boundaries for projectile
        CalcBoundsX(pillars, out max_x, out min_x);
        CalcBoundsZ(out max_z, out min_z);

        Debug.Log(new Vector2(min_x, max_x));
        Debug.Log(new Vector2(min_z, max_z));

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
        if(true)
        {
            // For Smartphone application
            //Vector3 new_pos = DragWithTouchPos();
            
            // For Desktop application (Debugging)
            Vector3 new_pos = DragWithMousePos(); 

            // Convert rigidbody position from world to local relative to partent
            Vector3 rb_local_pos = rbWorld2Local(rb.position);

            // Check barrier in x-dir
            if (new_pos.x <= max_x & new_pos.x >= min_x)
            {
                rb.position = transform.parent.TransformPoint(new Vector3(new_pos.x, init_y, rb_local_pos.z)); 
                UpdateLocalPos(rb.position, out rb_local_pos);  
            }

            else if (new_pos.x > max_x) 
            {
                rb.position = transform.parent.TransformPoint(new Vector3(max_x, init_y, rb_local_pos.z));
                UpdateLocalPos(rb.position, out rb_local_pos);
            }

            else 
            {
                rb.position = transform.parent.TransformPoint(new Vector3(min_x, init_y, rb_local_pos.z));
                UpdateLocalPos(rb.position, out rb_local_pos);
            }

            // Check barrier in z-dir
            if (new_pos.z <= max_z & new_pos.z >= min_z) 
            {
                rb.position = transform.parent.TransformPoint(new Vector3(rb_local_pos.x, init_y, new_pos.z));
                UpdateLocalPos(rb.position, out rb_local_pos);
            }

            else if (new_pos.z > max_z)
            {
                rb.position = transform.parent.TransformPoint(new Vector3(rb_local_pos.x, init_y, max_z));
                UpdateLocalPos(rb.position, out rb_local_pos);
            }

            else 
            {
                rb.position = transform.parent.TransformPoint(new Vector3(rb_local_pos.x, init_y, min_z));
                UpdateLocalPos(rb.position, out rb_local_pos);
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

        init_mousePos = Input.mousePosition;    //Drag ball with mouse 

        init_y = rb.transform.position.y;

        // Default distance to origin on interaction. Prevents unwanted movement.
        transform.Translate(Vector3.back * offset, Space.Self);
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

    private Vector3 DragWithMousePos()
    {
        Vector3 current_mousePos = Input.mousePosition;
        Vector3 delta = (current_mousePos - init_mousePos) * Time.deltaTime * speed;

        // Compute new position beforehand
        Vector3 new_pos = transform.parent.InverseTransformPoint(rb.position) + new Vector3(delta.x, 0f, delta.y);

        // Update mouse position
        init_mousePos = current_mousePos;
        
        return new_pos;
    }

    private Vector3 DragWithTouchPos()
    {
        touch = Input.GetTouch(0);

        Vector3 delta = new Vector3(
            touch.deltaPosition.x * Time.deltaTime * speed,
            0.0f,
            touch.deltaPosition.y * Time.deltaTime * speed
        );

        // Compute new position beforehand
        Vector3 new_pos = transform.parent.InverseTransformPoint(rb.position) + delta;

        return new_pos;
    }

    private void CalcBoundsX(Transform[] pil, out float max, out float min)
    {
        float dist;
        Vector3 dir;

        if(pil[0].position.x > pil[1].position.x)
        {
            dist = transform.parent.InverseTransformPoint(pil[0].position).magnitude;
            dir = (pil[0].position - transform.parent.position).normalized;
            max = Vector3.Scale(dir, dist * Vector3.right).x;

            dist = transform.parent.InverseTransformPoint(pil[1].position).magnitude;
            dir = (pil[1].position - transform.parent.position).normalized;
            min = Vector3.Scale(dir, dist * Vector3.right).x;
        }
        else
        {
            dist = transform.parent.InverseTransformPoint(pil[0].position).magnitude;
            dir = (pil[1].position - transform.parent.position).normalized;
            max = Vector3.Scale(dir, dist * Vector3.right).x;

            dist = transform.parent.InverseTransformPoint(pil[1].position).magnitude;
            dir = (pil[0].position - transform.parent.position).normalized;
            min = Vector3.Scale(dir, dist * Vector3.right).x;
        }
    }

        private void CalcBoundsZ(out float max, out float min)
    {
        float max_p = transform.parent.InverseTransformPoint(transform.parent.position + Vector3.back * max_z).z;
        float min_p = transform.parent.InverseTransformPoint(transform.parent.position + Vector3.back * min_z).z;

        if (max_p < min_p)
        {
            max = min_p; 
            min = max_p;
        }
        else 
        {
            max = max_p;
            min = min_p;
        }
    }

    private Vector3 rbWorld2Local(Vector3 world_pos)
    {
        Vector3 local_pos = transform.parent.InverseTransformPoint(world_pos);
        return local_pos;
    }

    private void UpdateLocalPos(Vector3 oldPos, out Vector3 newPos)
    {
        newPos = transform.parent.InverseTransformPoint(oldPos);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(CapsuleCollider))]
public class ObstacleBehavior : MonoBehaviour
{
    [HideInInspector]
    public Transform endPoint;
    [HideInInspector]
    public MainGameLoop main;

    private Rigidbody rb;

    private Vector3 initPos;

    private float startTime;
    private float LifeTimeOfObject = 1.0f;
    private float maxDist;
    public float move_speed = 20f;
    private float speed_variation;
    private bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        // Specify variables
        maxDist = Vector3.Distance(transform.position, endPoint.position);
        initPos = this.transform.position;
        speed_variation = Random.Range(0,4) * 5;

        // Get Components
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        // CapsuleCollider Properties
        Vector3 center = new Vector3(0.0f, 0.057f, 0.0f);
        float radius = 1.01f;
        float height = 4.13f;

        // Set CapsuleCollider properties
        col.radius = radius;
        col.height = height;
        col.center = center;

        // Set Rigidbody properties
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHit)
        {
            ObstacleMovement();   
        }

        else if (isHit)
        {
            DestroyObstacle();
        }
    }

    private void ObstacleMovement()
    {
        // Current distance
        Vector3 currentPos = transform.position;
        float dist = Vector3.Distance(initPos, currentPos);

        // compute max step
        float step = (move_speed + speed_variation) * Time.deltaTime;

        if (dist < maxDist)
        {
            // Move towards target
            rb.position = Vector3.MoveTowards(transform.position, endPoint.position, step);
        }

        else if (dist >= maxDist)
        {
            // Destroy object immediately
            Destroy(this.transform.gameObject);
        }
    }

    private void DestroyObstacle()
    {
        if((Time.time - startTime) > LifeTimeOfObject)
        {
            Destroy(this.transform.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        GameObject col = other.gameObject;
        string tag_id = col.tag;

        if(tag_id == "projectile" & !isHit)
        {
            // Switch to Ragdoll behavior
            rb.isKinematic = false;
            rb.useGravity = true;

            // Get impact infoand apply in rb
            Vector3 dir = other.relativeVelocity.normalized;
            float imp = other.impulse.magnitude;
            rb.AddForce(dir * imp, ForceMode.Impulse);

            // Reduce Point count
            main.ReducePointCount();
            //Decrease Life Count
            main.decreaseLifeCount();

            // Initiate Timer
            isHit = true;
            startTime = Time.time;
        }

        else
        {
            // Switch to Ragdoll behavior
            rb.isKinematic = false;
            rb.useGravity = true;

            isHit = true;
            startTime = Time.time;
        }
    }

    private void OnDestroy() 
    {
        // Get Component handling obstacles
        ObstacleHandler handler_css = gameObject.GetComponentInParent<ObstacleHandler>(); 

        // Change variable states
        if(handler_css != null)
        {
            handler_css.isPresent = false;
            handler_css.ResetSpawnTimeOnDestroy(true); 
        }
    }
}

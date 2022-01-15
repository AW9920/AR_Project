using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    public float speed = 1.0f;
    public float force_amp = 1.0f;
    public string id;
    public MainGameLoop main;
    private Transform target_Trans;
    private Vector3 start;
    private Vector3 end;
    private float dist_hit;
    private float start_time;
    private bool hit;

    // Start is called before the first frame update
    void Start()
    {

        string tag = this.tag;

        if(tag == "b_tar") id = "blue";

        else if(tag == "y_tar") id = "yellow";

        else if(tag == "r_tar") id = "red";

        // Debug.Log(id);
    }

    // Update is called once per frame
    void Update()
    {   
        if (hit)
        {
            float distCover = (Time.time - start_time) * speed;
            float lengthFrac = distCover / dist_hit;
            Vector3 current_pos = target_Trans.position;

            target_Trans.position = Vector3.Lerp(target_Trans.position, transform.GetChild(0).position, lengthFrac); 

            if (Vector3.Distance(current_pos, end) <= 0.5f)
            {
                Destroy(target_Trans.gameObject);
                main.proj_exist = false;
                hit = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        GameObject col = other.gameObject;
        Slingshot css = col.GetComponent<Slingshot>();;

        string col_tag = col.tag;

        float relVel = other.relativeVelocity.magnitude;

        if (col_tag == "projectile")
        {
            main.isHit = true;

            main.target_id = id;

            string col_id = css.id;

            if (col_id == id)
            {
                bool correct = true;
                // Increase point count
                main.IncreasePointCount(correct);

                target_Trans = RemoveOnHit(col);
            }
                    
            else if (col_id != id)
            {
                RepelOnHit(col,relVel);
            } 
        }  
    }

    private Transform RemoveOnHit(GameObject obj)
    {
        start = obj.transform.position;
        end = transform.GetChild(0).position;
        dist_hit = Vector3.Distance(start, end);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        BoxCollider bc = obj.GetComponent<BoxCollider>();
        Destroy(rb);
        Destroy(bc);

        start_time = Time.time;

        hit = true;
        
        return obj.transform;
    }

    private void RepelOnHit(GameObject obj, float velocity)
    {
        Vector3 rand_dir = Random.insideUnitCircle.normalized;
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        // Prevent random direction into collider
        rand_dir.x = Mathf.Abs(rand_dir.x);
        rand_dir.y = Mathf.Abs(rand_dir.y);

        // Reset velocity of rb
        rb.velocity *= 0f;

        Vector3 FlyOff_force = rand_dir * velocity * force_amp;
        Debug.Log(FlyOff_force);

        // Add force to rigidbody
        rb.AddForce(FlyOff_force, ForceMode.VelocityChange);

        Debug.DrawRay(this.transform.position, rand_dir * 100f);
        Debug.Log(rand_dir);
    }
}

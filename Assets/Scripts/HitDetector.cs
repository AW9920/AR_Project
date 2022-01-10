using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    public string id;
    public MainGameLoop main;

    // Start is called before the first frame update
    void Start()
    {
        string tag = this.tag;

        if(tag == "b_tar") id = "blue";

        else if(tag == "y_tar") id = "yellow";

        else if(tag == "r_tar") id = "red";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) 
    {
        GameObject col = other.gameObject;
        Slingshot css = col.GetComponent<Slingshot>();

        if (col.tag == "projectile")
        {
            main.isHit = true;

            main.target_id = id;

            string col_id = css.id;

            if (col_id == id)
            {
                bool correct = true;
                // Increase point count
                main.IncreasePointCount(correct);
            }
        }   
    }
}

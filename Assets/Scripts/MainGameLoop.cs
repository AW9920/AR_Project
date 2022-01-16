using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameLoop : MonoBehaviour
{
    private int points = 0;
    public int penelty = 10;
    public float startScoreTime;
    [HideInInspector]
    public string target_id;
    private GameObject proj;
    private static Quaternion rot;
    private static Vector3 pos;

    [HideInInspector]
    public bool proj_exist;
    [HideInInspector]
    public bool connected = false;
    [HideInInspector]
    public bool isHit;
    private GameObject projectile;
    public GameObject prefab;
    private GameObject obj;
    public Text scoreCount;
    
    void Awake()
    {
        //Extract Gameobject from Prefab+
        obj = prefab.transform.GetChild(0).gameObject;
        // Instantiate parameters
        rot = Quaternion.identity;
        pos = this.transform.position;

        //First projectile already loaded
        if (transform.childCount == 0)
            InitiateProjectile();

        // Get script
        scoreCount.text = "0";
    }

    // Update is called once per frame
    void Update()
    {   
        // Instantiate new projectile
        if (!proj_exist)
        {
            // Create new projectile & add to hierarchy
            InitiateProjectile();
            //Reset score timer
            startScoreTime = Time.time;
        }

        if(isHit)
        {
            if (target_id == "blue")
            {
                
            }

            else if (target_id == "yellow")
            {

            }

            else if (target_id == "red")
            {

            }
        }
    }

    protected void InitiateProjectile()
    {
        //Quaternion rot_z = Quaternion.AngleAxis(90f, Vector3.up);
        Quaternion rot = Quaternion.AngleAxis(90f,Vector3.up);
        Vector3 pos = this.transform.position;

        // Instantiate new projectile & add to hierarchy
        GameObject new_proj =  Instantiate(obj, pos, rot);
        new_proj.transform.localScale = new Vector3 (1f, 1f, 1f);
        new_proj.transform.SetParent(this.transform);
        new_proj.name = "Projectile";
        new_proj.tag = "projectile";

        // Add components
        new_proj.AddComponent<SpringJoint>().connectedBody = GetComponent<Rigidbody>();
        new_proj.AddComponent<Slingshot>();
        proj_exist = true;
    }

    public void IncreasePointCount(bool correct)
    {
        if (correct)
        {
            float timeDiff = Time.time - startScoreTime;
            //Add 100 / (time diff) to point count
            int point_add = (int)100f / (int)timeDiff;

            points += point_add; 

            scoreCount.text = points.ToString();
        }
    }

    public void ReducePointCount()
    {
        points -= penelty; 

        scoreCount.text = points.ToString();
    }
}
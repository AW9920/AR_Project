using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameLoop : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {   
        // Instantiate new projectile
        if (!proj_exist)
        {
            // Create new projectile & add to hierarchy
            InitiateProjectile();
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

        // Add components
        new_proj.AddComponent<SpringJoint>().connectedBody = GetComponent<Rigidbody>();
        new_proj.AddComponent<Slingshot>();
        proj_exist = true;
    }
}
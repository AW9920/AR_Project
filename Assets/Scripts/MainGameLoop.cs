using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameLoop : MonoBehaviour
{
    public int lifeCount = 3;
    private int points = 0;
    public int penelty = 10;
    public float startScoreTime;
    public float GameOverTime = 0.5f;
    private float startTime;
    [HideInInspector]
    private string target_id;
    private static Quaternion rot;
    private static Vector3 pos;

    [HideInInspector]
    public bool proj_exist;
    [HideInInspector]
    public bool connected = false;
    [HideInInspector]
    public bool isOver = false;
    [HideInInspector]
    public bool isHit;
    public GameObject prefab;
    private GameObject obj;
    public Text scoreCount;
    public Text GameOverText;
    public SceneChanger scene_css;
    public TargetIndicator[] sign_css;
    public Image[] hearts;
    [SerializeField]
    private Camera cam;
    
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
        GameOverText.enabled = false;

        // Attach Scene handler
        scene_css = GameObject.FindGameObjectWithTag("scene_handler").GetComponent<SceneChanger>();
    }

    // Update is called once per frame
    void Update()
    {   
        // Instantiate new projectile
        if (!proj_exist & !isOver)
        {
            // Create new projectile & add to hierarchy
            InitiateProjectile();
            //Reset score timer
            startScoreTime = Time.time;
        }

        if (isOver)
        {  
            // Destroy everything
            destroyAllObjects();

            // Change glow to red
            for(int i = 0; i < sign_css.Length; i++)
            {
                Color color = Color.red;
                sign_css[i].setSignColor(color);
            }

            // Change scene after 2s
            if((Time.time - startTime) > GameOverTime)
            {
                scene_css.FadeToScene(0);
            }
        }

    }

    protected void InitiateProjectile()
    {
        //Quaternion rot_z = Quaternion.AngleAxis(90f, Vector3.up);
        Quaternion rot = this.transform.rotation;
        Vector3 pos = this.transform.position;

        // Instantiate new projectile & add to hierarchy
        GameObject new_proj =  Instantiate(obj, pos, rot);
        new_proj.transform.SetParent(this.transform);
        new_proj.transform.localScale = new Vector3 (1f, 1f, 1f);
        new_proj.name = "Projectile";
        new_proj.tag = "projectile";

        // Add components
        new_proj.AddComponent<SpringJoint>().connectedBody = GetComponent<Rigidbody>();
        new_proj.AddComponent<Slingshot>();
        new_proj.GetComponent<Slingshot>().cam = cam;

        // Attach Main Game Loop script to specific object
        GameObject[] ancs = GameObject.FindGameObjectsWithTag("ancor");
        for(int i = 0; i < ancs.Length; i++)
        {
            BandGen rubber_css = ancs[i].GetComponent<BandGen>();
            if(rubber_css != null)
            {
                rubber_css.main = GetComponent<MainGameLoop>();
            }
        }

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

    private void destroyAllObjects()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obst");
        for(int i = 0; i < obstacles.Length; i++)
        {
            Destroy(obstacles[i]);
        }

        GameObject[] proj = GameObject.FindGameObjectsWithTag("projectile");
        for(int i =0; i < proj.Length; i++)
        {
            Destroy(proj[i]);
        }
    }

    public void ReducePointCount()
    {
        points -= penelty; 

        scoreCount.text = points.ToString();
    }

    public void SetTargetID(string id)
    {
        target_id = id;
    }

    public string GetTargetID()
    {
        return target_id;
    }

    public void decreaseLifeCount()
    {
        // Remove Heart from UI
        hearts[lifeCount-1].enabled = false;

        // Decrease Life Count
        lifeCount -= 1;

        if(lifeCount == 0)
        {
            // Switch to GameOver state
            isOver = true;
            GameOverText.enabled = true;
            startTime = Time.time;
        }
    }
}
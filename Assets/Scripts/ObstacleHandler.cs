using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleHandler : MonoBehaviour
{
    public GameObject obstacle;
    public MainGameLoop main;
    private Transform target;

    [HideInInspector]
    public bool isPresent;
    private float startTime;
    private float randSpawnTime;

    private void Start() 
    {
        target = transform.GetChild(0); 
        isPresent = false;

        // Setup spawn timer
        startTime = Time.time; 
        ResetSpawnTimeOnDestroy(true);


    }

    // Update is called once per frame
    void Update()
    {
        // Check for GameOver condition
        bool isOver = main.isOver;

        if(!isPresent & !isOver)
        {
            if((Time.time - startTime) >= randSpawnTime)
            {
                InitiateObstacle();
                isPresent = true;
            }
        }
    }

    protected void InitiateObstacle()
    {
        // Instantiation position and roation
        Vector3 pos = transform.position;
        Quaternion rot = Quaternion.identity;

        // Instantiate object at spawn point. Set properties
        GameObject new_obst =  Instantiate(obstacle, pos, rot);
        new_obst.transform.SetParent(this.transform);
        new_obst.tag = "obst";
        ObstacleBehavior css = new_obst.GetComponent<ObstacleBehavior>();
        css.main = main;  // pass on main loop reference
        css.endPoint = target;
    }

    public void ResetSpawnTimeOnDestroy(bool isDestroyed)
    { 
        if (isDestroyed)
        {
            startTime = Time.time;
            randSpawnTime = (float) Random.Range(1, 4);
        }
    }
}

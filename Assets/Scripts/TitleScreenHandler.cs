using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenHandler : MonoBehaviour
{
    private bool isStart = false;
    private bool isScore = false;
    private bool isQuit = false;
    private float startTime;
    public float speed = 1.0f;

    private float distLength;

    public GameObject proj;

    public Transform start;
    public Transform stop;
    public Transform interactStop;

    public SceneChanger fade;

    // Start is called before the first frame update
    private void Start() 
    {
        startTime = Time.time;   

        distLength = Vector3.Distance(start.position, stop.position);
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;

        float fractionOfJourney = distCovered / distLength;

        proj.transform.position = Vector3.Lerp(start.position, stop.position, fractionOfJourney);
    }

    public void startGame()
    {
        isStart  = true;

        MoveOutOfScreenSettings();

        fade.FadeToScene(1);
    }

    public void Scoreboard()
    {
        isScore = true;
    }

    public void QuitGame()
    {
        isQuit = true;
    }

    void MoveOutOfScreenSettings()
    {
        // Reset Timer
        startTime = Time.time;

        // Recalc distance
        distLength = Vector3.Distance(stop.position, interactStop.position);

        // Reste start end position
        start = stop;
        stop = interactStop;
    }
}

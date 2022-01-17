using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    public MainGameLoop main;
    private Material glow;
    private string current_target;
    public string own_id;
    public float emissionInt = 6.0f;

    // Start is called before the first frame update
    void Start()
    {
        glow = GetComponent<MeshRenderer>().material;
        
        // Setup default condition
        glow.DisableKeyword("_EMISSION");
        glow.SetColor("_EmissionColor", Color.green * emissionInt);
    }

    private void Update() 
    {
        string current_target = main.GetTargetID();

        if(current_target.Equals(own_id))
        {
            TurnLightOn();
        }
        else if (!current_target.Equals(own_id))
        {
            TurnLightOff();
        }
    }

    public void TurnLightOff()
    {
        glow.DisableKeyword("_EMISSION");
    }

    public void TurnLightOn()
    {
        glow.EnableKeyword("_EMISSION");
        glow.SetColor("_EmissionColor", Color.green * emissionInt);
    }
}

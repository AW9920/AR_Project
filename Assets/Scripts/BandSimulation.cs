using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandSimulation : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision col) 
    {
        // Get contact position in world space
        ContactPoint[] contacts = col.contacts;

    }
}

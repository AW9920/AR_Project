using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandGen : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private float width = 2f;
    private float length;
    private float length_Bed;
    // Band ancor objects
    private GameObject Anc_PL;
    private GameObject Anc_PR;
    private GameObject Anc_BL;
    private GameObject Anc_BR;

    // Main scipt
    private MainGameLoop main;
    // Start is called before the first frame update
    void Update()
    {
        // Get Ancor objects (Prefabs instantiated at startup)
        Anc_PL = GameObject.FindWithTag("PL_Anc");
        Anc_PR = GameObject.FindWithTag("PR_Anc");
        Anc_BL = GameObject.FindWithTag("BL_Anc");
        Anc_BR = GameObject.FindWithTag("BR_Anc");

        // Get Main Loop script
        main = this.GetComponent<MainGameLoop>();

        if (main.connected)
        {
            CreateShapeL();
            CreateShapeM();
            CreateShapeR();
        }
        else if (!main.connected)
        {
            if((Anc_BL.GetComponent<MeshFilter>().mesh != null) && (Anc_PR.GetComponent<MeshFilter>().mesh != null))
            {
                Anc_BL.GetComponent<MeshFilter>().mesh = null;
                Anc_PR.GetComponent<MeshFilter>().mesh = null;
            }
            
            CreateShapeWhole();
        }
    }

    void CreateShapeL()
    {
        // Vector Pillar left to Bed right
        Vector3 dist = Anc_BL.transform.position - Anc_PL.transform.position;
        length = dist.magnitude;

        // Rotate towards Ancor
        Anc_PL.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dist);

        mesh = new Mesh();
        Anc_PL.GetComponent<MeshFilter>().mesh = mesh;

        // Create Meshes
        vertices = new Vector3[]
        {
            new Vector3(0, -width/2, 0),
            new Vector3(0, width/2, 0),
            new Vector3(0, -width/2, length),
            new Vector3(0, width/2, length)
        };

        triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2
        };

        // Update mesh
        UpdateMesh();
    }
    void CreateShapeM()
    {   
        // Vector Pillar left to Bed right
        Vector3 dist = Anc_BR.transform.position - Anc_BL.transform.position;
        length = dist.magnitude;

        mesh = new Mesh();
        Anc_BL.GetComponent<MeshFilter>().mesh = mesh;

        // Create Meshes
        vertices = new Vector3[]
        {
            new Vector3(0, -width/2, 0),
            new Vector3(0, width/2, 0),
            new Vector3(0, -width/2, length),
            new Vector3(0, width/2, length)
        };

        triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2
        };

        // Update mesh
        UpdateMesh();
    }
    void CreateShapeR()
    {
        // Vector Pillar left to Bed right
        Vector3 dist = Anc_PR.transform.position - Anc_BR.transform.position;
        length = dist.magnitude;

        // Rotate towards Ancor
        Anc_PR.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dist);

        mesh = new Mesh();
        Anc_PR.GetComponent<MeshFilter>().mesh = mesh;

        // Create Meshes
        vertices = new Vector3[]
        {
            new Vector3(0, -width/2, 0),
            new Vector3(0, width/2, 0),
            new Vector3(0, -width/2, -length),
            new Vector3(0, width/2, -length)
        };

        triangles = new int[]
        {
            2, 1, 0,
            2, 3, 1
        };

        // Update mesh
        UpdateMesh();
    }

    void CreateShapeWhole()
    {
        // Vector Pillar left to Bed right
        Vector3 dist = Anc_PR.transform.position - Anc_PL.transform.position;
        length = dist.magnitude;

        Anc_PL.transform.rotation = Quaternion.identity;

        mesh = new Mesh();
        Anc_PL.GetComponent<MeshFilter>().mesh = mesh;

        // Create Meshes
        vertices = new Vector3[]
        {
            new Vector3(0, -width/2, 0),
            new Vector3(0, width/2, 0),
            new Vector3(0, -width/2, length),
            new Vector3(0, width/2, length)
        };

        triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2
        };

        // Update mesh
        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();   
    }
}

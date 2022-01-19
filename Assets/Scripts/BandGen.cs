using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandGen : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private float width = 2f;
    
    // Band ancor objects
    [SerializeField]
    private Transform Anc_BR;
    // Main scipt
    public MainGameLoop main;
    // Start is called before the first frame update

    void Update()
    {
        if (main.connected)
        {
            CreateShapeM();
        }

        else if (!main.connected)
        {
            ClearMesh();
        }
    }

    void CreateShapeM()
    {   
        // Vector Pillar left to Bed right
        Vector3 dir = transform.InverseTransformPoint(Anc_BR.position) - transform.InverseTransformPoint(transform.position);
        float length = dir.magnitude;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Determine direction to render mesh
        Vector3 relative = Vector3.Scale(Vector3.forward, dir).normalized;

        // Create Meshes
        vertices = new Vector3[]
        {
            new Vector3(0, -width/2, 0),
            new Vector3(0, width/2, 0),
            new Vector3(0, -width/2, length),
            new Vector3(0, width/2, length)
        };
        
        if(relative.z > 0)
        {
            triangles = new int[]
            {
                0, 1, 2,
                1, 3, 2
            };
        }

        else
        {
            triangles = new int[]
            {
                2, 1, 0,
                2, 3, 1
            };
        }

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

    private void ClearMesh()
    {
        this.GetComponent<MeshFilter>().mesh = null;
    }
}

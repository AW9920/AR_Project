using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBandMotion : MonoBehaviour
{
    [SerializeField]
    private MainGameLoop main;
    private Transform ancor;
    [SerializeField]
    private Transform op_Pillar;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private float width = 2.0f;

    // Update is called once per frame
    void Update()
    {
        if(main.connected & main.proj_exist)
        {
            if(ancor == null)
            {
                ancor = FindClosestAncor();
            }
            CreateShape(ancor);
        }

        else if (!main.connected | main.isOver)
        {
            CreateShapeWhole();
        }
    }

    private void CreateShape(Transform anc)
    {
        // Vector Pillar left to Bed right

        Vector3 dir = transform.parent.InverseTransformPoint(anc.position) - transform.parent.InverseTransformPoint(transform.position);
        float length = dir.magnitude;

        // Rotate towards Ancor
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, dir);
        rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, 0f);
        this.transform.rotation = rot;

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

    void CreateShapeWhole()
    {
        // Vector Pillar left to Bed right
        Vector3 dir = transform.parent.InverseTransformPoint(op_Pillar.position) - transform.parent.InverseTransformPoint(transform.position);
        float length = dir.magnitude;
        //Debug.Log(length);

        transform.rotation = Quaternion.identity;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Determine direction to render mesh
        Vector3 relative = Vector3.Scale(Vector3.right, dir).normalized;

        // Create Meshes
        vertices = new Vector3[]
        {
            new Vector3(0, -width/2, 0),
            new Vector3(0, width/2, 0),
            new Vector3(length * relative.x, -width/2, 0),
            new Vector3(length * relative.x, width/2, 0)
        };

        // flip array if rendered against z-axis
        if(relative.x > 0)
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

    private Transform FindClosestAncor()
    {
        GameObject[] ancs;
        ancs = GameObject.FindGameObjectsWithTag("ancor");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject anc in ancs)
        {
            Vector3 diff = anc.transform.position - position;
            // Exclude self
            if(!diff.Equals(Vector3.zero))
            {
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = anc;
                    distance = curDistance;
                }
            }
        }
        return closest.transform;
    }
}

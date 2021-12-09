using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIZMO : MonoBehaviour
{
    public float GizSize = 3;

    public Color color_sphere = Color.red;
    public Color color_wire = Color.white;

    private void Awake()
    {
        if (color_sphere == null) color_sphere = Color.red;
        if (color_wire == null) color_wire = Color.white;
        Debug.Log(color_sphere);
        Debug.Log(color_wire);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color_sphere;
        Gizmos.DrawSphere(transform.position, GizSize);

        Gizmos.color = color_wire;
        Gizmos.DrawWireSphere(transform.position, GizSize); 
    }
}

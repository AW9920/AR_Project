using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIZMO : MonoBehaviour
{
    public float GizSize = 0;

    public bool RayX, RayY, RayZ;

    public Color color_sphere = Color.red;
    public Color color_wire = Color.white;

    private void Awake()
    {
        if (color_sphere == null) color_sphere = Color.red;
        if (color_wire == null) color_wire = Color.white;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color_sphere;
        Gizmos.DrawSphere(transform.position, GizSize);

        Gizmos.color = color_wire;
        Gizmos.DrawWireSphere(transform.position, GizSize); 

        if (RayX) Gizmos.DrawRay(transform.position, transform.right * 5f);
        else if (RayY) Gizmos.DrawRay(transform.position, transform.up * 5f);
        else if (RayZ) Gizmos.DrawRay(transform.position, transform.forward * 5f);
    }
}

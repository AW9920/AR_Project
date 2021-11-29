using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIZMO : MonoBehaviour
{
    public int GizSize = 3;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, GizSize);

        Gizmos.DrawWireSphere(transform.position, GizSize + 0.1f);
        Gizmos.color = Color.white;
    }
}

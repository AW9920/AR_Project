using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]

public class ObjectOnPlane : MonoBehaviour
{
    private ARRaycastManager raycastMang;
    private ARPlaneManager planeManager;
    private GameObject spawnedObject;

    public GameObject PlaceablePrefab;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastMang = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    private void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (raycastMang.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPos = s_Hits[0].pose;
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(PlaceablePrefab, hitPos.position, hitPos.rotation);
                SetAllPlanesActive(false);
            }
            
           /* else
            {
                spawnedObject.transform.position = hitPos.position;
                spawnedObject.transform.rotation = hitPos.rotation;
            }*/

        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    private void SetAllPlanesActive(bool value)
    {
        foreach(var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
}

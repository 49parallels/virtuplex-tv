using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager), typeof(ARSessionOrigin))]
public class PlaceOnPlane : MonoBehaviour
{
    // Manager properties
    private ARRaycastManager arRayCastManager;
    private ARSessionOrigin arSessionOrigin;

    [Tooltip("Prefab to be shown on tap.")]
    [SerializeField]
    private GameObject prefabToPlace;

    public static UnityAction onPlacedObject;

    private ScriptableManager scriptableManager;
    // Result of successful raycast intersection with plane
    public GameObject spawnedObject = default;
    
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Vector2 touchPosition = default;

    private bool isPanRotating;
    private bool isRelocating;

    private Vector3 initialScale;
    
    public Quaternion targetRotation;
    public Vector3 targetScale;
    

    bool TryGetTouch(out Touch touch)
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            return true;
        }

        touch = default;
        return false;
    }
    
    void Awake()
    {
        // Instantiate required managers
        arSessionOrigin = GetComponent<ARSessionOrigin>();
        arRayCastManager = GetComponent<ARRaycastManager>();
        scriptableManager = GetComponent<ScriptableManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Try to get touch on screen
        if (!TryGetTouch(out Touch touch))
                return;

        // extract position from touch
        touchPosition = touch.position;

        // Intersect found plane with raycast
        if (!IsPointerOverUIObject() && arRayCastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinInfinity))
        {
            Pose hitPose = hits[0].pose;
            
            if (!spawnedObject)
            {
                // Spawn game object
                spawnedObject = Instantiate(prefabToPlace, hitPose.position, hitPose.rotation);
                arSessionOrigin.MakeContentAppearAt(spawnedObject.transform, hitPose.position, hitPose.rotation);
                targetRotation = spawnedObject.transform.rotation;
                targetScale = arSessionOrigin.transform.localScale;
                scriptableManager.ControlState.IsSpawned = true;
                onPlacedObject();
            } else if (scriptableManager.ControlState.IsRelocating)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    // relocate object if IsRelocating enabled
                    arSessionOrigin.MakeContentAppearAt(spawnedObject.transform, hitPose.position, hitPose.rotation);
                }
            }
        }
        
        if (spawnedObject)
        {
            // Set target rotation and scale calculated in LateUpdate()
            arSessionOrigin.MakeContentAppearAt(spawnedObject.transform, spawnedObject.transform.position,
                targetRotation);
            arSessionOrigin.transform.localScale = targetScale;
        }
    }

    // We do all the panrotationg touch logic in lateupdate
    private void LateUpdate()
    {
        if (spawnedObject && !scriptableManager.ControlState.IsRelocating)
        {
            initialScale = arSessionOrigin.transform.localScale;
            
            DetectTouchMovement.Calculate();

            if (Mathf.Abs(DetectTouchMovement.pinchFactor) > 0.0f)
            {
                // target scale
                targetScale = initialScale * DetectTouchMovement.pinchFactor;
            }

            if (Mathf.Abs(DetectTouchMovement.turnAngleDelta) > 0.0f)
            {
                // target rotation
                Vector3 rotationDeg = Vector3.zero;
                rotationDeg.y = -DetectTouchMovement.turnAngleDelta;
                targetRotation *= Quaternion.Euler(rotationDeg);
            }
        }
    }
    
    // Don't let raycast pass through UI element
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return (results.Count > 0 && spawnedObject != null);
    }
}

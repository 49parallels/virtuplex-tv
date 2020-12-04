using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * DoorController - is used for accesing and invoking
 * actions on Door Scripts (open, close etc.)
 */
public class DoorController : MonoBehaviour
{
    
    private RaycastHit hit;
    private Camera camera;

    // Start is called before the first frame update
    void Awake()
    {
        // Assign camera here because using Camera.main in Update is expensive
        camera = Camera.main;
    }
    
    bool TryGetTouch(out Touch touch)
    {
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            return true;
        }

        touch = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TryGetTouch(out Touch touch))
            return;
        
        if (touch.phase == TouchPhase.Began)
        {
            Physics.Raycast(camera.ScreenPointToRay(touch.position), out hit);
            BoxCollider boxCollider = hit.collider as BoxCollider;
            if (boxCollider)
            {
                StartCoroutine(nameof(InvokeRotation), boxCollider);
            }
        }
    }

    /*Moved GetComponent to coroutine it is bad practice and expensive (in case of batch of colliders)
     to use the method in Update*/
    IEnumerator InvokeRotation(BoxCollider collider)
    {
        DoorOpenClose door = collider.GetComponent<DoorOpenClose>();
        door.StartRotation();
        yield return null;
    }
}

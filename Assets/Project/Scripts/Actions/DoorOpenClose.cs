using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DoorOpenClose : MonoBehaviour
{
    // Door types
    enum DoorType
    {
        FrontLeft, // Front left door
        FrontRight, // Front right door
        BackLeft, // Back left door
        BackRight, // Back right door
        Tailgate  // Trunk
    }

    [Tooltip("Select what door is this component representing.")]
    [SerializeField]
    private DoorType doorType = DoorType.FrontLeft;
    
    
    [Tooltip("Drag and drop GameObject representing door.")]
    [SerializeField]
    private GameObject door;
 
    private bool rotating ;
    private bool isOpen = false;

    private Vector3 openOrClose;
    
    private Vector3 close;
    private Vector3 open;

    private void Awake()
    {
        switch (doorType)
        {
            case DoorType.FrontLeft:
                open = new Vector3(0, 45, 0);
                close = new Vector3(0, -45, 0);
                break;
            case DoorType.FrontRight:
                open = new Vector3(0, -45, 0);
                close = new Vector3(0, 45, 0);
                break;
            case DoorType.BackRight:
                open = new Vector3(0, -45, 0);
                close = new Vector3(0, 45, 0);
                break;
            case DoorType.BackLeft:
                open = new Vector3(0, 45, 0);
                close = new Vector3(0, -45, 0);
                break;
            case DoorType.Tailgate:
                open = new Vector3(75, 0, 0);
                close = new Vector3(-75, 0, 0);
                break;
        }
    }

    private IEnumerator Rotate( Vector3 angles, float duration )
    {
        rotating = true ;
        Quaternion startRotation = door.transform.rotation ;
        Quaternion endRotation = Quaternion.Euler( angles ) * startRotation ;
        for( float t = 0 ; t < duration ; t+= Time.deltaTime )
        {
            door.transform.rotation = Quaternion.Lerp( startRotation, endRotation, t / duration ) ;
            yield return null;
        }
        door.transform.rotation = endRotation  ;
        rotating = false;
        isOpen = !isOpen;
    }
 
    public void StartRotation()
    {
        openOrClose = isOpen ? close : open;
        if( !rotating )
            StartCoroutine( Rotate( openOrClose, 1 ) ) ;
    }
}

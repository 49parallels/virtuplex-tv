using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ScriptableManager))]
public class UIControl : MonoBehaviour
{
    private ScriptableManager scriptableManager;

    [SerializeField] private Button relocateButton;
    [SerializeField] private GameObject colorPallete;
    [SerializeField] private GameObject gestureInstructions;
    
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    ARCameraManager m_CameraManager;

    /// <summary>
    /// Get or set the <c>ARCameraManager</c>.
    /// </summary>
    public ARCameraManager cameraManager
    {
        get { return m_CameraManager; }
        set
        {
            if (m_CameraManager == value)
                return;

            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= FrameChanged;

            m_CameraManager = value;

            if (m_CameraManager != null & enabled)
                m_CameraManager.frameReceived += FrameChanged;
        }
    }

    const string k_FadeOffAnim = "FadeOff";
    const string k_FadeOnAnim = "FadeOn";
    

    [SerializeField]
    ARPlaneManager m_PlaneManager;

    public ARPlaneManager planeManager
    {
        get { return m_PlaneManager; }
        set { m_PlaneManager = value; }
    }

    [SerializeField]
    Animator m_MoveDeviceAnimation;

    public Animator moveDeviceAnimation
    {
        get { return m_MoveDeviceAnimation; }
        set { m_MoveDeviceAnimation = value; }
    }

    [SerializeField]
    Animator m_TapToPlaceAnimation;

    public Animator tapToPlaceAnimation
    {
        get { return m_TapToPlaceAnimation; }
        set { m_TapToPlaceAnimation = value; }
    }

    static List<ARPlane> s_Planes = new List<ARPlane>();

    bool m_ShowingTapToPlace = false;
    bool m_ShowingMoveDevice = true;
    
    // Start is called before the first frame update
    private void Awake()
    {
        scriptableManager = GetComponent<ScriptableManager>();
    }

    private void Update()
    {
        relocateButton.gameObject.SetActive(scriptableManager.ControlState.IsSpawned);
        colorPallete.SetActive(scriptableManager.ControlState.IsSpawned);
    }

    public void ToggleRelocate()
    {
        scriptableManager.ControlState.IsRelocating = !scriptableManager.ControlState.IsRelocating;
        if (scriptableManager.ControlState.IsRelocating)
        {
            relocateButton.GetComponentInChildren<Text>().text = "Turn Off Relocate";
        }
        else
        {
            relocateButton.GetComponentInChildren<Text>().text = "Turn On Relocate";
        }
    }
    
    void OnEnable()
    {
        if (m_CameraManager != null)
            m_CameraManager.frameReceived += FrameChanged;

        PlaceOnPlane.onPlacedObject += PlacedObject;
    }

    void OnDisable()
    {
        if (m_CameraManager != null)
            m_CameraManager.frameReceived -= FrameChanged;

        PlaceOnPlane.onPlacedObject -= PlacedObject;
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        if (PlanesFound() && m_ShowingMoveDevice)
        {
            if (moveDeviceAnimation)
                moveDeviceAnimation.SetTrigger(k_FadeOffAnim);

            if (tapToPlaceAnimation)
                tapToPlaceAnimation.SetTrigger(k_FadeOnAnim);

            m_ShowingTapToPlace = true;
            m_ShowingMoveDevice = false;
        }
    }

    bool PlanesFound()
    {
        if (planeManager == null)
            return false;

        return planeManager.trackables.count > 0;
    }

    void PlacedObject()
    {
        if (m_ShowingTapToPlace)
        {
            if (tapToPlaceAnimation)
                tapToPlaceAnimation.SetTrigger(k_FadeOffAnim);

            m_ShowingTapToPlace = false;
            StartCoroutine("ShowInstructions");
        }
    }

    IEnumerator ShowInstructions()
    {
        gestureInstructions.SetActive(true);
        yield return new WaitForSeconds(10);
        Destroy(gestureInstructions);
    }
}

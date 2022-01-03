using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopCameraController : MonoBehaviour
{
    private CustomInputActions cameraInputAsset; // inputactions defined in the file

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    // horizontal rotation speed
    public float horizontalSpeed = 1f;
    // vertical rotation speed
    public float verticalSpeed = 1f;
    public float movementSpeed = 0.2f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private Camera cam;
    private GameObject camObject;

    float h, v, mouseX, mouseY;
    bool activateInput = false;

    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        cameraInputAsset.asset.Disable();
    }


    void Start()
    {
        cam = Camera.main;
        camObject = cam.gameObject;

        initialPosition = camObject.transform.position;
        initialRotation = camObject.transform.eulerAngles;

        xRotation = initialRotation.x;
        yRotation = initialRotation.y;
    }
 

    void Update()
    {
        if(activateInput)
            UpdatePositionAndRotationFromInput();
    }

    void EnableInput()
    {
        if (cameraInputAsset == null)
        {
            cameraInputAsset = new CustomInputActions();
        }
            
        cameraInputAsset.asset.Enable();

        if (cameraInputAsset.asset.enabled)
        {
            cameraInputAsset.Player.EnableOrbit.performed += ctx => activateInput = true;
            cameraInputAsset.Player.EnableOrbit.canceled += ctx => activateInput = false;
            cameraInputAsset.Player.Move.performed += ctx => UpdateMovement(ctx.ReadValue<Vector2>());
            cameraInputAsset.Player.Move.canceled += ctx => UpdateMovement(new Vector2(0f, 0f));
            cameraInputAsset.Player.Look.performed += ctx => UpdateMouseOrbit(ctx.ReadValue<Vector2>());
            cameraInputAsset.Player.Look.canceled += ctx => UpdateMouseOrbit(new Vector2(0f, 0f));
        }
        else
        {
            Debug.LogError("InputManagerAsset is not enabled!");
        }
    }

    void UpdateMovement(Vector2 mov)
    {
        //Debug.Log("Move:" + mov);
        h = mov.x;
        v = mov.y;
    }

    void UpdateMouseOrbit(Vector2 delta)
    {
        //Debug.Log("Look:" + delta);
        mouseX = delta.x;
        mouseY = delta.y;
    }

    void UpdatePositionAndRotationFromInput()
    {
        // Rotation
        mouseX = mouseX * horizontalSpeed;
        mouseY = mouseY * verticalSpeed;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        camObject.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);

        if (camObject != null)
        {
            // Position
            Vector3 movement = cam.transform.forward * v * movementSpeed + cam.transform.right * h * movementSpeed;
            movement.y = 0f;
            camObject.transform.position += movement;
        }
    }

    public void RestartCameraPosition()
    {
        camObject.transform.position = initialPosition;
        camObject.transform.eulerAngles = initialRotation;
    }

    // Test EoM public events
    public void WhenExciteOMeterStarts()
    {
        Debug.LogWarning("START LOG SESSION: Actions from the end user");
    }

    public void WhenExciteOMeterStops()
    {
        Debug.LogWarning("STOP LOG SESSION: Actions from the end user");
    }
}

#if UNITY_EDITOR
//-------------------------------------------------------------------------
[UnityEditor.CustomEditor(typeof(DesktopCameraController))]
public class CameraControllerEditor : UnityEditor.Editor
{
    //-------------------------------------------------
    // Custom Inspector GUI allows us to click from within the UI
    //-------------------------------------------------
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DesktopCameraController myScript = (DesktopCameraController)target;

        if (GUILayout.Button("RESET CAMERA"))
        {
            myScript.RestartCameraPosition();
        }
    }
}
#endif
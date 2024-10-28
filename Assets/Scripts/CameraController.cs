using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public GameObject desk;

    private bool enabled = true;

    [Header("Mouse Sensitivity Settings")]
    [SerializeField] private float mouseSensitivity = 100f;

    [Header("Clamping Settings")]
    [SerializeField] private float minVerticalAngle = -80f; // Limit for looking down
    [SerializeField] private float maxVerticalAngle = 80f;  // Limit for looking up

    private float xRotation = 0f;  // Tracks vertical rotation
    private float yRotation = 0f;  // Tracks horizontal rotation
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        // Hide and lock the cursor in the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (enabled)
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Clamp vertical rotation (up/down) based on the min and max vertical angles
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

            // Clamp horizontal rotation (left/right) between -90 and 90 degrees
            yRotation += mouseX;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);

            // Apply rotation to the camera (around x-axis for vertical, y-axis for horizontal)
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
        
    }

    private void OnApplicationFocus(bool focus)
    {
        transform.LookAt(desk.transform.position);
    }

    public void CameraAndCursorToggle(bool toggle)
    {
        enabled = toggle;
        if (enabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

}

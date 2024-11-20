using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Components
    public PlayerStateMachine stateMachine {  get; private set; }
    public Rigidbody rb {  get; private set; }
    private PlayerInputActions inputActions;
    public Camera FPCam { get; private set; }
    #endregion

    [Header("Movement Info")]
    public float moveSpeed;
    public float sensitivity;
    public float jumpHeight = 5f;

    [Header("Camera Info")]
    public float minAngle = -90f;
    public float maxAngle = 90f;
    public bool lockCamera = false;
    [Space]
    public float bobFrequency = 5f;
    public float bobAmplitute = 0.1f;
    public float bobHorizontalAmplitude = 0.05f;
    public float bobSmoothSpeed = 8f;
    public float bobbingTime = 0f;
    public Vector3 cameraStartPosition;
    public GameObject itemSlot;

    [Header("Interactables")]
    public float interactDistance = 10f;
    public LayerMask interactableLayers;
    public GameObject currentHeldItem;


    #region States

    public enum STATES
    {
        IDLE,
        MOVE,
        JUMP,
        AIR
    }

    public STATES CURRENT_STATE;

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    #endregion


    private void Awake()
    {
        
        stateMachine = new PlayerStateMachine();
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        FPCam = Camera.main;

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
        rb = GetComponent<Rigidbody>();
        LockMouse();
        cameraStartPosition = FPCam.transform.localPosition;
    }

    private void Update()
    {
        stateMachine.currentState.Update();

        CameraRotation();
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        Vector3 moveDir = (transform.forward * _yVelocity) + (transform.right * _xVelocity);
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);
    }

    public void GetCurrentHeldObject()
    {
        currentHeldItem = itemSlot.GetComponentInChildren<Grabbable>().gameObject;
        currentHeldItem.GetComponent<Grabbable>().DisableCollider();
    }

    #region InputHandling
    public Vector2 GetMovementDirNormalized()
    {
        
        return inputActions.Locomotion.Move.ReadValue<Vector2>().normalized;
    }

    public Vector2 GetMouseDelta()
    {
        
        return inputActions.Camera.Mouse.ReadValue<Vector2>();
    }

    public bool IsSprinting()
    {
        return inputActions.Locomotion.Sprint.IsPressed();
    }

    public bool IsJumping()
    {
        return inputActions.Locomotion.Jump.WasPressedThisFrame();
    }

    public bool IsInteracting()
    {
        return inputActions.Interaction.Interact.WasPressedThisFrame();
    }

    public bool DropItem()
    {
        return inputActions.Interaction.Drop.WasPressedThisFrame();
    }
    #endregion

    #region Camera Movement
    private float verticalRotation = 0;
        private void CameraRotation()
    {
        Vector2 mouseDelta = GetMouseDelta();
        float mouseX = mouseDelta.x * sensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * sensitivity * Time.deltaTime;

        // Rotate Player when looking left/right
        transform.Rotate(Vector3.up, mouseX);

        verticalRotation -= mouseY; // invert
        verticalRotation = Mathf.Clamp(verticalRotation, minAngle, maxAngle);

        FPCam.transform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }

    public void LockMouse()
    {
        lockCamera = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouseLockCamera()
    {
        lockCamera = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

}

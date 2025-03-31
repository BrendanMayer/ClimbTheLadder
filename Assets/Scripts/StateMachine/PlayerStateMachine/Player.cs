using System;
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
    public Transform computerSitPosition;
    public Transform computerCamPosition;
    #endregion

    [Header("Movement Info")]
    public float moveSpeed;
    public float sensitivity;
    public float jumpHeight = 5f;
    public bool hasPlayedStepSound = false;

    [Header("Camera Info")]
    
    public float minAngle = -90f;
    public float maxAngle = 90f;
    public bool lockCamera = false;
    public float cameraLerpSpeed = 5f;
    public float threshold = 0.01f;
    public GameObject playerCanvas;
    [Space]
    public float bobFrequency = 5f;
    public float bobAmplitute = 0.1f;
    public float bobHorizontalAmplitude = 0.05f;
    public float bobSmoothSpeed = 8f;
    public float bobbingTime = 0f;
    public Vector3 cameraStartPosition;
    public GameObject itemSlot;

    [Header("Interactables")]
    public float interactDistance = 2f;
    public LayerMask interactableLayers;
    private IInteractable currentInteractable = null;
    public bool isDragging = false;
    public LayerMask draggable;
    public HingeJoint currentDraggable;
    private Vector3 lastMousePosition;

    [Header("NPC Interaction")]
    
    public NPC currentTalkingToNPC;
    public bool talkingToNPC = false;
    public bool recording = false;


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
    public PlayerComputerState computerState { get; private set; }
    public PlayerInteractState interactState { get; private set; }
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
        computerState = new PlayerComputerState(this, stateMachine, "Computer");
        interactState = new PlayerInteractState(this, stateMachine, "Talk");
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

    public bool CloseNPCChat()
    {
        return inputActions.Interaction.CloseChat.WasPressedThisFrame();
    }

    public bool DropItem()
    {
        return inputActions.Interaction.Drop.WasPressedThisFrame();
    }

    public bool VoiceInput()
    {
            return inputActions.Interaction.Speech.WasPressedThisFrame();
 
        
    }

    public bool InventorySlot1()
    {
        return inputActions.InventoryMap.Slot1.WasPressedThisFrame();

    }

    public bool InventorySlot2()
    {
        return inputActions.InventoryMap.Slot2.WasPressedThisFrame();

    }

    public bool InventorySlot3()
    {
        return inputActions.InventoryMap.Slot3.WasPressedThisFrame();

    }

    public bool PressAnyKey()
    {
        return inputActions.AnyButton.Any.WasPressedThisFrame();
    }

    public bool HoldClick()
    {
        return inputActions.Interaction.Click.IsPressed();
    }
    #endregion

    #region Camera Movement
    private float verticalRotation = 0;
    

    private void CameraRotation()
    {
        if (!lockCamera)
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

    private void TrySelectDoor(RaycastHit hit)
    {
        
            HingeJoint hinge = hit.collider.GetComponent<HingeJoint>();

            if (hinge != null) // Ensure it's a door
            {
                
                currentDraggable = hinge; // Store the selected door
                currentDraggable.useMotor = true; // Ensure motor is enabled
                isDragging = true;
                lastMousePosition = Input.mousePosition; // Store mouse position for smooth dragging
            }
           
       
    }

    private void DragSelectedDoor()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // Use raw mouse movement
        float force = mouseDelta.x * -20f; // Only use horizontal movement

        if (currentDraggable != null)
        {
            JointMotor motor = currentDraggable.motor;
            motor.targetVelocity = force * 20f; // Adjust speed
            motor.force = 50f; // Ensure enough force is applied
            currentDraggable.motor = motor;
        }
    }

    public void PerformRaycast()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = FPCam.ScreenPointToRay(screenCenter);

        string tooltipToShow = "NONE"; // Default tooltip

        // Interactable Check
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayers))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable?.EnableOrDisableText(false); // Disable previous text
                    currentInteractable = interactable;
                    currentInteractable.EnableOrDisableText(true);
                }

                tooltipToShow = "INTERACT"; // Keep showing INTERACT while looking at it
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.EnableOrDisableText(false);
                currentInteractable = null;
            }
        }

        // Drag Check (only override NONE, not INTERACT)
        Ray dragRay = FPCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(dragRay, out RaycastHit dragHit, 2, draggable))
        {
            if (dragHit.collider != null && tooltipToShow == "NONE" && !HoldClick()) // Prevent overriding INTERACT
            {
                tooltipToShow = "DRAG";
            }
        }

        // Apply Tooltip at the End (prevents overwriting)
        

        // Handle Dragging
        if (Input.GetMouseButtonDown(0) && dragHit.collider != null)
        {
            
            TrySelectDoor(dragHit);
        }
        UIManager.instance.ShowTooltip(tooltipToShow);
        if (isDragging && currentDraggable != null)
        {
            DragSelectedDoor();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            if (currentDraggable != null)
            {
                JointMotor motor = currentDraggable.motor;
                currentDraggable.motor = motor;
                currentDraggable = null;
            }
        }

        lastMousePosition = Input.mousePosition;

        if (currentInteractable != null && IsInteracting())
        {
            currentInteractable.Interact();
        }
    }




    #endregion

}

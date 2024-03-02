using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Connection;
using FishNet.Object;

[RequireComponent(typeof(NetworkObject))]
public class PlayerMovement : NetworkBehaviour {
    #region Variables
    public float baseMoveSpeed = 7f, sprintModifier = 1.5f, moveSpeed;
    public float turnSpeed = 150f;
    public float jumpForce = 0.5f;
    public float gravityValue = -9.81f;
    public float groundCheckDistance = 0.1f;
    public float currentStamina = 0, maxStamina = 100;
    private float lastSprinted;
    public float staminaRecoveryRate = 10f, staminaDrainRate = 50f;
    private CharacterController controller;
    public Vector2 moveInput;
    public Vector2 turnInput;
    private Vector3 playerVelocity;
    private Vector3 moveDirection;
    public bool isGrounded;
    private Camera playerCamera;
    private bool isAutoRunning = false; // Flag for auto-run state
    private bool wasVerticalInput = false; // Flag to track if there was vertical input in the last frame
    public PlayerControls playerControls;
    public InputAction move;
    public InputAction turn;
    public InputAction jump;
    public InputAction sprint;
    public InputAction autoRun;
    public UIManager uiManager;
    #endregion

    // Ground check layer mask (optional)
    public LayerMask groundLayer;

    private void Awake() {
        playerControls = new PlayerControls();
    }

    public override void OnStartClient() {
        base.OnStartClient();
        if (base.IsOwner) {
            controller = GetComponent<CharacterController>();
            playerCamera = GetComponentInChildren<Camera>(true);
            groundLayer = 1;
            currentStamina = maxStamina;
            move = playerControls.Player.Move;
            move.Enable();
            turn = playerControls.Player.Turn;
            turn.Enable();
            jump = playerControls.Player.Jump;
            jump.Enable();
            sprint = playerControls.Player.Sprint;
            sprint.Enable();
            autoRun = playerControls.Player.AutoRun;
            autoRun.Enable();
            uiManager = FindObjectOfType<UIManager>();
        } else {
            enabled = false;
        }
    }

    private void OnDisable() {
        if (base.IsOwner) {
            move.Disable();
            turn.Disable();
            jump.Disable();
            sprint.Disable();
            autoRun.Disable();
        }
    }

    void Update() {
        if (!base.IsClientInitialized)
            return;

        if (ChatManager.instance.playerMessage.isFocused) {
            playerControls.Disable();
        } else {
            playerControls.Enable();
        }

        // Ground detection using raycasting
        isGrounded = CheckGrounded();
        // Toggle auto-run state when the "Auto Run" button is pressed
        if (autoRun.triggered) {
            isAutoRunning = !isAutoRunning; // Toggle auto-run
        }
        
        // If there's a "button down" event on vertical input, disable auto-run
        if (!wasVerticalInput && Mathf.Abs(move.ReadValue<Vector2>().normalized.y) != 0 && isAutoRunning) {
            isAutoRunning = false;
        }

        // Update the wasVerticalInput for the next frame
        wasVerticalInput = Mathf.Abs(move.ReadValue<Vector2>().normalized.y) != 0;

        // Horizontal rotation
        turnInput = turn.ReadValue<Vector2>();
        transform.Rotate(0f, (turnInput.x * turnSpeed * Time.deltaTime), 0f);

        // Mouse Rotation
        if (uiManager.ui.FindAction("RightClick").IsPressed()) { // Modify forward direction based on camera when right-clicking
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0; // Ignore vertical component
            cameraForward.Normalize();
            // Rotate player to face camera direction while right-clicking
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 100);
        }

        // Apply gravity
        if (isGrounded && playerVelocity.y <= 0) {
            playerVelocity.y = 0f;

            // Jumping
            if (jump.triggered && isGrounded) {
                playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravityValue);
                currentStamina = Mathf.Max((currentStamina - 20), 0);
            }

            if (uiManager.ui.FindAction("Click").IsPressed() && uiManager.ui.FindAction("RightClick").IsPressed()) { // Both mouse buttons held
                isAutoRunning = false;
                moveInput.y = 1;
                moveDirection = (transform.forward * moveInput.y);
            } else {
                // Handle auto-running and horizontal strafing
                if (isAutoRunning) {
                    moveInput = move.ReadValue<Vector2>();
                    moveInput.y = 1;
                    moveInput = moveInput.normalized;
                    moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized; // Combine forward and horizontal directions
                } else {
                    moveInput = move.ReadValue<Vector2>();
                    moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
                    if (moveInput.y < 0) {
                        moveDirection *= 0.5f; // Slow down if moving backward
                    }
                }
            }
            // Sprinting
            Sprint();
        } else {
            playerVelocity.y += gravityValue * Time.deltaTime;
            if (moveDirection.x == 0 && moveDirection.z == 0) {
                if (uiManager.ui.FindAction("Click").IsPressed() && uiManager.ui.FindAction("RightClick").IsPressed()) { // Both mouse buttons held
                    isAutoRunning = false;
                    moveInput.y = 1;
                    moveDirection = (transform.forward * moveInput.y);
                } else {
                    // Handle auto-running and horizontal strafing
                    if (isAutoRunning) {
                        moveInput = move.ReadValue<Vector2>();
                        moveInput.y = 1;
                        moveInput = moveInput.normalized;
                        moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized; // Combine forward and horizontal directions
                    } else {
                        moveInput = move.ReadValue<Vector2>();
                        moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
                    }
                }
                moveDirection *= 0.5f;
            }
        }

        // Apply movement including vertical velocity
        controller.Move(moveSpeed * Time.deltaTime * moveDirection + playerVelocity * Time.deltaTime);
    }
    private bool CheckGrounded() {
        float capsuleHeight = controller.height;
        float radius = controller.radius;
        Vector3 bottomCenter = transform.position + controller.center - new Vector3(0, capsuleHeight / 2, 0);

        // Check center and around the base of the capsule
        if (Physics.Raycast(bottomCenter, -Vector3.up, groundCheckDistance, groundLayer))
            return true;

        // Checking around the base in a circle
        int rayCount = 4; // Number of rays to cast around the base
        for (int i = 0; i < rayCount; i++) {
            Vector3 direction = Quaternion.Euler(0, 360 * i / rayCount, 0) * Vector3.forward;
            Vector3 start = bottomCenter + direction * radius;

            if (Physics.Raycast(start, -Vector3.up, groundCheckDistance, groundLayer))
                return true;
        }

        return false; // Not grounded
    }

    private void Sprint() {
        if (sprint.IsPressed() && (moveDirection.x != 0 || moveDirection.z != 0) && currentStamina > 0) {
            moveSpeed = baseMoveSpeed * sprintModifier;
            lastSprinted = Time.time;
            currentStamina = Mathf.Max(currentStamina - (staminaDrainRate * Time.deltaTime), 0);
        } else {
            moveSpeed = baseMoveSpeed;
            if (currentStamina > 0 || Time.time - lastSprinted >= 1f) {
                currentStamina = Mathf.Min(currentStamina + (staminaRecoveryRate * Time.deltaTime), maxStamina);
            }
        }
    }
}
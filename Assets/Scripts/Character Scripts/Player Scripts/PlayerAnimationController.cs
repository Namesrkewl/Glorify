using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class PlayerAnimationController : NetworkBehaviour {
    private Animator animator;
    private PlayerMovement playerMovement; // Reference to your movement script

    public override void OnStartClient() {
        base.OnStartClient();
        if (base.IsOwner) {
            // Get the Animator and PlayerMovement components
            animator = GetComponent<Animator>();
            playerMovement = transform.parent.GetComponent<PlayerMovement>();
        } else {
            enabled = false;
        }
    }

    void Awake() {
        
    }

    void Update() {
        if (!base.IsClientInitialized)
            return;

        // Update animation parameters
        //Debug.Log(playerMovement.horizontal);

        // Set Idle state when no movement input is detected
        if (playerMovement.moveInput.x == 0 && playerMovement.moveInput.y == 0) {
            foreach (AnimatorControllerParameter parameter in animator.parameters) {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                    animator.SetBool(parameter.name, false);
            }
        } else {
            // Update movement based animations
            animator.SetBool("Run Forward", true);
            SetMovementAnimation();
        }
    }

    void SetMovementAnimation() {
        switch (playerMovement.moveInput.x) {
            case (0):
                switch (playerMovement.moveInput.y) {
                    case (1):
                        // Moving Forward
                        animator.SetBool("Run Forward", true);
                        foreach (AnimatorControllerParameter parameter in animator.parameters) {
                            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name != "Run Forward")
                                animator.SetBool(parameter.name, false);
                        }
                        break;
                    case (-1):
                        // Moving Backward
                        animator.SetBool("Run Backward", true);
                        foreach (AnimatorControllerParameter parameter in animator.parameters) {
                            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name != "Run Backward")
                                animator.SetBool(parameter.name, false);
                        }
                        break;
                }
                break;
            case (1):
                switch (playerMovement.moveInput.y) {
                    case (0):
                        // Running Right
                        animator.SetBool("Run Right", true);
                        foreach (AnimatorControllerParameter parameter in animator.parameters) {
                            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name != "Run Right")
                                animator.SetBool(parameter.name, false);
                        }
                        break;
                    case (1):
                        break;
                    case (-1):
                        break;
                }
                break;
            case (-1):
                switch (playerMovement.moveInput.y) {
                    case (0):
                        // Running Left
                        animator.SetBool("Run Left", true);
                        foreach (AnimatorControllerParameter parameter in animator.parameters) {
                            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name != "Run Left")
                                animator.SetBool(parameter.name, false);
                        }
                        break;
                    case (1):
                        break;
                    case (-1):
                        break;
                }
                break;

        }
    }
}
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Component.Animating;

public class PlayerAnimation: NetworkBehaviour {
    private Animator animator;
    private NetworkAnimator networkAnimator;
    public PlayerMovement playerMovement; // Reference to your movement script

    public override void OnStartClient() {
        base.OnStartClient();
        if (base.IsOwner) {
            animator = GetComponentInChildren<Animator>();
            networkAnimator = GetComponentInChildren<NetworkAnimator>();
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

        SetMovementAnimation();
    }

    void SetMovementAnimation() {
        if (playerMovement.moveInput.y > 0) {
            animator.SetBool("Run Forward", true);
            animator.SetBool("Run Backward", false);
        } else if (playerMovement.moveInput.y < 0) {
            animator.SetBool("Run Backward", true);
            animator.SetBool("Run Forward", false);
        } else {
            animator.SetBool("Run Forward", false);
            animator.SetBool("Run Backward", false);
        }

        if (playerMovement.moveInput.x > 0) {
            animator.SetBool("Run Right", true);
            animator.SetBool("Run Left", false);
        } else if (playerMovement.moveInput.x < 0) {
            animator.SetBool("Run Left", true);
            animator.SetBool("Run Right", false);
        } else {
            animator.SetBool("Run Right", false);
            animator.SetBool("Run Left", false);
        }

        if (playerMovement.jump.triggered && playerMovement.isGrounded && !animator.GetBool("Run Forward") && !animator.GetBool("Run Backward") && !animator.GetBool("Run Left") && !animator.GetBool("Run Right")) {
            animator.SetTrigger("Jump");
            networkAnimator.SetTrigger("Jump");
            Debug.Log("Jumped!");
        }

        if (playerMovement.isGrounded) {
            animator.SetBool("isGrounded", true);
        } else {
            animator.SetBool("isGrounded", false);
        }
    }
}
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Component.Animating;

public class PlayerAnimation: NetworkBehaviour {
    public static PlayerAnimation instance;
    private Animator animator;
    private NetworkAnimator networkAnimator;

    public override void OnStartClient() {
        base.OnStartClient();
        if (base.IsOwner) {
            instance = this;
            animator = GetComponentInChildren<Animator>();
            networkAnimator = GetComponentInChildren<NetworkAnimator>();
        } else {
            enabled = false;
        }
    }

    void Awake() {
        
    }

    void Update() {
        if (!base.IsClientInitialized || !PlayerBehaviour.instance.isReady.Value || PlayerBehaviour.instance.player.Value.targetStatus == TargetStatus.Dead)
            return;

        SetMovementAnimation();
    }

    void SetMovementAnimation() {
        if (PlayerMovement.instance.moveInput.y > 0) {
            animator.SetBool("Run Forward", true);
            animator.SetBool("Run Backward", false);
        } else if (PlayerMovement.instance.moveInput.y < 0) {
            animator.SetBool("Run Backward", true);
            animator.SetBool("Run Forward", false);
        } else {
            animator.SetBool("Run Forward", false);
            animator.SetBool("Run Backward", false);
        }

        if (PlayerMovement.instance.moveInput.x > 0) {
            animator.SetBool("Run Right", true);
            animator.SetBool("Run Left", false);
        } else if (PlayerMovement.instance.moveInput.x < 0) {
            animator.SetBool("Run Left", true);
            animator.SetBool("Run Right", false);
        } else {
            animator.SetBool("Run Right", false);
            animator.SetBool("Run Left", false);
        }

        if (PlayerMovement.instance.jump.triggered && PlayerMovement.instance.isGrounded && !animator.GetBool("Run Forward") && !animator.GetBool("Run Backward") && !animator.GetBool("Run Left") && !animator.GetBool("Run Right")) {
            animator.SetTrigger("Jump");
            networkAnimator.SetTrigger("Jump");
            Debug.Log("Jumped!");
        }

        if (PlayerMovement.instance.isGrounded) {
            animator.SetBool("isGrounded", true);
        } else {
            animator.SetBool("isGrounded", false);
        }
    }
}
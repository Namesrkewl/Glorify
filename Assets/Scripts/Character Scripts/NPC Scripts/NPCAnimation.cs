using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Component.Animating;
using FishNet.Managing.Logging;

public class NPCAnimation : NetworkBehaviour {
    private NetworkAnimator networkAnimator;
    private Animator animator;
    private NPCBehaviour npcBehaviour;
    private Vector3 lastPosition;
    private float movementThreshold = 0.01f; // Threshold to determine if movement is significant


    [Server(Logging = LoggingType.Off)]
    public override void OnStartServer() {
        base.OnStartServer();
        npcBehaviour = GetComponent<NPCBehaviour>();
        animator = GetComponentInChildren<Animator>();
        networkAnimator = GetComponentInChildren<NetworkAnimator>();
    }

    [Server(Logging = LoggingType.Off)]
    void Update() {
        Debug.Log("Test");
        if (!base.IsServerInitialized)
            return;
        Debug.Log("Made");

        // Update animation parameters
        //Debug.Log(playerMovement.horizontal);

        SetMovementAnimation();
    }

    [Server(Logging = LoggingType.Off)]
    void SetMovementAnimation() {
        bool isMoving = false;


        // Check for NavMeshAgent-based movement
        if (npcBehaviour.agent != null && npcBehaviour.agent.velocity.magnitude > 0) {
            isMoving = true;
            Debug.Log("Agent");
        }
        // Check for direct transform manipulation movement
        else if (Vector3.Distance(transform.position, lastPosition) > movementThreshold) {
            isMoving = true;
            Debug.Log("Manual");
        } else {
            Debug.Log("Standing Here I Realize");
        }

        Debug.Log(isMoving);

        animator.SetBool("Run Forward", isMoving);

        // Update last position for next frame's comparison
        lastPosition = transform.position;
    }
}
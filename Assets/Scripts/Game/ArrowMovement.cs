using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ArrowMovement : MonoBehaviour {
    public float height = 0.5f; // The maximum height the arrow will move up before coming back down
    public float duration = 2f; // The duration it takes to complete a full cycle (up and back down)
    public float offset = 2f;

    private Vector3 startPosition;
    private float startTime;

    void Start() {
        startPosition = transform.position; // Store the original position of the arrow
        startTime = Time.time; // Store the start time
    }

    void Update() {
        // Calculate the current time within the cycle
        float time = (Time.time - startTime) / duration;

        // Use Mathf.PingPong to oscillate the position offset between 0 and the specified height
        float displacement = Mathf.PingPong(time * duration, height);

        // Apply the displacement to the arrow's original position
        transform.position = transform.parent.position + new Vector3(0, offset, 0) + startPosition + Vector3.up * displacement;
    }
}

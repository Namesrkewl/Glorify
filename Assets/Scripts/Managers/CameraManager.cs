using UnityEngine;
using System.Collections;
using FishNet.Connection;
using FishNet.Object;

public class CameraManager : NetworkBehaviour {

	public Camera thisCamera;
	public Transform target;
	private PlayerMovement playerMovement;
	public UIManager uiManager;

	public float targetHeight = 1.7f;
	public float distance = 5.0f;
	public float offsetFromWall = 0.1f;

	public float maxDistance = 20;
	public float minDistance = .6f;
	public float speedDistance = 5;

	public float xSpeed = 50.0f;
	public float ySpeed = 50.0f;

	public int yMinLimit = -40;
	public int yMaxLimit = 80;

	public int zoomRate = 40;

	public float rotationDampening = 3.0f;
	public float zoomDampening = 5.0f;

	public LayerMask collisionLayers = -1;

	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float currentDistance;
	private float desiredDistance;
	private float correctedDistance;

    private void Awake() {
		gameObject.SetActive(false);
    }

    public override void OnStartClient() {
		base.OnStartClient();
		if (base.IsOwner) {
			gameObject.SetActive(true);
			yDeg = 27f;
			xDeg = 0f;
			currentDistance = distance;
			desiredDistance = distance;
			correctedDistance = distance;
			playerMovement = GetComponentInParent<PlayerMovement>();
			uiManager = FindObjectOfType<UIManager>();
		} else {
			gameObject.SetActive(false);
		}
	}

	/**
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */
	void LateUpdate() {
		if (!base.IsClientInitialized)
			return;

		// Don't do anything if target is not defined
		if (!target)
			return;

		// If either mouse buttons are down, let the mouse govern camera position
		/*if (GUIUtility.hotControl == 0) {*/

		if (uiManager.ui.FindAction("Click").IsPressed() || uiManager.ui.FindAction("RightClick").IsPressed()) {
			xDeg += uiManager.ui.FindAction("MouseMove").ReadValue<Vector2>().x * xSpeed * 0.02f;
			yDeg -= uiManager.ui.FindAction("MouseMove").ReadValue<Vector2>().y * ySpeed * 0.02f;
		}

		// otherwise, ease behind the target if any of the directional keys are pressed
		else if (playerMovement.moveInput.y != 0 || playerMovement.moveInput.x != 0 || playerMovement.turnInput.x != 0) {
			float targetRotationAngle = target.eulerAngles.y;
			float currentRotationAngle = transform.eulerAngles.y;
			xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
		}
		//}


		// calculate the desired distance
		desiredDistance -= uiManager.ui.FindAction("ScrollWheel").ReadValue<Vector2>().normalized.y * Time.deltaTime * 10 * Mathf.Abs(desiredDistance) * speedDistance;
		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

		yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

		// set camera rotation
		Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);
		correctedDistance = desiredDistance;

		// calculate desired camera position
		Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance);

		// check for collision using the true target's desired registration point as set by user using height
		RaycastHit collisionHit;
		Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y, target.position.z);

		// if there was a collision, correct the camera position and calculate the corrected distance
		bool isCorrected = false;

		// Remove the ignored layers from the collisionLayers mask
		// This uses bitwise operations to clear the specific bit that corresponds to the ignore layer
		collisionLayers &= ~(1 << 6);
		collisionLayers &= ~(1 << 7);
		collisionLayers &= ~(1 << 8);

		if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers.value)) {
			// calculate the distance from the original estimated position to the collision location,
			// subtracting out a safety "offset" distance from the object we hit.  The offset will help
			// keep the camera from being right on top of the surface we hit, which usually shows up as
			// the surface geometry getting partially clipped by the camera's front clipping plane.
			correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
			isCorrected = true;
		}

		// For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
		currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

		// keep within legal limits
		currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

		// recalculate position based on the new currentDistance
		position = target.position - (rotation * Vector3.forward * currentDistance);

		transform.rotation = rotation;
		transform.position = position;
	}

	private static float ClampAngle(float angle, float min, float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}
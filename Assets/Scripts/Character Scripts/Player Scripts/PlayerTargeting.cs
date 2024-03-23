using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using GameKit.Dependencies.Utilities;

public class PlayerTargeting : NetworkBehaviour {
    public static PlayerTargeting instance = null;
    private List<GameObject> validTargets = new List<GameObject>();
    private int targetIndex = -1;
    public GameObject currentTarget;
    private const float MAX_TARGET_RANGE = 50.0f;
    private GameObject lastClickedTarget;
    private CameraManager cameraManager;
    public GameObject arrowPrefab; // Reference to the arrow prefab
    private static GameObject arrowInstance; // Instance of the arrow object
    public Material normalArrowMaterial;
    public Material autoAttackArrowMaterial;

    public override void OnStartClient() {
        base.OnStartClient();
        if (base.IsOwner) {
            instance = this;
            ClearTarget(true);
            cameraManager = GetComponentInChildren<CameraManager>(true);
        } else {
            GetComponent<PlayerTargeting>().enabled = false;
        }
    }

    void Update() {
        if (!base.IsClientInitialized || !PlayerBehaviour.instance.isReady.Value)
            return;
        UpdateValidTargets();
        HandleMouseInput();
        HandleTabTargeting();
        HandleCancelInput();
        CheckTargetDistance();
    }

    [ServerRpc(RequireOwnership = true)]
    private void UpdateCurrentTarget(GameObject _currentTarget) {
        GetComponent<PlayerBehaviour>().player.Value.currentTarget = _currentTarget;
    }

    private void HandleMouseInput() {
        if (Input.GetMouseButtonDown(0)) { // Left click
            SelectWithMouse(false);
        } else if (Input.GetMouseButtonDown(1)) { // Right click
            SelectWithMouse(true);
        }
    }

    private void SelectWithMouse(bool isRightClick) {
        Ray ray = cameraManager.thisCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200f)) {
            if (Vector3.Distance(transform.position, hit.transform.position) > MAX_TARGET_RANGE) {
                return; // Target is too far from the player
            }
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject != null) {
                SelectTarget(hitObject, true, isRightClick, true);
            }
        }
    }

    private void SelectTarget(GameObject targetObject, bool isClick, bool isRightClick, bool reset) {
        if (targetObject.IsDestroyed() || targetObject == null)
            return;

        if (targetObject.GetComponent<ITargetable>() != null) {
            if (currentTarget != targetObject) {
                ClearTarget(reset);
                currentTarget = targetObject;
                UpdateCurrentTarget(currentTarget);

                if (arrowPrefab != null) {
                    // Correctly create the rotation so the arrow faces the same y rotation as the target and points downwards (90 degrees on the x axis)
                    Quaternion arrowRotation = Quaternion.Euler(90, currentTarget.transform.eulerAngles.y, 0);
                    // Instantiate the arrow with the correct rotation
                    arrowInstance = Instantiate(arrowPrefab, Vector3.zero, arrowRotation, currentTarget.transform);
                    UpdateArrowMaterial(false); // Default to normal material when a new target is selected
                }

                if (isClick) {
                    lastClickedTarget = currentTarget;
                }
            }

            if (isRightClick) {
                ITargetable target = targetObject.GetComponent<ITargetable>();
                if (target.GetTargetStatus() == TargetStatus.Alive && (target.GetTargetType() == TargetType.Neutral || target.GetTargetType() == TargetType.Hostile)) {
                    UpdateArrowMaterial(true); // Change to auto-attack material
                    StartAutoAttack(targetObject);
                }
            }
        }
    }

    [ServerRpc]
    private void StartAutoAttack(GameObject targetObject, NetworkConnection sender = null) {
        Player player = GetComponent<PlayerBehaviour>().player.Value;
        player.actionState = ActionState.AutoAttacking;
        player.Sync();
    }

    public void StopAttack(NetworkConnection sender = null) {
        Player player = GetComponent<PlayerBehaviour>().player.Value;
        if (player.actionState == ActionState.AutoAttacking || player.actionState == ActionState.Casting) {
            player.actionState = ActionState.Idle;
            player.Sync();
            UpdateArrowMaterial(false);
        }
    }

    private void UpdateArrowMaterial(bool isAttacking) {
        if (arrowInstance != null) {
            Renderer arrowRenderer = arrowInstance.GetComponent<Renderer>();
            if (arrowRenderer != null) {
                arrowRenderer.material = isAttacking ? autoAttackArrowMaterial : normalArrowMaterial;
            }
        }
    }

    private void HandleTabTargeting() {
        if (Input.GetButtonDown("Target Enemy") && (validTargets.Count > 0)) {
            MoveLastClickedTargetToEnd();
            for (int i = 0; i < validTargets.Count; i++) {
                targetIndex = (targetIndex + 1) % validTargets.Count;
                GameObject newTarget = validTargets[targetIndex];
                if (newTarget != currentTarget && CheckTargetLineOfSight(newTarget)) {
                    StopAttack();
                    SelectTarget(newTarget, false, false, false);
                    break;
                }
            }
        }
    }

    private bool CheckTargetLineOfSight(GameObject target) {
        if (Vector3.Distance(transform.position, target.transform.position) > MAX_TARGET_RANGE) {
            return false; // Target is too far from the player
        }
        GameObject targetObject = target;
        Vector3 directionToTarget = targetObject.transform.position - cameraManager.transform.position;

        // Raycast to check for obstacles.
        RaycastHit[] hits;
        float rayDistance = 200f;
        hits = Physics.RaycastAll(cameraManager.transform.position, directionToTarget.normalized, rayDistance);

        // Sort hits by distance to ensure closest hit is checked first.
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits) {
            // Ignore the hit if it's this GameObject.
            if (hit.collider.gameObject == gameObject) continue;
            // Ignore the hit if it's the current target.
            if (hit.collider.gameObject == currentTarget) continue;
            // Check if the hit object is the target object.
            if (hit.collider.gameObject == targetObject) {  
                Debug.Log(hit.collider.name);
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    private void HandleCancelInput() {
        if (Input.GetButtonDown("Cancel")) {
            StopAttack();
            ClearTarget(true);
        }
    }

    // Modifications in ClearTarget to remove the arrow
    public void ClearTarget(bool reset) {
        if (arrowInstance != null) {
            Destroy(arrowInstance); // Destroy the arrow instance
            arrowInstance = null;
        }
        currentTarget = null;
        UpdateCurrentTarget(currentTarget);
        if (reset) {
            targetIndex = -1; // Reset the target index
        }
    }

    private void MoveLastClickedTargetToEnd() {
        if (lastClickedTarget != null && validTargets.Contains(lastClickedTarget)) {
            validTargets.Remove(lastClickedTarget);
            validTargets.Add(lastClickedTarget);
            lastClickedTarget = null;
        }
    }

    private void UpdateValidTargets() {
        List<GameObject> potentialTargets = new List<GameObject>();
        // Calculate the camera's frustum planes.
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraManager.thisCamera);
        // Find all colliders within a sphere centered at the camera's position, up to MAX_TARGET_RANGE.
        Collider[] collidersInView = Physics.OverlapSphere(cameraManager.thisCamera.transform.position, MAX_TARGET_RANGE + Vector3.Distance(transform.position, cameraManager.transform.position));

        foreach (Collider collider in collidersInView) {
            // Check if the collider's bounds are within the camera's view frustum.
            if (collider)
            if (!GeometryUtility.TestPlanesAABB(planes, collider.bounds)) continue; // Skip if collider is not in camera view.

            ITargetable target = collider.GetComponent<ITargetable>();
            if (target == null) continue; // Skip if the object does not implement ITargetable.
            if (target.GetTargetObject() == gameObject) continue;
            potentialTargets.Add(target.GetTargetObject());
        }
        if (potentialTargets.Count > 0) {
            ConfirmValidTargets(potentialTargets);
        } else {
            validTargets = new List<GameObject>();
        }
    }

    public void ConfirmValidTargets(List<GameObject> targetObjects) {
        List<GameObject> confirmedTargets = new List<GameObject>();
        foreach (GameObject targetObject in targetObjects) {
            if (targetObject.IsDestroyed() || targetObject == null) continue;
            ITargetable target = targetObject.GetComponent<ITargetable>();
            if (target.GetTarget() == null) continue;
            if (target.GetTargetStatus() == TargetStatus.Alive && (target.GetTargetType() == TargetType.Neutral || target.GetTargetType() == TargetType.Hostile)) {
                confirmedTargets.Add(targetObject);
            }
        }
        if (confirmedTargets.Count > 0) {
            AddValidTargets(confirmedTargets);
        }
    }

    private void AddValidTargets(List<GameObject> targetObjects) {
        List<GameObject> _validTargets = new List<GameObject>();
        foreach (GameObject targetObject in targetObjects) {
            if (targetObject.IsDestroyed())
                continue;
            _validTargets.Add(targetObject);
        }
        if (_validTargets.Count > 0) {
            validTargets = _validTargets;
            SortTargetsByCameraDirection();
        }
        
    }

    private void SortTargetsByCameraDirection() {
        validTargets = validTargets.OrderBy(t =>
            Vector3.Angle(cameraManager.thisCamera.transform.forward, t.transform.position - cameraManager.thisCamera.transform.position)
        ).ToList();
    }

    private bool IsWithinCameraView(Transform targetTransform) {
        Vector3 viewportPosition = cameraManager.thisCamera.WorldToViewportPoint(targetTransform.position);
        return viewportPosition.z > 0 && viewportPosition.x > 0 && viewportPosition.x < 1 && viewportPosition.y > 0 && viewportPosition.y < 1;
    }

    public GameObject GetCurrentTarget() {
        if (currentTarget != null && !currentTarget.IsDestroyed()) {
            return currentTarget;
        } else {
            //return playerBehaviour.playerData.Value;
            return null;
        }
    }

    private void CheckTargetDistance() {
        if (currentTarget != null) {
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > MAX_TARGET_RANGE) {
                ClearTarget(true);
            }
        }
    }

}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

public class PlayerTargeting : NetworkBehaviour {
    private List<GameObject> validTargets = new List<GameObject>();
    private int targetIndex = -1;
    private GameObject currentTarget;
    public Material highlightMaterial;
    public Material autoAttackHighlightMaterial;
    private Material originalMaterial;
    private Renderer targetRenderer;
    private const float MAX_TARGET_RANGE = 50.0f;
    private GameObject lastClickedTarget;
    private CameraManager cameraManager;
    private PlayerBehaviour playerBehaviour;
    public bool IsAutoAttacking { get; private set; }

    public override void OnStartClient() {
        base.OnStartClient();
        if (base.IsOwner) {
            playerBehaviour = GetComponent<PlayerBehaviour>();
            cameraManager = GetComponentInChildren<CameraManager>(true);
        } else {
            GetComponent<PlayerTargeting>().enabled = false;
        }
    }

    void Update() {
        if (!base.IsClientInitialized)
            return;
        UpdateValidTargets();
        HandleMouseInput();
        HandleTabTargeting();
        HandleCancelInput();
        CheckTargetDistance();
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
        if (Physics.Raycast(ray, out hit, MAX_TARGET_RANGE + Vector3.Distance(transform.position, cameraManager.transform.position))) {
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
                targetRenderer = targetObject.GetComponent<Renderer>();
                originalMaterial = targetRenderer.material;
                UpdateHighlight(false);
                if (isClick) {
                    lastClickedTarget = currentTarget;
                }
            }

            if (isRightClick) {
                ConfirmValidTarget(targetObject);
            }
        }
    }

    [ServerRpc]
    private void ConfirmValidTarget(GameObject targetObject,  NetworkConnection sender = null) {
        ITargetable target = targetObject.GetComponent<ITargetable>();
        if (target.GetTargetStatus() == TargetStatus.Alive && (target.GetTargetType() == TargetType.Neutral || target.GetTargetType() == TargetType.Hostile)) {
            AttackTarget(sender);
        }
    }

    [TargetRpc]
    private void AttackTarget(NetworkConnection receiver) {
        StartAutoAttack();
    }


    private void HandleTabTargeting() {
        if (Input.GetButtonDown("Target Enemy") && (validTargets.Count > 0)) {
            MoveLastClickedTargetToEnd();
            for (int i = 0; i < validTargets.Count; i++) {
                targetIndex = (targetIndex + 1) % validTargets.Count;
                GameObject newTarget = validTargets[targetIndex];
                if (newTarget != currentTarget && CheckTargetLineOfSight(newTarget)) {
                    StopAutoAttack();
                    SelectTarget(newTarget, false, false, false);
                    break;
                }
            }
        }
    }

    private bool CheckTargetLineOfSight(GameObject target) {
        GameObject targetObject = target;
        Vector3 directionToTarget = targetObject.transform.position - cameraManager.transform.position;

        // Raycast to check for obstacles.
        RaycastHit[] hits;
        float rayDistance = MAX_TARGET_RANGE + Vector3.Distance(transform.position, cameraManager.transform.position);
        hits = Physics.RaycastAll(cameraManager.transform.position, directionToTarget.normalized, rayDistance);
        Debug.DrawRay(cameraManager.transform.position, directionToTarget.normalized * rayDistance, Color.red, 2.0f);

        // Sort hits by distance to ensure closest hit is checked first.
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits) {
            // Ignore the hit if it's this GameObject.
            if (hit.collider.gameObject == gameObject) continue;
            // Check if the hit object is the target object.
            if (hit.collider.gameObject == targetObject) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    public void StartAutoAttack() {
        IsAutoAttacking = true;
        UpdateHighlight(true);
        // Possibly trigger some event or call in PlayerBehaviour
    }

    public void StopAutoAttack() {
        if (IsAutoAttacking) {
            IsAutoAttacking = false;
            UpdateHighlight(false);
            // Possibly trigger some event or call in PlayerBehaviour
        }
    }

    private void HandleCancelInput() {
        if (Input.GetButtonDown("Cancel")) {
            if (IsAutoAttacking) {
                StopAutoAttack();
            } else {
                ClearTarget(true);
            }
        }
    }

    private void ClearTarget(bool reset) {
        if (targetRenderer != null) {
            // Revert the material to its original
            targetRenderer.material = originalMaterial;
            targetRenderer = null;
        }
        currentTarget = null;
        IsAutoAttacking = false;
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

    private void UpdateHighlight(bool isAttacking) {
        if (currentTarget != null && !currentTarget.IsDestroyed()) {
            targetRenderer = currentTarget.GetComponent<Renderer>();
            if (targetRenderer != null) {
                targetRenderer.material = isAttacking ? autoAttackHighlightMaterial : highlightMaterial;
            }
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

    [ServerRpc]
    public void ConfirmValidTargets(List<GameObject> targetObjects, NetworkConnection sender = null) {
        Debug.Log("Got To Confirmation");
        List<GameObject> confirmedTargets = new List<GameObject>();
        foreach (GameObject targetObject in targetObjects) {
            if (targetObject.IsDestroyed())
                continue;
            ITargetable target = targetObject.GetComponent<ITargetable>();
            if (target as PlayerBehaviour != null && target.GetKey().name == null)
                continue;
            if (target.GetTargetStatus() == TargetStatus.Alive && (target.GetTargetType() == TargetType.Neutral || target.GetTargetType() == TargetType.Hostile)) {
                confirmedTargets.Add(targetObject);
            }
        }
        if (confirmedTargets.Count > 0) {
            AddValidTargets(sender, confirmedTargets);
        }
    }

    [TargetRpc]
    private void AddValidTargets(NetworkConnection receiver, List<GameObject> targetObjects) {
        Debug.Log("Got To Add");
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
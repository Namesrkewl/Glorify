using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class PlayerTargeting : NetworkBehaviour {
    private List<ITargetable> validTargets = new List<ITargetable>();
    private int targetIndex = -1;
    private ITargetable currentTarget;
    public Material highlightMaterial;
    public Material autoAttackHighlightMaterial;
    private Material originalMaterial;
    private Renderer targetRenderer;
    private const float MAX_TARGET_RANGE = 50.0f;
    private ITargetable lastClickedTarget;
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
        /*
        if (!base.IsClientInitialized)
            return;
        UpdateValidTargets();
        HandleMouseInput();
        HandleTabTargeting();
        HandleCancelInput();
        CheckTargetDistance();
        */
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
        Debug.Log("Clicked!");
        if (Physics.Raycast(ray, out hit, MAX_TARGET_RANGE + Vector3.Distance(transform.position, cameraManager.transform.position))) {
            GameObject hitObject = hit.collider.gameObject;
            Debug.Log(hitObject);
            if (hitObject != null) {
                SelectTarget(hitObject, true, isRightClick, true);
            }
        }
    }

    private void SelectTarget(GameObject targetObject, bool isClick, bool isRightClick, bool reset) {
        ITargetable target = targetObject.GetComponent<ITargetable>();
        Debug.Log(target);
        if (target != null) {
            if (currentTarget != target) {
                ClearTarget(reset);
                currentTarget = target;
                targetRenderer = targetObject.GetComponent<Renderer>();
                originalMaterial = targetRenderer.material;
                UpdateHighlight(false);
                if (isClick) {
                    lastClickedTarget = target;
                }
                //Debug.Log("Currently Targeting: " + currentTargetGameObject.transform.parent.name);
            }

            /*
            if (isRightClick && (npcBehaviour.npcData.status == NPC.NPCStatus.Hostile || npcBehaviour.npcData.status == NPC.NPCStatus.Neutral)) {
                StartAutoAttack();
            }
            */
        }
    }

    private void HandleTabTargeting() {
        if (Input.GetButtonDown("Target Enemy") && (validTargets.Count > 0)) {
            MoveLastClickedTargetToEnd();
            foreach (var target in validTargets)
                Debug.Log(target.GetTargetObject().transform.parent.name);
            targetIndex = (targetIndex + 1) % validTargets.Count;
            ITargetable newTarget = validTargets[targetIndex];
            if (newTarget != currentTarget) {
                StopAutoAttack();
                SelectTarget(newTarget.GetTargetObject(), false, false, false);
            } else if (validTargets.Count > 1) {
                targetIndex = (targetIndex + 1) % validTargets.Count;
                newTarget = validTargets[targetIndex];
                StopAutoAttack();
                SelectTarget(newTarget.GetTargetObject(), false, false, false);
            }
        }
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
        if (currentTarget != null) {
            targetRenderer = currentTarget.GetTargetObject().GetComponent<Renderer>();
            if (targetRenderer != null) {
                targetRenderer.material = isAttacking ? autoAttackHighlightMaterial : highlightMaterial;
            }
        }
    }

    private void UpdateValidTargets() {
        validTargets.Clear();
        foreach (var target in FindObjectsOfType<MonoBehaviour>().OfType<ITargetable>()) {
            GameObject targetObject = target.GetTargetObject();
            if (IsWithinCameraView(targetObject.transform) && (target.GetTargetStatus() == TargetStatus.Alive) &&
                Vector3.Distance(transform.position, targetObject.transform.position) <= MAX_TARGET_RANGE &&
                (target.GetTargetType() == TargetType.Neutral || target.GetTargetType() == TargetType.Hostile)) {
                validTargets.Add(target);
            }
        }
        SortTargetsByCameraDirection();
    }

    private void SortTargetsByCameraDirection() {
        validTargets = validTargets.OrderBy(t =>
            Vector3.Angle(cameraManager.thisCamera.transform.forward, t.GetTargetObject().transform.position - cameraManager.thisCamera.transform.position)
        ).ToList();
    }

    private bool IsWithinCameraView(Transform targetTransform) {
        Vector3 viewportPosition = cameraManager.thisCamera.WorldToViewportPoint(targetTransform.position);
        return viewportPosition.z > 0 && viewportPosition.x > 0 && viewportPosition.x < 1 && viewportPosition.y > 0 && viewportPosition.y < 1;
    }

    public ITargetable GetCurrentTarget() {
        if (currentTarget != null) {
            return currentTarget;
        } else {
            //return playerBehaviour.playerData.Value;
            return null;
        }
    }

    private void CheckTargetDistance() {
        if (currentTarget != null) {
            if (Vector3.Distance(transform.position, currentTarget.GetTargetObject().transform.position) > MAX_TARGET_RANGE) {
                ClearTarget(true);
            }
        }
    }

}
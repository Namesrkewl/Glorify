using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class InformationText : FloatingText
{
    public Color playerColor = Color.blue;
    public Color friendlyColor = Color.green;
    public Color neutralColor = Color.yellow;
    public Color hostileColor = Color.red;
    public Color deadColor = Color.gray;
    private Transform targetTransform;
    private Character targetCharacter;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Update() {
        if (PlayerBehaviour.instance != null && targetTransform != null && targetCharacter != null) {
            if (Vector3.Distance(PlayerBehaviour.instance.transform.position, targetTransform.position) > 50f) {
                textMesh.text = null;
            } else {
                UpdateInformationText();
            }
        }
    }

    public static void CreateInformationText(Character receiver) {
        if (receiver != null) {
            GameObject newInformationTextObj = Instantiate(Resources.Load<GameObject>("Prefabs/Floating Text/Text/InformationText"), FloatingTextManager.instance.InformationTextContainer.transform);
            InformationText newInformationText = newInformationTextObj.GetComponent<InformationText>();
            newInformationText.targetTransform = receiver.networkObject.transform;
            newInformationText.targetCharacter = receiver;
            receiver.SetInformationText(newInformationText);
            if (Vector3.Distance(PlayerBehaviour.instance.transform.position, newInformationText.targetTransform.position) > 50f) {
                newInformationText.textMesh.text = null;
            } else {
                newInformationText.UpdateInformationText();
            }
        }
    }

    private void UpdateInformationText() {
        if (targetCharacter != null && targetTransform != null && targetTransform.gameObject.activeSelf != false) {
            if (targetTransform != null || targetTransform.gameObject.activeSelf != false) {
                upwardOffset = targetTransform.GetComponent<Collider>().bounds.size;
                upwardOffset.x = 0;
                upwardOffset.z = 0;
                Vector3 textPosition = targetTransform.position + upwardOffset;
                Vector3 screenPosition = CameraManager.instance.thisCamera.WorldToScreenPoint(textPosition);
                if (screenPosition.z < 0) {
                    textMesh.name = null;
                    return;
                }
                transform.position = screenPosition;
            }
            textMesh.text = targetCharacter.name;
            if (targetCharacter.targetStatus == TargetStatus.Dead || targetCharacter.targetStatus == TargetStatus.Object) {
                textMesh.color = deadColor;
            } else if (targetCharacter.targetType == TargetType.Ally || targetCharacter.targetType == TargetType.Companion || targetCharacter.targetType == TargetType.Friendly) {
                textMesh.color = friendlyColor;
            } else if (targetCharacter.targetType == TargetType.Neutral) {
                textMesh.color = neutralColor;
            } else if (targetCharacter.targetType == TargetType.Hostile) {
                textMesh.color = hostileColor;
            } else if (targetCharacter.targetType == TargetType.Player) {
                textMesh.color = playerColor;
            }
        } else {
            Destroy(gameObject);
        }
    }
}

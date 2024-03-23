using FishNet.Connection;
using FishNet.Example.ColliderRollbacks;
using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CombatText : FloatingText {
    public Color highRollColor = Color.red;
    public Color criticalHitColor = Color.yellow;
    private Transform targetTransform = null;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Update() {
        if (targetTransform != null && !targetTransform.gameObject.IsDestroyed() && targetTransform.gameObject.activeSelf != false) {
            Vector3 screenPosition = CameraManager.instance.thisCamera.WorldToScreenPoint(targetTransform.position);
            if (screenPosition.z < 0 ) {
                textMesh.name = null;
                return;
            }
            transform.position = screenPosition + upwardOffset;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetColor(Color color) {
        textMesh.color = color;
    }

    public static void CreateDamageText(Character receiver, string message) {
        if (receiver != null) {
            GameObject newCombatTextObj = Instantiate(Resources.Load<GameObject>("Prefabs/Floating Text/Text/CombatText"), FloatingTextManager.instance.CombatTextContainer.transform);
            CombatText newCombatText = newCombatTextObj.GetComponent<CombatText>();
            newCombatText.targetTransform = receiver.networkObject.transform;

            if (newCombatText.targetTransform != null && !newCombatText.targetTransform.gameObject.IsDestroyed() && newCombatText.targetTransform.gameObject.activeSelf != false) {
                Vector3 screenPosition = CameraManager.instance.thisCamera.WorldToScreenPoint(newCombatText.targetTransform.position);
                if (screenPosition.z < 0) {
                    newCombatText.textMesh.name = null;
                    return;
                }
                newCombatText.transform.position = screenPosition + newCombatText.upwardOffset;
            } else {
                Destroy(newCombatText.gameObject);
                return;
            }


            // Set the text and initial position
            newCombatText.Initialize(message);

            // Set color based on the message
            newCombatText.SetColor(message.StartsWith("-") ? Color.white : Color.green);

            // Start the floating and fading behaviour
            newCombatText.FloatAndFade();
        }
    }

    public static void CreateStatusText(Character receiver, string message) {
        if (receiver != null) {
            GameObject newCombatTextObj = Instantiate(Resources.Load<GameObject>("Prefabs/Floating Text/Text/CombatText"), FloatingTextManager.instance.CombatTextContainer.transform);
            CombatText newCombatText = newCombatTextObj.GetComponent<CombatText>();
            newCombatText.targetTransform = receiver.networkObject.transform;

            // Set the text and initial position
            newCombatText.Initialize(message);

            // Set color based on the message
            newCombatText.SetColor(Color.cyan);

            // Start the floating and fading behaviour
            newCombatText.FloatAndFade();
        }
    }

    public void FloatAndFade() {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut() {
        float duration = 1.0f; // Duration of the effect
        Vector3 startOffset = Vector3.zero;
        Vector3 endOffset = Vector3.up * 60f; // Adjust the upward movement distance as needed
        float startTime = Time.time;
        while (Time.time - startTime < duration) {
            float elapsed = Time.time - startTime;
            upwardOffset = Vector3.Lerp(startOffset, endOffset, elapsed / duration);
            //Debug.Log(upwardOffset);

            // Fading logic
            // Update the alpha based on elapsed time
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1.0f - (elapsed / duration));

            yield return null;
        }

        Destroy(gameObject);
    }

    // Override any additional methods if needed for special behaviours
}
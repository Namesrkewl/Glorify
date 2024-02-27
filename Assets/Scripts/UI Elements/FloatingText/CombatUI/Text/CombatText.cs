using System.Collections;
using UnityEngine;

public class CombatText : FloatingText {
    public Color highRollColor = Color.green;
    public Color criticalHitColor = Color.yellow;
    public bool isHighRoll;
    public bool isCriticalHit;

    protected override void Awake() {
        base.Awake();
        /*
        // Set special colors/effects based on combat text type
        if (isHighRoll) {
            textMesh.color = highRollColor;
            // Add additional high roll effects if needed
        } else if (isCriticalHit) {
            textMesh.color = criticalHitColor;
            // Add additional critical hit effects if needed
        }*/
    }

    public void SetColor(Color color) {
        textMesh.color = color;
    }

    public void FloatAndFade() {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut() {
        float duration = 1.0f; // Duration of the effect
        Vector3 startOffset = Vector3.zero;
        Vector3 endOffset = Vector3.up * 0.6f; // Adjust the upward movement distance as needed
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
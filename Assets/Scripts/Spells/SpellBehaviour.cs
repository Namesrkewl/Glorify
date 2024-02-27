using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBehaviour : MonoBehaviour {

    public Spell currentSpell;

    /*

    #region Spell Logic

    

    [Server(Logging = LoggingType.Off)]
    private IEnumerator SpellLogic(Player player, Spell spell, EntityBehaviour targetBehavior, bool canAggro) {
        if (spell.needsLineOfSight && targetBehavior != this) {
            Vector3 targetDirection = targetBehavior.transform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.forward, targetDirection.normalized);

            if (dotProduct > 0) { // Target is in front of the player
                RaycastHit hit;
                // Check if there is an obstacle between the player and the target
                if (Physics.Raycast(transform.position, targetDirection.normalized, out hit, targetDirection.magnitude)) {
                    if (hit.collider.transform != targetBehavior.transform) {
                        // Hit something else before the target, so LOS is obstructed
                        yield break; // Exit the coroutine early
                    }
                }
            } else {
                // Target is not in front of the player
                yield break; // Exit the coroutine early
            }
        }
        currentSpell = spell;
        yield return StartCoroutine(uiManager.ShowCastBar(spell));
        if (targetBehavior != null && targetBehavior.characterData.currentHealth > 0 && canAggro) {
            NPCBehaviour targetBehaviorAsNPC = targetBehavior as NPCBehaviour;
            if (targetBehaviorAsNPC != null) {
                //if (targetBehaviorAsNPC)
                //EnterCombatWith(player, targetBehavior); // Enter combat with the target
            }
        }
        //yield return StartCoroutine(spell.Cast(this, targetBehavior));
        StopCasting(player);
        yield return null;
    }

    [Server(Logging = LoggingType.Off)]
    public void PerformSpell(Player player, Spell spell, bool canAggro) {
        StopCasting(player);
        StartCoroutine(SpellLogic(player, spell, targetBehavior, canAggro));
    }

    [Server(Logging = LoggingType.Off)]
    public void StopCasting(Player player) {
        currentSpell = null;
        StopAllCoroutines();
        uiManager.HideCastBar();
        uiManager.StopAllCoroutines();
        LeanTween.cancel(uiManager.gameObject);
    }

    [Server(Logging = LoggingType.Off)]
    public void UpdateSpellDescriptions(Player player) {
        foreach (var spell in player.Spells) {
            spell.UpdateDescription();
        }
        foreach (var passive in player.Passives) {
            passive.UpdateDescription();
        }
    }

    #endregion
    */
}

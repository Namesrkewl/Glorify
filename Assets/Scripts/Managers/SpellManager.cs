using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Logging;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager instance;

    private void Awake() {
        instance = this;
    }

    //UpdateSpellDescriptions(player);

    /*
        if (currentSpell != null) {
            if (playerMovement.moveInput.x != 0 || playerMovement.moveInput.y != 0) {
                StopCasting(player);
            }
        }*/
    /*
    [Server(Logging = LoggingType.Off)]
    public void HandleSpell(Player player, ITargetable target, Spell spell) {
        if (player.currentMana >= spell.manaCost) {
            GameObject currentTargetGameObject = null;
            EntityBehaviour targetBehavior = null;
            if (currentTargetGameObject != null) {
                targetBehavior = currentTargetGameObject.GetComponent<EntityBehaviour>();
            }
            if (spell.targetType == Spell.TargetType.Enemy) {
                NPCBehaviour targetBehaviorAsNPC = targetBehavior as NPCBehaviour;
                if (targetBehaviorAsNPC != null) {
                    if ((targetBehaviorAsNPC.npcData.status == NPC.NPCStatus.Hostile || targetBehaviorAsNPC.npcData.status == NPC.NPCStatus.Neutral) && targetBehaviorAsNPC.state != State.Dead) {
                        PerformSpell(player, spell, targetBehaviorAsNPC, true);
                    } else {
                        Debug.Log("Invalid Target");
                    }
                } else {
                    Debug.Log("Invalid Target");
                }
            } else if (spell.targetType == Spell.TargetType.Ally) {
                NPCBehaviour targetBehaviorAsNPC = targetBehavior as NPCBehaviour;
                if (targetBehaviorAsNPC != null) {
                    if (targetBehaviorAsNPC.npcData.status == NPC.NPCStatus.Friendly) {
                        PerformSpell(player, spell, targetBehaviorAsNPC, false);
                    } else {
                        PerformSpell(player, spell, this, false);
                    }
                } else {
                    PerformSpell(player, spell, this, false);
                }
            } else if (spell.targetType == Spell.TargetType.Aura || spell.targetType == Spell.TargetType.Self) {
                PerformSpell(player, spell, this, false);
            } else if (spell.targetType == Spell.TargetType.Any) {
                if (targetBehavior != null && targetBehavior.state != State.Dead) {
                    PerformSpell(player, spell, targetBehavior, false);
                } else {
                    PerformSpell(player, spell, this, false);
                }
            } else if (spell.targetType == Spell.TargetType.Area) {
                Debug.Log("Not Implemented Yet!");
            }

        }
    }
    */
}

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Healing Word", menuName = "Spell/Priest/Healing Word")]
public class HealingWord : Spell {
    /*
    public override IEnumerator Cast(PlayerBehaviour playerBehaviour, EntityBehaviour targetBehaviour) {
        CombatManager.instance.SendDamage(targetBehaviour, damage);
        //playerBehaviour.playerData.Value.currentMana -= manaCost;
        yield return null;
    } */

    public override void UpdateDescription() {
        description = $"Heals the target for {damage} health.";
    }
}

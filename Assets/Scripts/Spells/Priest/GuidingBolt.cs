using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Guiding Bolt", menuName = "Spell/Priest/Guiding Bolt")]
public class GuidingBolt : Spell {
    /*
    public override IEnumerator Cast(PlayerBehaviour playerBehaviour, EntityBehaviour targetBehaviour) {
        if (targetBehaviour.npc.currentHealth > 0) {
            CombatManager.instance.SendDamage(targetBehaviour, damage);
            //playerBehaviour.playerData.Value.currentMana -= manaCost;
        }
        yield return null;
    } */

    public override void UpdateDescription() {
        description = $"The target takes {damage} damage.";
    }
}

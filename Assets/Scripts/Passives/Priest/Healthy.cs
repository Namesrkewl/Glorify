using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Healthy", menuName = "Passive/Priest/Healthy")]
public class Healthy : Spell {
    public float healthIncreasePercentage = 0.1f; // 10% increase
    /*
    public override IEnumerator Cast(ICastable _caster, ICastable _target) {
        Identifiable caster = Database.instance.GetTarget(_caster as ITargetable);
        Identifiable target = Database.instance.GetTarget(_target as ITargetable);
        int healthIncrease = (int)(targetBehaviour.npc.maxHealth * healthIncreasePercentage);
        targetBehaviour.npc.maxHealth += healthIncrease;
        targetBehaviour.npc.currentHealth += healthIncrease;
        yield return null;
        
    }*/

    public override void RemoveEffect(Character character) {
        //Player player = character as Player;
        Player player = new Player();
        int healthDecrease = (int)(player.maxHealth / healthIncreasePercentage);
        player.maxHealth -= healthDecrease;
        player.currentHealth -= healthDecrease;
    }
}

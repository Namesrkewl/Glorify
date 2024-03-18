using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Healthy", menuName = "Passive/Priest/Healthy")]
public class Healthy : Spell {
    public float healthIncreasePercentage = 0.1f; // 10% increase

    public override void Cast(ITargetable _caster, ITargetable _target) {
        base.Cast(_caster, _target);
        int healthIncrease = (int)(target.maxHealth * healthIncreasePercentage);
        target.maxHealth += healthIncrease;
        target.currentHealth += healthIncrease;        
    }

    public override void RemoveEffect(Character character) {
        int healthDecrease = (int)(character.maxHealth / healthIncreasePercentage);
        character.maxHealth -= healthDecrease;
        character.currentHealth -= healthDecrease;
    }
}

using UnityEngine;

public class GuidingBolt : Spell {

    public override void Cast(Character caster, Character target) {
        if (target.currentHealth > 0) {
            CombatManager.instance.Damage(target, damage);
            caster.currentMana -= manaCost;
        }
    }

    public override void UpdateDescription() {
        description = $"The target takes {damage} damage.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Healing Word", menuName = "Spell/Priest/Healing Word")]
public class HealingWord : Spell {
    public override void Cast(Character caster, Character target) {
        CombatManager.instance.Damage(target, damage);
        caster.currentMana -= manaCost;
    }

    public override void UpdateDescription() {
        description = $"Heals the target for {damage} health.";
    }
}

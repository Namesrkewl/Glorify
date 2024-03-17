using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Healing Word", menuName = "Spell/Priest/Healing Word")]
public class HealingWord : Spell {
    public override void Cast(ITargetable _caster, ITargetable _target) {
        base.Cast(_caster, _target);
        CombatManager.instance.SendDamage(_target, damage);
        caster.SetCurrentMana(caster.GetCurrentMana() - manaCost);
    }

    public override void UpdateDescription() {
        description = $"Heals the target for {damage} health.";
    }
}

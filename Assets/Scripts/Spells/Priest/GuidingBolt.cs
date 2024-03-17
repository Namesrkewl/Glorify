using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Guiding Bolt", menuName = "Spell/Priest/Guiding Bolt")]
public class GuidingBolt : Spell {

    public override void Cast(ITargetable _caster, ITargetable _target) {
        base.Cast(_caster, _target);
        if (target.GetCurrentHealth() > 0) {
            CombatManager.instance.SendDamage(_target, damage);
            caster.SetCurrentMana(caster.GetCurrentMana() - manaCost);
        }
    }

    public override void UpdateDescription() {
        description = $"The target takes {damage} damage.";
    }
}

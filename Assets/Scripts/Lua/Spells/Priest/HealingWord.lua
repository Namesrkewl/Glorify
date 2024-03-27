function Cast(caster, target)
    caster.currentMana = caster.currentMana - spell.manaCost
    combatManager.Heal(target, spell.damage + caster.intelligence, caster)
    caster.Sync()
end

function UpdateDescription()
    spell.description = "Heals the target for", spell.damage, "health.";
end
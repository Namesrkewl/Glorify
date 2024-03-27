function Cast(caster, target)
    if target == nil or target.currentHealth > 0 then
        caster.currentMana = caster.currentMana - spell.manaCost
        combatManager.Damage(target, spell.damage + caster.intelligence, caster)
        caster.Sync()
    else
        log("Invalid Target.")
    end
end

function UpdateDescription()
    description = "The target takes", spell.damage, "damage.";
end
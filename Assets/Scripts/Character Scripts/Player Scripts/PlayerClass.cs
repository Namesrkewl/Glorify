using System.Collections.Generic;
using UnityEngine;

public enum Classes {
    Alchemist,
    Enchanter,
    Hunter,
    Mage,
    Monk,
    Paladin,
    Priest,
    Rogue,
    Unspecialized,
    Warrior,
    Weaponsmith
}

public class PlayerClass : ScriptableObject {

    public List<Spell> Passives;
    public List<Spell> Spells;

    public void UpdateClass() {
        SetSpells();
        SetPassives();
    }

    public virtual void SetSpells() {
        // Base method for setting spell info on the derived classes
    }

    public virtual void SetPassives() {
        // Base method for setting passive info on the derived classes
    }

    /*
    void ApplyPassives(PlayerBehaviour playerBehaviour) {
        // Apply passive abilities
        foreach (var passive in playerBehaviour.playerData.Value.Passives) {
            if (playerBehaviour.playerData.Value.level >= passive.levelRequired) {
                playerBehaviour.PerformSpell(passive, playerBehaviour, false);
            }
        }
    }

    public void SetClass(PlayerBehaviour playerBehaviour) {
        // New lists of passives and spells from the class
        HashSet<Spell> newPassives = new HashSet<Spell>(Passives);
        HashSet<Spell> newSpells = new HashSet<Spell>(Spells);

        // Reference to player's current passives and spells
        var currentPassives = new HashSet<Spell>(playerBehaviour.playerData.Value.Passives);
        var currentSpells = new HashSet<Spell>(playerBehaviour.playerData.Value.Spells);

        // Remove passives that are not in the new list (no longer valid)
        foreach (var passive in currentPassives) {
            if (!newPassives.Contains(passive)) {
                RemovePassive(playerBehaviour, passive);
            }
        }

        // Apply valid passives that are not already present
        foreach (var passive in newPassives) {
            if (!currentPassives.Contains(passive)) {
                ApplyPassive(playerBehaviour, passive);
            }
        }

        // Update the player's passives to the new set
        playerBehaviour.playerData.Value.Passives = new List<Spell>(newPassives);

        // Similar logic applied to Spells
        foreach (var spell in currentSpells) {
            if (!newSpells.Contains(spell)) {
                RemoveSpell(playerBehaviour, spell);
            }
        }

        foreach (var spell in newSpells) {
            if (!currentSpells.Contains(spell)) {
                // If there's any effect to apply when adding a spell, apply it here
                // Note: This is where you'd reapply the spell if needed
            }
        }

        // Update the player's spells to the new set
        playerBehaviour.playerData.Value.Spells = new List<Spell>(newSpells);
    }


    private void RemoveSpell(PlayerBehaviour playerBehaviour, Spell spell) {
        // Logic to remove spell effects, if any
        playerBehaviour.playerData.Value.Spells.Remove(spell);
    }

    private void ApplyPassive(PlayerBehaviour playerBehaviour, Spell passive) {
        // Apply the passive's effect
        playerBehaviour.PerformSpell(passive, playerBehaviour, false);
    }

    private void RemovePassive(PlayerBehaviour playerBehaviour, Spell passive) {
        // Precisely reverse the passive's effect
        //passive.RemoveEffect(playerBehaviour.playerData.Value);
        playerBehaviour.playerData.Value.Passives.Remove(passive);
    }
    */
}
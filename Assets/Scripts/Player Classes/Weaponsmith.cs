using UnityEngine;

[CreateAssetMenu(fileName = "Weaponsmith", menuName = "Player Class/Weaponsmith")]
public class Weaponsmith : PlayerClass
{
    public override void SetPassives() {
        // Load all the passive scriptable objects in the Weaponsmith folder as passives
        Spell[] loadedPassives = Resources.LoadAll<Spell>("Passives/Weaponsmith");

        // Clear the existing passives and add new ones from the loaded resources
        Passives.Clear();
        foreach (var passive in loadedPassives) {
            Passives.Add(passive);
        }
    }

    public override void SetSpells() {
        // Load all the spell scriptable objects in the Weaponsmith folder as spells
        Spell[] loadedSpells = Resources.LoadAll<Spell>("Spells/Weaponsmith");

        // Clear the existing spells and add new ones from the loaded resources
        Spells.Clear();
        foreach (var spell in loadedSpells) {
            Spells.Add(spell);
        }
    }
}

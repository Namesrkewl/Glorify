using UnityEngine;

[CreateAssetMenu(fileName = "Paladin", menuName = "Player Class/Paladin")]
public class Paladin : PlayerClass
{
    public override void SetPassives() {
        // Load all the passive scriptable objects in the Paladin folder as passives
        Spell[] loadedPassives = Resources.LoadAll<Spell>("Passives/Paladin");

        // Clear the existing passives and add new ones from the loaded resources
        Passives.Clear();
        foreach (var passive in loadedPassives) {
            Passives.Add(passive);
        }
    }

    public override void SetSpells() {
        // Load all the spell scriptable objects in the Paladin folder as spells
        Spell[] loadedSpells = Resources.LoadAll<Spell>("Spells/Paladin");

        // Clear the existing spells and add new ones from the loaded resources
        Spells.Clear();
        foreach (var spell in loadedSpells) {
            Spells.Add(spell);
        }
    }
}

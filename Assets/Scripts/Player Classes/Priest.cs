using UnityEngine;

[CreateAssetMenu(fileName = "Priest", menuName = "Player Class/Priest")]
public class Priest : PlayerClass {

    public override void SetPassives() {
        // Load all the passive scriptable objects in the Priest folder as passives
        Spell[] loadedPassives = Resources.LoadAll<Spell>("Passives/Priest");

        // Clear the existing passives and add new ones from the loaded resources
        Passives.Clear();
        foreach (var passive in loadedPassives) {
            Passives.Add(passive);
        }
    }

    public override void SetSpells() {
        // Load all the spell scriptable objects in the Priest folder as spells
        Spell[] loadedSpells = Resources.LoadAll<Spell>("Spells/Priest");

        // Clear the existing spells and add new ones from the loaded resources
        Spells.Clear();
        foreach (var spell in loadedSpells) {
            Spells.Add(spell);
        }
    }
}

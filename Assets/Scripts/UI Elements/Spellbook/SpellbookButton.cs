using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellbookButton : ActionButton
{
    public TextMeshProUGUI spellName;
    public TextMeshProUGUI spellTag;

    protected override void Update() {
        base.Update();
        UpdateSpellName();
    }

    private void UpdateSpellName() {
        if (assignedAction != null) {
            spellName.text = assignedAction.name;
            if ((assignedAction as Spell).type == Spell.SpellType.Passive) {
                spellTag.gameObject.SetActive(true);
                spellTag.text = "    Passive";
            } else {
                spellTag.gameObject.SetActive(false);
            }
        }
    }
}

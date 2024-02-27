using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationText : FloatingText
{
    public Color friendlyColor = Color.blue;
    public Color neutralColor = Color.yellow;
    public Color hostileColor = Color.red;
    public Color deadColor = Color.gray;
    public EntityBehaviour entityBehaviour;

    protected override void Awake() {
        base.Awake();
    }

    public override void Initialize(EntityBehaviour entity) {
        entityBehaviour = entity;
    }

    protected override void Update() {
        if (entityBehaviour as EntityBehaviour) {
            EntityBehaviour characterBehavior = entityBehaviour as EntityBehaviour;
            /*
            if (characterBehavior != null) {
                Player characterAsPlayer = characterBehavior.characterData as Player;
                NPC characterAsNPC = characterBehavior.characterData as NPC;
                textMesh.text = characterBehavior.characterData.name;
                if (characterBehavior.state == EntityBehaviour.State.Dead) {
                    textMesh.color = deadColor;
                } else if (characterAsPlayer != null || (characterAsNPC != null && characterAsNPC.status == NPC.NPCStatus.Friendly)) {
                    textMesh.color = friendlyColor;
                } else if (characterAsNPC != null && (characterAsNPC.status == NPC.NPCStatus.NeutralFriendly || characterAsNPC.status == NPC.NPCStatus.Neutral)) {
                    textMesh.color = neutralColor;
                } else if (characterAsNPC != null && characterAsNPC.status == NPC.NPCStatus.Hostile) {
                    textMesh.color = hostileColor;
                }
            }
            */
        }
    }

    public void SetColor(Color color) {
        textMesh.color = color;
    }

    // Override any additional methods if needed for special behaviours
}

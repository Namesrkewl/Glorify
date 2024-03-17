using FishNet.Demo.AdditiveScenes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InformationText : FloatingText
{
    public Color friendlyColor = Color.blue;
    public Color neutralColor = Color.yellow;
    public Color hostileColor = Color.red;
    public Color deadColor = Color.gray;

    protected override void Awake() {
        base.Awake();
    }

    public override void Initialize(ITargetable entity) {
        Identifiable entityData = Database.instance.GetTarget(entity);
        if (entityData as Player != null) {
            Player player = entityData as Player;
            textMesh.text = player.GetName();
            if (player.GetTargetStatus() == TargetStatus.Dead) {
                textMesh.color = deadColor;
            } else if (player.GetTargetType() == TargetType.Player) {
                textMesh.color = friendlyColor;
            }
        } else {
            NPC npc = entityData as NPC;
            textMesh.text = npc.GetName();
            if (npc.GetTargetStatus() == TargetStatus.Dead || npc.GetTargetStatus() == TargetStatus.Object) {
                textMesh.color = deadColor;
            } else if (npc.GetTargetType() == TargetType.Ally || npc.GetTargetType() == TargetType.Companion || npc.GetTargetType() == TargetType.Friendly) {
                textMesh.color = friendlyColor;
            } else if (npc.GetTargetType() == TargetType.Neutral) {
                textMesh.color = neutralColor;
            } else if (npc.GetTargetType() == TargetType.Hostile) {
                textMesh.color = hostileColor;
            }
        }
    }
    protected override void Update() {
    }

    public void SetColor(Color color) {
        textMesh.color = color;
    }

    // Override any additional methods if needed for special behaviours
}

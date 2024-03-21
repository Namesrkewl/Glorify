using UnityEngine;

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
            textMesh.text = player.name;
            if (player.targetStatus == TargetStatus.Dead) {
                textMesh.color = deadColor;
            } else if (player.targetType == TargetType.Player) {
                textMesh.color = friendlyColor;
            }
        } else {
            NPC npc = entityData as NPC;
            textMesh.text = npc.name;
            if (npc.targetStatus == TargetStatus.Dead || npc.targetStatus == TargetStatus.Object) {
                textMesh.color = deadColor;
            } else if (npc.targetType == TargetType.Ally || npc.targetType == TargetType.Companion || npc.targetType == TargetType.Friendly) {
                textMesh.color = friendlyColor;
            } else if (npc.targetType == TargetType.Neutral) {
                textMesh.color = neutralColor;
            } else if (npc.targetType == TargetType.Hostile) {
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

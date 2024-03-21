using UnityEngine;

public interface ITargetable {
    Key GetKey();
    Character GetTarget();

    ITargetable GetTargetComponent();

    TargetType GetTargetType();

    TargetStatus GetTargetStatus();

    GameObject GetTargetObject();
}

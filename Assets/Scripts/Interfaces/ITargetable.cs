using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable {
    Key GetKey();
    Character GetTarget();

    ITargetable GetTargetComponent();

    TargetType GetTargetType();

    TargetStatus GetTargetStatus();

    GameObject GetTargetObject();
}

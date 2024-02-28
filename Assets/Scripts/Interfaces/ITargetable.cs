using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable {
    int GetID();
    Identifiable GetTarget();

    ITargetable GetTargetComponent();

    TargetType GetTargetType();

    TargetStatus GetTargetStatus();

    GameObject GetTargetObject();
}

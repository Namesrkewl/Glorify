using UnityEngine;

public interface ICombatable : ITargetable
{
    void EnterCombat(GameObject target);
    void ExitCombat(GameObject target);
    void ExitAllCombat();
}

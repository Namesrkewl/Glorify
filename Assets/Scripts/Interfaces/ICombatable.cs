using UnityEngine;

public interface ICombatable : ITargetable
{
    void ServerEnterCombat(GameObject target);
    void ServerExitCombat(GameObject target);
    void ExitAllCombat();
}

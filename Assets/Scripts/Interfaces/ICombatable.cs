
using FishNet.Connection;
using FishNet.Object;

public interface ICombatable : ITargetable
{
    void ServerEnterCombat(NetworkBehaviour target);
    void ServerExitCombat(NetworkBehaviour target);
    void ExitAllCombat();
}

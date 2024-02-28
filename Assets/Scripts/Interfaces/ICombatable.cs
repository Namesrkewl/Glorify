
public interface ICombatable : ITargetable
{
    void EnterCombat(ICombatable target);
    void ExitCombat(ICombatable target);
}

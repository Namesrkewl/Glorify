using UnityEngine;

public class PlayerAction : ScriptableObject
{
    public new string name;
    public string description;
    [System.NonSerialized] public Sprite icon;
    public virtual void ExecuteAction() {
        
    }
    public virtual void ExecuteAction(PlayerBehaviour playerBehaviour) {

    }
}

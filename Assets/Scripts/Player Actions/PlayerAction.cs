using UnityEngine;

public class PlayerAction : ScriptableObject {
    public string description;
    [System.NonSerialized] public Sprite icon;
    public virtual void ExecuteAction() {
        
    }
    public virtual void ExecuteAction(PlayerBehaviour playerBehaviour) {

    }
}

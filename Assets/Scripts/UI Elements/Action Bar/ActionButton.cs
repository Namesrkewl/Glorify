using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet.Object;
using FishNet.Connection;

public class ActionButton : MonoBehaviour {
    public PlayerAction assignedAction;
    public Image image;
    protected virtual void Awake() {
        image = transform.GetChild(2).GetComponent<Image>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateActionSprite();
    }

    public void AssignAction(PlayerAction action) {
        assignedAction = action;
    }

    public void ExecuteAssignedAction() {
        PlayerBehaviour playerBehaviour = FindObjectOfType<PlayerBehaviour>();
        if (assignedAction != null) {
            assignedAction.ExecuteAction(playerBehaviour);
        }
    }

    protected virtual void UpdateActionSprite() {
        if (assignedAction != null) {
            if (assignedAction.icon != null) {
                image.enabled = true;
                image.sprite = assignedAction.icon;
            } else {
                image.enabled = false;
            }
        } else {
            image.enabled = false;
        }
    }

}

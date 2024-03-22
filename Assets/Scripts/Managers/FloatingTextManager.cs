using UnityEngine;
using System.Collections.Generic;
using FishNet.Object;
using System.Linq;

public class FloatingTextManager : MonoBehaviour {
    public static FloatingTextManager instance;
    public GameObject CombatTextContainer;
    public GameObject InformationTextContainer;
    public GameObject NameplateContainer;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            ClearAllContainers();
        }
    }

    private void ClearAllContainers() {
        foreach (Transform child in CombatTextContainer.transform) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in InformationTextContainer.transform) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in NameplateContainer.transform) {
            Destroy(child.gameObject);
        }
    }
}
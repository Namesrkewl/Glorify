using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager instance;
    public TextMeshProUGUI conversation;
    public new TextMeshProUGUI name;
    public Dialogue currentDialogue;
    public DialogueTree currentTree;
    public bool talking = false;
    public GameObject window;
    public GameObject optionPrefab;
    public GameObject optionContainer;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            window.SetActive(false);
        }
    }

    private void Update() {
        if (window.activeSelf && currentTree != null && currentDialogue != null) {
            if (Input.GetMouseButtonDown(0)) {
                CallNextDialogue();
            }
        }
    }

    public void ShowDialogue(DialogueTree tree, int index = 0) {
        ClearOptions();
        window.SetActive(true);
        currentTree = tree;
        currentDialogue = tree.dialogue[index];
        name.text = tree.name;
        conversation.text = currentDialogue.content;
        talking = true;
        if (currentDialogue.branches.Count > 1) {
            ShowDialogueOptions();
        }
    }

    public void ShowDialogue(Dialogue dialogue) {
        ClearOptions();
        currentDialogue = dialogue;
        conversation.text = currentDialogue.content;
        if (currentDialogue.branches.Count > 1) {
            ShowDialogueOptions();
        }
    }

    private void ShowDialogueOptions() {
        ClearOptions();
        foreach (int branch in currentDialogue.branches) {
            GameObject option = Instantiate(optionPrefab, optionContainer.transform);
            DialogueButton dialogueButton = option.GetComponent<DialogueButton>();
            dialogueButton.dialogue = currentTree.dialogue[branch];
            dialogueButton.textMesh.text = dialogueButton.dialogue.title;
        }
    }

    private void CallNextDialogue() {
        if (currentDialogue.branches.Count == 0) {
            // Logic for last piece of dialogue
            CloseDialogue();
        } else if (currentDialogue.branches.Count == 1) {
            // Logic for a sequential piece of dialogue
            ShowDialogue(currentTree.dialogue[currentDialogue.branches[0]]);
        }
    }

    public void CloseDialogue() {
        ClearOptions();
        conversation.text = null;
        currentDialogue = null;
        currentTree = null;
        talking = false;
        window.SetActive(false);
    }

    private void ClearOptions() {
        foreach (Transform option in optionContainer.transform) {
            Destroy(option.gameObject);
        }
    }
}

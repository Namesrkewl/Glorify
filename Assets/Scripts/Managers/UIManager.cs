using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMesh Pro namespace
using System;
using System.Collections;
using UnityEngine.InputSystem;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using FishNet.Managing.Logging;
using UnityEngine.Rendering.PostProcessing;

public class UIManager : NetworkBehaviour {

    public static UIManager instance;

    [Header("UI Elements")]
    public TMP_Text characterNameText;
    public Image characterPortrait;
    public Slider healthBar;
    public TMP_Text healthPercentageText;
    public TMP_Text healthNumericText;
    public Slider manaBar;
    public TMP_Text manaPercentageText;
    public TMP_Text manaNumericText;
    public Slider staminaBar;
    public TMP_Text staminaPercentageText;
    public TMP_Text staminaNumericText;
    public Slider experienceBar;
    public TMP_Text experiencePercentageText;
    public TMP_Text experienceNumericText;
    public Transform buffContainer;
    public Transform debuffContainer;
    public Slider castBar;
    public TMP_Text spellNameText;
    public TMP_Text levelText;
    public TMP_Text classText;


    [Header("Combat Info")]
    public GameObject combatInfoPrefab; // Prefab for displaying combat information
    public PlayerMovement playerMovement;
    public PlayerTargeting playerTargeting;
    public PlayerControls playerControls;
    public InputActionMap ui;

    private void Awake() {
        instance = this;
        playerControls = new PlayerControls();
        LeanTween.init(1000); // Initializing LeanTween with a larger amount of tweening spaces
    }

    private void OnEnable() {
        ui = playerControls.UI;
        ui.Enable();
    }

    private void OnDisable() {
        ui.Disable();
    }

    public override void OnStartClient() {
        base.OnStartClient();
    }

    public void UpdatePlayerInformation(Player player) {
        characterNameText.text = player.name;
        UpdateHealthBar(player);
        UpdateManaBar(player);
        UpdateStaminaBar(100);
        UpdateExperienceBar(player);
        UpdateLevel(player);
        UpdateClass(player);
        UpdateBuffsAndDebuffs(player);
        UpdateCurrentTarget(player);
    }

    private void UpdateHealthBar(Player player) {
        if (player.maxHealth == 0) {
            return;
        }
        float newHealthPercentage = player.currentHealth / player.maxHealth;
        healthBar.value = newHealthPercentage;
        healthPercentageText.text = Mathf.RoundToInt(newHealthPercentage * 100) + "%";
        healthNumericText.text = $"{(int)player.currentHealth}/{(int)player.maxHealth}";
    }

    private void UpdateManaBar(Player player) {
        if (player.maxMana == 0) {
            return;
        }
        float newManaPercentage = player.currentMana / player.maxMana;
        manaBar.value = newManaPercentage;
        manaPercentageText.text = Mathf.RoundToInt(newManaPercentage * 100) + "%";
        manaNumericText.text = $"{(int)player.currentMana}/{(int)player.maxMana}";
    }

    private void UpdateStaminaBar(float newStaminaPercentage) {
        if (newStaminaPercentage >= 1) {
            staminaBar.gameObject.SetActive(false);
        } else {
            staminaBar.gameObject.SetActive(true);
            staminaBar.value = newStaminaPercentage;
            staminaPercentageText.text = Mathf.RoundToInt(newStaminaPercentage * 100) + "%";
            staminaNumericText.text = $"{(int)playerMovement.currentStamina}/{(int)playerMovement.maxStamina}";
        }
    }

    private void UpdateExperienceBar(Player player) {
        if (player.maxExperience == 0) {
            return;
        }
        float newExperiencePercentage = player.currentExperience / player.maxExperience;
        experienceBar.value = newExperiencePercentage;
        experiencePercentageText.text = Mathf.RoundToInt(newExperiencePercentage * 100) + "%";
        experienceNumericText.text = $"{(int)player.currentExperience}/{(int)player.maxExperience}";
    }

    private void UpdateLevel(Player player) {
        levelText.text = $"Level {player.level}";
    }

    private void UpdateClass(Player player) {
        classText.text = $"{player.classEnum}";
    }

    private void UpdateBuffsAndDebuffs(Player player) {
        // Implement logic to update buffs and debuffs
        // You might need to iterate through the player's active status effects
    }

    private void UpdateCurrentTarget(Player player) {
        
    }

    public void DisplayCombatInfo(string info, Color color) {
        GameObject combatInfo = Instantiate(combatInfoPrefab, characterPortrait.transform);
        TMP_Text infoText = combatInfo.GetComponent<TMP_Text>();
        infoText.text = info;
        infoText.color = color;

        // Implement additional positioning or animation logic for combat info
    }
    
    public IEnumerator ShowCastBar(Spell spell) {
        Debug.Log("Called Show Cast Bar");
        castBar.gameObject.SetActive(true);
        spellNameText.text = spell.name;
        LeanTween.value(gameObject, UpdateCastBar, 0, 1, spell.castTime);
        yield return new WaitForSeconds(spell.castTime);
        HideCastBar();
    }

    private void UpdateCastBar(float value) {
        castBar.value = value;
    }

    public void HideCastBar() {
        castBar.gameObject.SetActive(false);
    }
}
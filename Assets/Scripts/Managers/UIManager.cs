using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMesh Pro namespace
using System;
using System.Collections;
using UnityEngine.InputSystem;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class UIManager : NetworkBehaviour {

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

    private Player playerData;
    private PlayerBehaviour playerBehaviour;
    private PlayerMovement playerMovement;
    public PlayerControls playerControls;
    public InputActionMap ui;

    private void Awake() {
        playerControls = new PlayerControls();
        LeanTween.init(1000); // Initializing LeanTween with a larger amount of tweening spaces
    }

    public void SetPlayer(PlayerBehaviour _playerBehaviour) {
        playerBehaviour = _playerBehaviour;
        //playerData = playerBehaviour.playerData.Value; // Reference to the player data
        playerMovement = playerBehaviour.GetComponent<PlayerMovement>();
    }

    private void OnEnable() {
        ui = playerControls.UI;
        ui.Enable();
    }

    private void OnDisable() {
        ui.Disable();
    }

    void Update() {
        if (base.IsClientInitialized)
            UpdateUI(); // Continuously update UI elements
    }

    private void UpdateUI() {
        //characterNameText.text = playerData.name;
        // Update character portrait if applicable
        // characterPortrait.sprite = ...;

        //UpdatePlayerInformation();
    }
    
    private void UpdatePlayerInformation() {
        UpdateHealthBar(playerData.currentHealth / playerData.maxHealth);
        UpdateManaBar(playerData.currentMana / playerData.maxMana);
        UpdateStaminaBar(playerMovement.currentStamina / playerMovement.maxStamina);
        UpdateExperienceBar(playerData.currentExperience / playerData.maxExperience);
        UpdateLevel();
        UpdateClass();
        UpdateBuffsAndDebuffs();
    }

    private void UpdateHealthBar(float newHealthPercentage) {
        healthBar.value = newHealthPercentage;
        healthPercentageText.text = Mathf.RoundToInt(newHealthPercentage * 100) + "%";
        healthNumericText.text = $"{(int)playerData.currentHealth}/{(int)playerData.maxHealth}";
    }

    private void UpdateManaBar(float newManaPercentage) {
        manaBar.value = newManaPercentage;
        manaPercentageText.text = Mathf.RoundToInt(newManaPercentage * 100) + "%";
        manaNumericText.text = $"{(int)playerData.currentMana}/{(int)playerData.maxMana}";
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

    private void UpdateExperienceBar(float newExperiencePercentage) {
        experienceBar.value = newExperiencePercentage;
        experiencePercentageText.text = Mathf.RoundToInt(newExperiencePercentage * 100) + "%";
        experienceNumericText.text = $"{(int)playerData.currentExperience}/{(int)playerData.maxExperience}";
    }

    private void UpdateLevel() {
        levelText.text = $"Level {playerData.level}";
    }

    private void UpdateClass() {
        classText.text = $"{playerData.playerClass.playerClass}";
    }

    private void UpdateBuffsAndDebuffs() {
        // Implement logic to update buffs and debuffs
        // You might need to iterate through the player's active status effects
    }

    public void DisplayCombatInfo(string info, Color color) {
        GameObject combatInfo = Instantiate(combatInfoPrefab, characterPortrait.transform);
        TMP_Text infoText = combatInfo.GetComponent<TMP_Text>();
        infoText.text = info;
        infoText.color = color;

        // Implement additional positioning or animation logic for combat info
    }
    
    /*
    public void UpdateHealth(int newHealth) {
        float newHealthPercentage = newHealth / (float)playerData.maxHealth;
        LeanTween.value(gameObject, healthBar.value, newHealthPercentage, 0.5f)
            .setOnUpdate((float val) => UpdateHealthBar(val));
    }

    
    public void UpdateMana(int newMana) {
        float newManaPercentage = newMana / (float)playerData.maxMana;
        LeanTween.value(gameObject, manaBar.value, newManaPercentage, 0.5f)
            .setOnUpdate((float val) => UpdateManaBar(val));
    }
    */

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
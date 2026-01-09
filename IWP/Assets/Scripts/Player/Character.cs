using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Manages each Player character's in-game stats & input handling
/// </summary>
public class Character : MonoBehaviour
{
    #region Stats
    [SerializeField] private CharacterKlass klass;

    [Header("Current Stats")]
    [SerializeField] private int currHealth;       
    [SerializeField] private int currAP;       
    [SerializeField] private int currStrength;
    [SerializeField] private int currMagic;
    [SerializeField] private int currEndurance;
    [SerializeField] private int currSpeed;
    [SerializeField] private float currCrit;

    // Orignal Stats
    [Header("Original Stats")]
    [SerializeField] private int originalHealth;
    [SerializeField] private int originalAP;
    [SerializeField] private int originalStrength;
    [SerializeField] private int originalMagic;
    [SerializeField] private int originalEndurance;
    [SerializeField] private int originalSpeed;
    [SerializeField] private float originalCrit;

    // Stat Buff Turn Counter
    [SerializeField] private int strengthBuffTimer;
    [SerializeField] private int magicBuffTimer;
    [SerializeField] private int enduranceBuffTimer;
    [SerializeField] private int speedBuffTimer;
    [SerializeField] private int critBuffTimer;

    // Stat Nerf Turn Counter
    [SerializeField] private int strengthNerfTimer;
    [SerializeField] private int magicNerfTimer;
    [SerializeField] private int enduranceNerfTimer;
    [SerializeField] private int speedNerfTimer;
    [SerializeField] private int critNerfTimer;
    #endregion

    #region Aktion
    [SerializeField] private Aktion unique1;
    [SerializeField] private Aktion unique2;
    [SerializeField] private List<Aktion> listOfAktions;
    public List<Aktion> GetAktionList() { return listOfAktions; }
    public Aktion GetAktion(int i) { return listOfAktions[i]; }
    #endregion

    #region Enhancements
    [SerializeField] private List<Enhancement> enhancementsList;

    public List<Enhancement> GetEnhancementsList() { return enhancementsList; }
    public void AddEnhancement(Enhancement e) {  enhancementsList.Add(e); }
    #endregion

    #region Inputs
    [SerializeField] private InputActionAsset playerActionAsset;
    private InputActionMap playerActionMap;
    private InputAction confirmAction;
    private bool isConfirmPressed;
    public bool IsConfirmPressed() { return  isConfirmPressed; }

    private InputAction hitAction;
    private bool isHitPressed;
    public bool IsHitPressed() { return isHitPressed; }

    private InputAction navigateAction;
    public Vector2 GetNavigateInput() { return navigateAction.ReadValue<Vector2>(); }
    #endregion

    #region Animator
    [SerializeField] private Animator playerAnimator;

    public void SetAnimator(Animator animator) { playerAnimator = animator; }
    public Animator GetAnimator() { return playerAnimator; }

    public void PlayAnimation(string animationName)
    {
        playerAnimator.CrossFade(animationName, 0.2f);
    }

    #endregion

    [SerializeField] private List<string> turnUpdateTexts = new List<string>();
    public List<string> GetTurnUpdateLists() { return turnUpdateTexts; }
    [SerializeField] public CanvasManager canvasManager { private set; get; }
    [SerializeField] public  AktionManager aktionManager { private set; get; }

    public void OnStart(int playerNo, CanvasManager cm, AktionManager am)
    {
        #region Stats
        // Simpan original values dari klass
        originalHealth = klass.GetHealth();
        originalAP = klass.GetAP();
        originalStrength = klass.GetStrength();
        originalMagic = klass.GetMagic();
        originalEndurance = klass.GetEndurance();
        originalSpeed = klass.GetSpeed();
        originalCrit = 1.5f;

        // Set current stats sama dengan original
        currHealth = originalHealth;
        currAP = originalAP;
        currStrength = originalStrength;
        currMagic = originalMagic;
        currEndurance = originalEndurance;
        currSpeed = originalSpeed;
        currCrit = originalCrit;

        Debug.Log($"HP: {currHealth}, AP: {currAP}, ST: {currStrength}, MA: {currMagic}, EN: {currEndurance}, SP: {currSpeed}");
        #endregion

        #region Aktion
        unique1 = klass.GetUnique1();
        unique2 = klass.GetUnique2();

        listOfAktions = new List<Aktion>
        {
            unique1,
            unique2
        };
        #endregion

        #region Inputs
        playerActionMap = playerActionAsset.FindActionMap(playerNo == 1 ? "player1" : "player2");

        confirmAction = playerActionMap.FindAction("Confirm");
        confirmAction.Enable();

        hitAction = playerActionMap.FindAction("Hit");
        hitAction.Enable();

        navigateAction = playerActionMap.FindAction("Navigate");
        navigateAction.Enable();
        #endregion

        canvasManager = cm;
        aktionManager = am;

        playerAnimator = GetComponentInChildren<Animator>();
    }

    public void OnFirstTurn()
    {
        ResetHealthAndMana();
        ResetToOriginalStats();
        ResetStatChangeTimers();
        CheckForEnhancement(); 
        Debug.Log($"HP: {currHealth}, AP: {currAP}, ST: {currStrength}, MA: {currMagic}, EN: {currEndurance}, SP: {currSpeed}");

    }

    public void ResetAktionList()
    {
        listOfAktions.Clear();
        listOfAktions.Add(unique1);
        listOfAktions.Add(unique2);
    }

    void CheckForEnhancement()
    {
        if (enhancementsList.Count > 0)
        {
            ResetAktionList();
            foreach (Enhancement enhancement in enhancementsList)
            {
                foreach (EnhancementEffect effect in enhancement.EnhancementEffects())
                {
                    if (effect.GetEnhancementType() == EnhancementType.AktionGain)
                    {
                        listOfAktions.Add(effect.GetAktion());
                        canvasManager.ResetList();
                        Debug.Log($"Added Aktion Gain type Enhancement of name {enhancement.EnhancementName()}");
                    }
                    else if (effect.GetEnhancementType() == EnhancementType.StatBoost)
                    {
                        if (effect.GetChangedStat() == Stat.Strength) SetOriginalStrength(Mathf.CeilToInt(originalStrength + effect.GetFlatStatChangeValue() * (1 + effect.GetPercentageStatChangeValue())));
                        if (effect.GetChangedStat() == Stat.Magic) SetOriginalMagic(Mathf.CeilToInt(originalMagic + effect.GetFlatStatChangeValue() * (1 + effect.GetPercentageStatChangeValue())));
                        if (effect.GetChangedStat() == Stat.Endurance) SetOriginalEndurance(Mathf.CeilToInt(originalEndurance + effect.GetFlatStatChangeValue() * (1 + effect.GetPercentageStatChangeValue())));
                        if (effect.GetChangedStat() == Stat.Speed) SetOriginalSpeed(Mathf.CeilToInt(originalSpeed + effect.GetFlatStatChangeValue() * (1 + effect.GetPercentageStatChangeValue())));
                        if (effect.GetChangedStat() == Stat.Health) SetOriginalHealth(Mathf.CeilToInt(originalHealth + effect.GetFlatStatChangeValue() * (1 + effect.GetPercentageStatChangeValue())));
                        if (effect.GetChangedStat() == Stat.AP) SetOriginalAP(Mathf.CeilToInt(originalAP + effect.GetFlatStatChangeValue() * (1 + effect.GetPercentageStatChangeValue())));
                        if (effect.GetChangedStat() == Stat.Crit) SetOriginalCrit(Mathf.CeilToInt(originalCrit + effect.GetFlatStatChangeValue() * (1 + effect.GetPercentageStatChangeValue())));

                        Debug.Log($"Added Stat Boost type Enhancement of name {enhancement.EnhancementName()}");
                    }
                }
            }
        }
    }

    // Update is called once per frame 
    public void OnUpdate()
    {
        isConfirmPressed = confirmAction.triggered;
        isHitPressed = hitAction.triggered;
    }

    public void EnableActions()
    {
        foreach(InputAction action in playerActionMap)
        {
            action.Enable();
        }
    }

    public void DisableActions()
    {
        foreach (InputAction action in playerActionMap)
        {
            action.Disable();
        }
    }

    #region Stat Getters
    // Getter untuk original stats
    public int GetOriginalHealth() { return originalHealth; }
    public int GetOriginalAP() { return originalAP; }
    public int GetOriginalStrength() { return originalStrength; }
    public int GetOriginalMagic() { return originalMagic; }
    public int GetOriginalEndurance() { return originalEndurance; }
    public int GetOriginalSpeed() { return originalSpeed; }
    public float GetOriginalCrit() { return originalCrit; }

    // Getter untuk current stats
    public int GetHealth() { return currHealth; }
    public int GetAP() { return currAP; }
    public int GetStrength() { return currStrength; }
    public int GetMagic() { return currMagic; }
    public int GetEndurance() { return currEndurance; }
    public int GetSpeed() { return currSpeed; }
    public float GetCrit() { return currCrit; }
    // Getter untuk stat buff timers
    public int GetStrengthBuffTimer() { return strengthBuffTimer; }
    public int GetMagicBuffTimer() { return magicBuffTimer; }
    public int GetEnduranceBuffTimer() { return enduranceBuffTimer; }
    public int GetSpeedBuffTimer() { return speedBuffTimer; }
    public int GetCritBuffTimer() { return critBuffTimer; }
    // Getter untuk stat nerf timers
    public int GetStrengthNerfTimer() { return strengthNerfTimer; }
    public int GetMagicNerfTimer() { return magicNerfTimer; }
    public int GetEnduranceNerfTimer() { return enduranceNerfTimer; }
    public int GetSpeedNerfTimer() { return speedNerfTimer; }
    public int GetCritNerfTimer() { return critNerfTimer; }
    #endregion
    #region Stat Setter
    // Setter untuk current stats
    public void SetHealth(int value) { currHealth = value; }
    public void SetAP(int value) { Mathf.Clamp(currAP = value, 0, originalAP); }
    public void SetStrength(int value) { currStrength = value; }
    public void SetMagic(int value) { currMagic = value; }
    public void SetEndurance(int value) { currEndurance = value; }
    public void SetSpeed(int value) { currSpeed = value; }
    public void SetCrit(float  value) { currCrit = value; }
    public void SetUnique1(Aktion value) { unique1 = value; }
    public void SetUnique2(Aktion value) { unique2 = value; }

    //Setter untuk original stats
    public void SetOriginalHealth(int value) { originalHealth = value; }
    public void SetOriginalAP(int value) { originalAP = value; }
    public void SetOriginalStrength(int value) { originalStrength = value; }
    public void SetOriginalMagic(int value) { originalMagic = value; }
    public void SetOriginalEndurance(int value) { originalEndurance = value; }
    public void SetOriginalSpeed(int value) { originalSpeed = value; }
    public void SetOriginalCrit(float value) { originalCrit = value; }
    // Setter untuk stat buff timers
    public void SetStrengthBuffTimer(int value) { strengthBuffTimer = value; }
    public void SetMagicBuffTimer(int value) { magicBuffTimer = value; }
    public void SetEnduranceBuffTimer(int value) { enduranceBuffTimer = value; }
    public void SetSpeedBuffTimer(int value) { speedBuffTimer = value; }
    public void SetCritBuffTimer(int value) { critBuffTimer = value; }
    // Setter untuk stat nerf timers
    public void SetStrengthNerfTimer(int value) { strengthNerfTimer = value; }
    public void SetMagicNerfTimer(int value) { magicNerfTimer = value; }
    public void SetEnduranceNerfTimer(int value) { enduranceNerfTimer = value; }
    public void SetSpeedNerfTimer(int value) { speedNerfTimer = value; }
    public void SetCritNerfTimer(int value) { critNerfTimer = value; }

    // Function untuk reset stats ke original
    public void ResetHealthAndMana()
    {
        currHealth = originalHealth;
        currAP = originalAP;
    }
    public void ResetToOriginalStats()
    {
        currStrength = originalStrength;
        currMagic = originalMagic;
        currEndurance = originalEndurance;
        currSpeed = originalSpeed;
        currCrit = originalCrit;
    }

    public void DownStatChangeTimers()
    {
        if (strengthBuffTimer > 0)
        {
            strengthBuffTimer--;
            if (strengthBuffTimer <= 0)
            {
                currStrength = originalStrength;

                string msg = $"{name}'s Strength is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }
        if (magicBuffTimer > 0)
        {
            magicBuffTimer--;
            if (magicBuffTimer <= 0)
            {
                currMagic = originalMagic;
                string msg = $"{name}'s Magic is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }
        if (enduranceBuffTimer > 0)
        {
            enduranceBuffTimer--;
            if (enduranceBuffTimer <= 0)
            {
                currEndurance = originalEndurance;

                string msg = $"{name}'s Endurance is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }
        if (speedBuffTimer > 0)
        {
            speedBuffTimer--;
            if (speedBuffTimer <= 0)
            {
                currSpeed = originalSpeed;

                string msg = $"{name}'s Speed is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }
        if (critBuffTimer > 0)
        {
            critBuffTimer--;
            if (critBuffTimer <= 0)
            {
                currCrit = originalCrit;

                string msg = $"{name}'s Crit is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }

        if (strengthNerfTimer > 0)
        {
            strengthNerfTimer--;
            if (strengthNerfTimer <= 0)
            {
                currStrength = originalStrength;

                string msg = $"{name}'s Strength is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }
        if (magicNerfTimer > 0)
        {
            magicNerfTimer--;
            if (magicNerfTimer <= 0)
            {
                currMagic = originalMagic;

                string msg = $"{name}'s Magic is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }
        if (enduranceNerfTimer > 0)
        {
            enduranceNerfTimer--;
            if (enduranceNerfTimer <= 0)
            {
                currEndurance = originalEndurance;

                string msg = $"{name}'s Endurance is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }
        if (speedNerfTimer > 0)
        {
            speedNerfTimer--;
            if (speedNerfTimer <= 0)
            {
                currSpeed = originalSpeed;

                string msg = $"{name}'s Speed is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }
        if (critNerfTimer > 0)
        {
            critNerfTimer--;
            if (critNerfTimer <= 0)
            {
                currCrit = originalCrit;

                string msg = $"{name}'s Crit is back to normal.";
                turnUpdateTexts.Add(msg);
                Debug.Log(msg);
            }
        }

    }

    public void ResetStatChangeTimers()
    {
        strengthBuffTimer = 0;
        magicBuffTimer = 0;
        enduranceBuffTimer = 0;
        speedBuffTimer = 0;
        critBuffTimer = 0;

        strengthNerfTimer = 0;
        magicNerfTimer = 0;
        enduranceNerfTimer = 0;
        speedNerfTimer = 0;
        critNerfTimer = 0;
    }
    #endregion
}

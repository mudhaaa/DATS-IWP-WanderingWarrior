using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // Stat Change Turn Counter
    [SerializeField] private int healthChangeTimer;
    [SerializeField] private int APChangeTimer;
    [SerializeField] private int strengthChangeTimer;
    [SerializeField] private int magicChangeTimer;
    [SerializeField] private int enduranceChangeTimer;
    [SerializeField] private int speedChangeTimer;
    [SerializeField] private float critChangeTimer;
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

        CheckForEnhancement();
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
    #endregion
    #region Stat Setter
    // Setter untuk current stats
    public void SetHealth(int value) { currHealth = value; }
    public void SetAP(int value) { currAP = value; }
    public void SetStrength(int value) { currStrength = value; }
    public void SetMagic(int value) { currMagic = value; }
    public void SetEndurance(int value) { currEndurance = value; }
    public void SetSpeed(int value) { currSpeed = value; }
    public void SetCrit(float  value) { currCrit = value; }
    public void SetUnique1(Aktion value) { unique1 = value; }
    public void SetUnique2(Aktion value) { unique2 = value; }

    //Setter untuk original stats
    public void SetOriginalHealth(int value) { currHealth = value; }
    public void SetOriginalAP(int value) { currAP = value; }
    public void SetOriginalStrength(int value) { currStrength = value; }
    public void SetOriginalMagic(int value) { currMagic = value; }
    public void SetOriginalEndurance(int value) { currEndurance = value; }
    public void SetOriginalSpeed(int value) { currSpeed = value; }
    public void SetOriginalCrit(float value) { currCrit = value; }

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
    #endregion
}

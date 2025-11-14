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

    [SerializeField] private int currHealth;       
    [SerializeField] private int currMana;       
    [SerializeField] private int currStrength;
    [SerializeField] private int currMagic;
    [SerializeField] private int currEndurance;
    [SerializeField] private int currSpeed;

    // Orignal Stats
    [SerializeField] private int originalHealth;
    [SerializeField] private int originalMana;
    [SerializeField] private int originalStrength;
    [SerializeField] private int originalMagic;
    [SerializeField] private int originalEndurance;
    [SerializeField] private int originalSpeed;
    #endregion

    #region Aktion
    [SerializeField] private Aktion unique1;
    [SerializeField] private Aktion unique2;
    [SerializeField] private List<Aktion> listOfAktions;
    public List<Aktion> GetAktionList() { return listOfAktions; }
    public Aktion GetAktion(int i) { return listOfAktions[i]; }
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

    private CanvasManager canvasManager;
    private AktionManager aktionManager;

    public void OnStart(int playerNo, CanvasManager cm, AktionManager am)
    {
        #region Stats
        // Simpan original values dari klass
        originalHealth = klass.GetHealth();
        originalMana = klass.GetMana();
        originalStrength = klass.GetStrength();
        originalMagic = klass.GetMagic();
        originalEndurance = klass.GetEndurance();
        originalSpeed = klass.GetSpeed();

        // Set current stats sama dengan original
        currHealth = originalHealth;
        currMana = originalMana;
        currStrength = originalStrength;
        currMagic = originalMagic;
        currEndurance = originalEndurance;
        currSpeed = originalSpeed;
        #endregion

        #region Aktion
        unique1 = klass.GetUnique1();
        unique2 = klass.GetUnique2();

        listOfAktions = new List<Aktion>();
        listOfAktions.Add(unique1);
        listOfAktions.Add(unique2);
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
    }

    // Update is called once per frame 
    public void OnUpdate()
    {
        isConfirmPressed = confirmAction.triggered;
        isHitPressed = hitAction.triggered;

        if (isConfirmPressed)
        {
            if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.P1turn)
            {
                BattleManager.instance.ActivateBattleBarState();
            }
            else if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.P2turn)
            {
                BattleManager.instance.ActivateBattleBarState();
            }
        }

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
    public int GetOriginalMana() { return originalMana; }
    public int GetOriginalStrength() { return originalStrength; }
    public int GetOriginalMagic() { return originalMagic; }
    public int GetOriginalEndurance() { return originalEndurance; }
    public int GetOriginalSpeed() { return originalSpeed; }

    // Getter untuk current stats
    public int GetHealth() { return currHealth; }
    public int GetMana() { return currMana; }
    public int GetStrength() { return currStrength; }
    public int GetMagic() { return currMagic; }
    public int GetEndurance() { return currEndurance; }
    public int GetSpeed() { return currSpeed; }
    #endregion
    #region Stat Setter
    // Setter untuk current stats
    public void SetHealth(int value) { currHealth = value; }
    public void SetMana(int value) { currMana = value; }
    public void SetStrength(int value) { currStrength = value; }
    public void SetMagic(int value) { currMagic = value; }
    public void SetEndurance(int value) { currEndurance = value; }
    public void SetSpeed(int value) { currSpeed = value; }
    public void SetUnique1(Aktion value) { unique1 = value; }
    public void SetUnique2(Aktion value) { unique2 = value; }

    // Function untuk reset stats ke original
    public void ResetToOriginalStats()
    {
        currHealth = originalHealth;
        currMana = originalMana;
        currStrength = originalStrength;
        currMagic = originalMagic;
        currEndurance = originalEndurance;
        currSpeed = originalSpeed;
    }
    #endregion
}

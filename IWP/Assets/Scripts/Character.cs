using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    #region Stats
    [SerializeField] private CharacterKlass klass;
    [SerializeField] public int currHealth;       
    [SerializeField] private int currStrength;
    [SerializeField] private int currMagic;
    [SerializeField] private int currEndurance;
    [SerializeField] private int currSpeed;

    [SerializeField] private Aktion unique1;
    [SerializeField] private Aktion unique2;

    // Orignal Stats
    private int originalHealth;
    private int originalStrength;
    private int originalMagic;
    private int originalEndurance;
    private int originalSpeed;
    #endregion

    [SerializeField] private InputActionAsset playerActionAsset;
    private InputActionMap playerActionMap;
    private InputAction confirmAction;

    public void OnStart(int playerNo)
    {
        #region Stats
        // Simpan original values dari klass
        originalHealth = klass.GetHealth();
        originalStrength = klass.GetStrength();
        originalMagic = klass.GetMagic();
        originalEndurance = klass.GetEndurance();
        originalSpeed = klass.GetSpeed();

        // Set current stats sama dengan original
        currHealth = originalHealth;
        currStrength = originalStrength;
        currMagic = originalMagic;
        currEndurance = originalEndurance;
        currSpeed = originalSpeed;
        unique1 = klass.GetUnique1();
        unique2 = klass.GetUnique2();
        #endregion

        playerActionMap = playerActionAsset.FindActionMap(playerNo == 1 ? "player1" : "player2");
        confirmAction = playerActionMap.FindAction("Confirm");
        confirmAction.Enable();
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if (confirmAction.IsPressed())
        {
            if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.P1turn)
                BattleManager.instance.ChangeState(BattleManager.BattleStates.P2turn);
            else 
                BattleManager.instance.ChangeState(BattleManager.BattleStates.P1turn);

        }
    }


    #region Stat Getters
    // Getter untuk original stats
    public int GetOriginalHealth() { return originalHealth; }
    public int GetOriginalStrength() { return originalStrength; }
    public int GetOriginalMagic() { return originalMagic; }
    public int GetOriginalEndurance() { return originalEndurance; }
    public int GetOriginalSpeed() { return originalSpeed; }

    // Getter untuk current stats
    public int GetStrength() { return currStrength; }
    public int GetMagic() { return currMagic; }
    public int GetEndurance() { return currEndurance; }
    public int GetSpeed() { return currSpeed; }
    public Aktion GetUnique1() { return unique1; }
    public Aktion GetUnique2() { return unique2; }
    #endregion
    #region Stat Setter
    // Setter untuk current stats
    public void SetHealth(int value) { currHealth = value; }
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
        currStrength = originalStrength;
        currMagic = originalMagic;
        currEndurance = originalEndurance;
        currSpeed = originalSpeed;
    }
    #endregion
}

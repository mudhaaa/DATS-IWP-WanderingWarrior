using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages all the battle logic flow and centralises all the other managers
/// </summary>
public class BattleManager : MonoBehaviour
{
    #region Singleton
    public static BattleManager instance;
    private void Awake()
    {
        // Check kalau dah ada instance lain
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Set instance ini
        instance = this;

        // Optional: Kalau nak persist across scenes
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    [SerializeField] private CameraManager cameraManager;

    #region States
    [Header("Battle States")]
    [SerializeField] private BattleStates currState;
    public BattleStates GetCurrState() { return currState; }
    public bool IsAttackState() { return currState == BattleStates.P1attack || currState == BattleStates.P2attack; }
    public enum BattleStates
    {
        RoundStart,
        P1turn,
        P2turn,
        P1attack,
        P2attack,
        P1winRound,
        P2winRound,
        P1winBattle,
        P2winBattle,
        Enhancement,
        StatusAktion
    }

    public void ChangeState(BattleStates state)
    {
        currState = state;
    }
    #endregion

    [Header("Players")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private bool player1start;


    private int damageFormula1; // damage calculation done by player 1
    private int damageFormula2; // damage calculation done by player 2

    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private BattleBarManager barManager;
    [SerializeField] private AktionManager aktionManager;
    [SerializeField] private EnhancementManager enhancementManager;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager.OnStart(canvasManager, aktionManager);
        enhancementManager.OnStart(playerManager, canvasManager);
        canvasManager.OnStart(playerManager, enhancementManager);
        barManager.OnStart(playerManager);
        aktionManager.OnStart(playerManager, barManager, canvasManager);
    }

    public void OnFirstTurn()
    {
        playerManager.OnFirstTurn();

        cameraManager.OnStart();

        canvasManager.OnFirstTurn();

    }

    // Update is called once per frame
    void Update()
    {
        playerManager.OnUpdate();
        canvasManager.OnUpdate();
        barManager.OnUpdate();
        cameraManager.OnUpdate();
        aktionManager.OnUpdate();
        enhancementManager.OnUpdate();
    }

    public void ActivateBattleBarState()
    {
        if (currState == BattleStates.P1turn) currState = BattleStates.P1attack;
        else if (currState == BattleStates.P2turn) currState = BattleStates.P2attack;

    }

    public void EndAttackState()
    {

        if (currState == BattleStates.P1attack)
        {
            aktionManager.AktionEffect(playerManager.GetPlayer1(), canvasManager.GetP1Aktion());
            currState = BattleStates.P2turn;
        }
        else if (currState == BattleStates.P2attack)
        {
            aktionManager.AktionEffect(playerManager.GetPlayer2(), canvasManager.GetP2Aktion());
            currState = BattleStates.P1turn;
        }
        CheckForRoundWinner();
    }

    public void EnterStatusAktionState(int i)
    {
        StartCoroutine(StatusAktion(i));
    }

    IEnumerator StatusAktion(int i)
    {
        currState = BattleStates.StatusAktion;

        if (i == 1)
        {
            Debug.Log("p2 status move");
            aktionManager.AktionEffect(playerManager.GetPlayer1(), canvasManager.GetP1Aktion());
        }
        else if (i == 2)
        {
            Debug.Log("p2 status move");
            aktionManager.AktionEffect(playerManager.GetPlayer2(), canvasManager.GetP2Aktion());
        }

        yield return new WaitForSeconds(5);
        if (i == 1) currState = BattleStates.P2turn;
        else if(i == 2) currState= BattleStates.P1turn;
    }

    public void CheckForRoundWinner()
    {
        if (playerManager.GetPlayer1().GetHealth() <= 0)
        {
            StartCoroutine(EndRound(2));
        }
        else if (playerManager.GetPlayer2().GetHealth() <= 0)
        {
            StartCoroutine(EndRound(1));
        }
    }
    IEnumerator EndRound(int i)
    {
        if (i == 1) currState = BattleStates.P1winRound;
        else if (i == 2) currState = BattleStates.P2winRound;

        yield return new WaitForSecondsRealtime(5);

        currState = BattleStates.Enhancement;
    }
}
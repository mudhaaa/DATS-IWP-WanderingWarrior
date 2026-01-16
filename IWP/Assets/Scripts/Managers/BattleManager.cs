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

    [SerializeField] private int turnCounter;
    public int GetTurnCounter() { return turnCounter; }

    [SerializeField] private int p1wins;
    public int GetP1Wins() { return p1wins; }
    [SerializeField] private int p2wins;
    public int GetP2Wins() { return p2wins; }
    #endregion

    #region Events
    public System.Action OnTurn1;
    public System.Action OnEndOfTurn;
    #endregion

    [Header("Players")]
    [SerializeField] private PlayerManager playerManager;
    public PlayerManager PlayerManager() { return playerManager; }



    [SerializeField] private bool player1start;


    [SerializeField] private CameraManager cameraManager;
    public CameraManager CameraManager() { return cameraManager; }

    [SerializeField] private CanvasManager canvasManager;
    public CanvasManager CanvasManager() { return canvasManager; }

    [SerializeField] private BattleBarManager barManager;
    public BattleBarManager BattleBarManager() { return barManager; }

    [SerializeField] private AktionManager aktionManager;
    public AktionManager AktionManager() { return aktionManager; }

    [SerializeField] private EnhancementManager enhancementManager;
    public EnhancementManager EnhancementManager() { return enhancementManager; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager.OnStart(canvasManager, aktionManager);
        enhancementManager.OnStart(playerManager, canvasManager);
        canvasManager.OnStart(playerManager, enhancementManager, barManager);
        barManager.OnStart(playerManager, aktionManager);
        aktionManager.OnStart(playerManager, barManager, canvasManager);
        ChangeState(BattleStates.Enhancement);

        //StartCoroutine(canvasManager.IntroSequence());
    }

    public void OnFirstTurn()
    {
        playerManager.OnFirstTurn();

        cameraManager.OnStart();

        enhancementManager.OnFirstTurn();

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
        if (currState == BattleStates.P1turn)
        {
            playerManager.GetPlayer2().PlayAnimation("Block Idle");
            currState = BattleStates.P1attack;
        }
        else if (currState == BattleStates.P2turn)
        {
            playerManager.GetPlayer1().PlayAnimation("Block Idle");
            currState = BattleStates.P2attack;
        }

    }

    private Coroutine currentAttackCoroutine = null;

    public void EndAttackState()
    {
        // Guard against multiple calls
        if (currentAttackCoroutine != null)
        {
            return; // Already processing, ignore
        }

        Debug.Log($"[EndAttackState] Starting - State: {currState}, Turn: {turnCounter}");

        currentAttackCoroutine = StartCoroutine(EndOfAttackState());
    }

    Coroutine currentCoroutine = null;
    IEnumerator EndOfAttackState()
    {
        if (currState == BattleStates.P1attack)
        {
            aktionManager.AktionEffect(playerManager.GetPlayer1(), canvasManager.GetP1Aktion());
            yield return new WaitForSeconds(4);
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(canvasManager.TurnUpdates(1));
            }
        }
        else if (currState == BattleStates.P2attack)
        {
            aktionManager.AktionEffect(playerManager.GetPlayer2(), canvasManager.GetP2Aktion());
            yield return new WaitForSeconds(4);
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(canvasManager.TurnUpdates(2));
            }
        }
        else
        {
            Debug.LogWarning($"[EndOfAttackState] Unexpected state: {currState}");
        }

        barManager.ActivateBattleBars(false);
        barManager.SetInputAcceptState(1, false);
        barManager.SetInputAcceptState(2, false);

        currentAttackCoroutine = null;

        playerManager.GetPlayer1().PlayAnimation("Idle");
        playerManager.GetPlayer2().PlayAnimation("Idle");

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
            Debug.Log("P1 status move");
            aktionManager.AktionEffect(playerManager.GetPlayer1(), canvasManager.GetP1Aktion());
            
            yield return new WaitForSeconds(4);

            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(canvasManager.TurnUpdates(i));
            }
        }
        else if (i == 2)
        {
            Debug.Log("P2 status move");
            aktionManager.AktionEffect(playerManager.GetPlayer2(), canvasManager.GetP2Aktion());

            yield return new WaitForSeconds(4);

            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(canvasManager.TurnUpdates(i));
            }
        }
    }

    public void EndOfTurn(int i)
    {
        currentCoroutine = null;
        if (i == 1)
        {
            currState = BattleStates.P2turn;
            int newAP = playerManager.GetPlayer2().GetAP() + 1;
            playerManager.GetPlayer2().SetAP(newAP);
            canvasManager.UpdatePlayerBars(playerManager.GetPlayer2());

            playerManager.GetPlayer2().DownStatChangeTimers();
            canvasManager.UpdateStatusChangeUI(1);
            canvasManager.UpdateStatusChangeUI(2);

            Debug.Log("Starting P2 turn");
        }
        else if (i == 2)
        {
            currState = BattleStates.P1turn;
            int newAP = playerManager.GetPlayer1().GetAP() + 1;
            playerManager.GetPlayer1().SetAP(newAP);
            canvasManager.UpdatePlayerBars(playerManager.GetPlayer1());

            playerManager.GetPlayer1().DownStatChangeTimers();
            canvasManager.UpdateStatusChangeUI(1);
            canvasManager.UpdateStatusChangeUI(2);

            Debug.Log("Starting P1 turn");

        }
        turnCounter = turnCounter + 1;

        CheckForRoundWinner();
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
        if (i == 1)
        {
            currState = BattleStates.P1winRound;
            p1wins++;
            canvasManager.UpdateWinIcons(1);
        }
        else if (i == 2)
        {
            currState = BattleStates.P2winRound;
            p2wins++;
            canvasManager.UpdateWinIcons(2);
        }

        yield return new WaitForSecondsRealtime(5);

        if (p1wins == 3)
        {
            currState = BattleStates.P1winBattle;
        }
        else if (p2wins == 3)
        {
            currState = BattleStates.P2winBattle;
        }
        else
        {
            currState = BattleStates.Enhancement;
        }
    }
}
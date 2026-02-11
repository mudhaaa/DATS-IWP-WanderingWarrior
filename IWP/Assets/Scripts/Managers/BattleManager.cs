using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        StatusAktion,
        AktionAnimation
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


    [SerializeField] private int maxWins;
    public int GetMaxWins() { return maxWins;}
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


    [SerializeField] private AudioData audio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(cameraManager  == null) cameraManager = FindAnyObjectByType<CameraManager>();
        if(canvasManager == null) canvasManager = FindAnyObjectByType<CanvasManager>();
        if(barManager == null) barManager = FindAnyObjectByType<BattleBarManager>();
        if(aktionManager == null) aktionManager = FindAnyObjectByType<AktionManager>();
        if(enhancementManager == null) enhancementManager = FindAnyObjectByType<EnhancementManager>();

        playerManager.OnStart(canvasManager, aktionManager);
        enhancementManager.OnStart(playerManager, canvasManager);
        canvasManager.OnStart(playerManager, enhancementManager, barManager);
        barManager.OnStart(playerManager, aktionManager);
        aktionManager.OnStart(playerManager, barManager, canvasManager);

        ChangeState(BattleStates.Enhancement);

        maxWins = 1;

        AudioManager.instance.PlayAudio(audio);

        Time.timeScale = 1;
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

    private void LateUpdate()
    {
        cameraManager.OnLateUpdate();
    }

    public void Rematch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        barManager.ActivateBattleBars(false);

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
            CheckForRoundWinner();
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(canvasManager.TurnUpdates(1));
            }
        }
        else if (currState == BattleStates.P2attack)
        {
            aktionManager.AktionEffect(playerManager.GetPlayer2(), canvasManager.GetP2Aktion());
            yield return new WaitForSeconds(4);
            CheckForRoundWinner();
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
        barManager.currentCoroutine = null;
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

            Debug.Log("Starting P2 turn");
        }
        else if (i == 2)
        {
            currState = BattleStates.P1turn;
            int newAP = playerManager.GetPlayer1().GetAP() + 1;
            playerManager.GetPlayer1().SetAP(newAP);
            canvasManager.UpdatePlayerBars(playerManager.GetPlayer1());



            Debug.Log("Starting P1 turn");

        }
        turnCounter = turnCounter + 1;
    }

    public void CheckForRoundWinner()
    {
        if (currState == BattleStates.P2attack || currState == BattleStates.P2turn)
        {
            if (playerManager.GetPlayer1().GetHealth() <= 0)
            {
                currentCoroutine = StartCoroutine(EndRound(2));
                Debug.Log("P2 win");
            }
            else if (playerManager.GetPlayer2().GetHealth() <= 0)
            {
                playerManager.GetPlayer2().PlayAnimation("Death");
                currentCoroutine = StartCoroutine(EndRound(1));
                Debug.Log("P1 win");

            }
        }
        else if (currState == BattleStates.P1attack || currState == BattleStates.P1turn)
        {
            if (playerManager.GetPlayer2().GetHealth() <= 0)
            {
                currentCoroutine = StartCoroutine(EndRound(1));
                Debug.Log("P1 win");

            }
            else if (playerManager.GetPlayer1().GetHealth() <= 0)
            {
                playerManager.GetPlayer1().PlayAnimation("Death");
                currentCoroutine = StartCoroutine(EndRound(2)); 
                Debug.Log("P2 win");

            }
        }
    }

    IEnumerator EndRound(int i)
    {
        if (i == 1)
        {
            currState = BattleStates.P1winRound;
            p1wins++;
            playerManager.GetPlayer1().IncreaseWins();
            canvasManager.UpdateWinIcons(1);
        }
        else if (i == 2)
        {
            currState = BattleStates.P2winRound;
            p2wins++;
            playerManager.GetPlayer2().IncreaseWins();
            canvasManager.UpdateWinIcons(2);
        }

        yield return new WaitForSecondsRealtime(3);

        if (playerManager.GetPlayer1().GetWins() == maxWins)
        {
            currState = BattleStates.P1winBattle;
            canvasManager.ActivateVictoryUI(1);
            playerManager.GetPlayer1().PlayAnimation("Victory");
        }
        else if (playerManager.GetPlayer2().GetWins() == maxWins)
        {
            currState = BattleStates.P2winBattle;
            canvasManager.ActivateVictoryUI(2);
            playerManager.GetPlayer2().PlayAnimation("Victory");
        }
        else
        {
            currState = BattleStates.Enhancement;

            playerManager.GetPlayer1().PlayAnimation("Idle");
            playerManager.GetPlayer2().PlayAnimation("Idle");
        }
    }
}
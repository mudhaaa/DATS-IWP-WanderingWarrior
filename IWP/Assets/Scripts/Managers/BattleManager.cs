using System.Collections;
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
        Enhancement
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasManager.OnStart(playerManager);
        barManager.OnStart(playerManager);
        aktionManager.OnStart(playerManager, barManager);
    }

    public void OnFirstTurn()
    {
        playerManager.OnStart(canvasManager, aktionManager);

        cameraManager.OnStart();

    }

    // Update is called once per frame
    void Update()
    {
        playerManager.OnUpdate();
        canvasManager.OnUpdate();
        barManager.OnUpdate();
        cameraManager.OnUpdate();

    }
    
    public void ActivateBattleBarState()
    {
        if(currState == BattleStates.P1turn) currState = BattleStates.P1attack;
        else if(currState == BattleStates.P2turn) currState = BattleStates.P2attack;

    }

    public void EndAttackState()
    {
        if (currState == BattleStates.P1attack) currState = BattleStates.P2turn;
        else if (currState == BattleStates.P2attack) currState = BattleStates.P1turn;
    }
}

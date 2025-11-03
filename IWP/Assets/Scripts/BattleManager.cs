using UnityEngine;

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
    #endregion

    [Header("Players")]
    [SerializeField] private Character player1;
    [SerializeField] private Character player2;
    [SerializeField] private bool player1start;

    private int damageFormula1; // damage calculation done by player 1
    private int damageFormula2; // damage calculation done by player 2



    public void ChangeState(BattleStates state)
    {
        currState = state;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player1.OnStart(1);
        player2.OnStart(2);

        cameraManager.OnStart();

        SpeedCheck();
    }

    // Update is called once per frame
    void Update()
    {
        if (currState == BattleStates.P1turn)
        {
            player1.OnUpdate();
            cameraManager.ChangeCameraPos(1);
        }
        else if (currState == BattleStates.P2turn)
        {
            player2.OnUpdate();
            cameraManager.ChangeCameraPos(2);
        } 
    }
    


    void SpeedCheck()
    {
        // speed check
        if (player1.GetSpeed() > player2.GetSpeed())
        {
            Debug.Log("Player 1 start");

            player1start = true;
            currState = BattleStates.P1turn;
        }
        else if (player1.GetSpeed() < player2.GetSpeed())
        {
            Debug.Log("Player 2 start");

            player1start = false;
            currState = BattleStates.P2turn;
        }
        // speed draw
        else
        {
            Debug.Log("Speed tie");
            int speedTie = Random.Range(0, 2);

            // p1 start
            if (speedTie > 0)
            {
                player1start = true;
                currState = BattleStates.P1turn;
                Debug.Log("Player 1 start");
            }
            else
            {
                player1start = false;
                currState = BattleStates.P2turn;
                Debug.Log("Player 2 start");
            }
        }
    }
}

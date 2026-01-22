using DG.Tweening;
using UnityEngine;
using static BattleManager;

public class PlayerManager : MonoBehaviour
{
    [Header("Player 1 Characters")]
    [SerializeField] GameObject knightP1;
    [SerializeField] GameObject mageP1;
    [SerializeField] GameObject bulwarkP1;

    [SerializeField] private Character player1;
    [SerializeField] private CharacterKlass klassP1;
    public Character GetPlayer1() {  return player1; }


    [Header("Player 2 Characters")]
    [SerializeField] GameObject knightP2;
    [SerializeField] GameObject mageP2;
    [SerializeField] GameObject bulwarkP2;

    [SerializeField] private Character player2;
    [SerializeField] private CharacterKlass klassP2;
    public Character GetPlayer2() { return player2; }

    private CanvasManager canvasManager;
    private AktionManager aktionManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(CanvasManager cm, AktionManager am)
    {
        canvasManager = cm;
        aktionManager = am;

        if (CharacterSelectManager.instance.Player1() != null) klassP1 = CharacterSelectManager.instance.Player1();
        if (CharacterSelectManager.instance.Player2() != null) klassP2 = CharacterSelectManager.instance.Player2();

        player1.OnStart(1, canvasManager, aktionManager, klassP1);
        player2.OnStart(2, canvasManager, aktionManager, klassP2);

        ActivateModels();
    }

    public void OnFirstTurn()
    {
        player1.OnFirstTurn();
        player2.OnFirstTurn();
        SpeedCheck();
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if(BattleManager.instance.GetCurrState() == BattleStates.P1turn || 
            BattleManager.instance.IsAttackState() || 
            BattleManager.instance.GetCurrState() == BattleStates.Enhancement) 
            player1.OnUpdate();

        if (BattleManager.instance.GetCurrState() == BattleStates.P2turn ||
            BattleManager.instance.IsAttackState() ||
            BattleManager.instance.GetCurrState() == BattleStates.Enhancement) 
            player2.OnUpdate();
    }

    void ActivateModels()
    {
        knightP1.SetActive(false);
        mageP1.SetActive(false);
        bulwarkP1.SetActive(false);

        knightP2.SetActive(false);
        mageP2.SetActive(false);
        bulwarkP2.SetActive(false);

        if (klassP1.name.Contains("Knight"))
        {
            knightP1.SetActive(true);
            player1.SetAnimator(knightP1.GetComponentInChildren<Animator>());
        }
        if (klassP1.name.Contains("Mage"))
        {
            mageP1.SetActive(true);
            player1.SetAnimator(mageP1.GetComponentInChildren<Animator>());
        }
        if (klassP1.name.Contains("Bulwark"))
        {
            bulwarkP1.SetActive(true);
            player1.SetAnimator(bulwarkP1.GetComponentInChildren<Animator>());
        }

        if (klassP2.name.Contains("Knight"))
        {
            knightP2.SetActive(true);
            player2.SetAnimator(knightP2.GetComponentInChildren<Animator>());
        }
        if (klassP2.name.Contains("Mage"))
        {
            mageP2.SetActive(true);
            player2.SetAnimator(mageP2.GetComponentInChildren<Animator>());
        }
        if (klassP2.name.Contains("Bulwark"))
        {
            bulwarkP2.SetActive(true);
            player2.SetAnimator(bulwarkP2.GetComponentInChildren<Animator>());
        }

    }

    public void MoveTowardsEnemy()
    {
        if (BattleManager.instance.GetCurrState() == BattleStates.P1attack)
        {
            player1.transform.DOMove(player2.transform.position - Vector3.right, 0.1f);
        }
        else if (BattleManager.instance.GetCurrState() == BattleStates.P1attack)
        {
            player2.transform.DOMove(player1.transform.position - Vector3.left, 0.1f);
        }
    }
    public void SpeedCheck()
    {
        // speed check
        if (player1.GetSpeed() > player2.GetSpeed())
        {
            Debug.Log("Player 1 start");

            BattleManager.instance.ChangeState(BattleStates.P1turn);
        }
        else if (player1.GetSpeed() < player2.GetSpeed())
        {
            Debug.Log("Player 2 start");

            BattleManager.instance.ChangeState(BattleStates.P2turn);

        }
        // speed draw
        else
        {
            Debug.Log("Speed tie");
            int speedTie = Random.Range(0, 2);

            // p1 start
            if (speedTie > 0)
            {
                BattleManager.instance.ChangeState(BattleStates.P1turn);

                Debug.Log("Player 1 start");
            }
            else
            {
                BattleManager.instance.ChangeState(BattleStates.P2turn);

                Debug.Log("Player 2 start");
            }
        }
    }

    
}

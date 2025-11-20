using NUnit.Framework;
using UnityEngine;
using static BattleManager;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private Character player1;
    public Character GetPlayer1() {  return player1; }

    [SerializeField] private Character player2;
    public Character GetPlayer2() { return player2; }

    private CanvasManager canvasManager;
    private AktionManager aktionManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(CanvasManager cm, AktionManager am)
    {
        canvasManager = cm;
        aktionManager = am;

        player1.OnStart(1, canvasManager, aktionManager);
        player2.OnStart(2, canvasManager, aktionManager);
    }

    public void OnFirstTurn()
    {



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

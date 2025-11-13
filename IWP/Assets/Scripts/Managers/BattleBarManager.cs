using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleBarManager : MonoBehaviour
{
    [Header("Player 1")]
    [SerializeField] private Slider battleBarP1;
    [SerializeField] private BattleBarSlider bbSliderP1;
    public BattleBarSlider GetSliderP1() {  return bbSliderP1; }

    [Header("Player 2")]
    [SerializeField] private Slider battleBarP2;
    [SerializeField] private BattleBarSlider bbSliderP2;
    public BattleBarSlider GetSliderP2() { return bbSliderP2; }


    private PlayerManager playerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(PlayerManager pm)
    {
        playerManager = pm;

        battleBarP1.gameObject.SetActive(false);
        battleBarP2.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        ActivateBattleBars();
        BattleBarSliderMoving();
        WaitForInput();
    }

    public void ActivateBattleBars()
    {
        battleBarP1.gameObject.SetActive(BattleManager.instance.IsAttackState());
        battleBarP2.gameObject.SetActive(BattleManager.instance.IsAttackState());

    }

    bool p1barMax = false;
    bool p2barMax = false;
    void BattleBarSliderMoving()
    {
        if (BattleManager.instance.IsAttackState())
        {
            if (battleBarP1.gameObject.activeSelf && battleBarP2.gameObject.activeSelf)
            {
                if (!p1pressed)
                {
                    if (battleBarP1.value >= battleBarP1.maxValue) p1barMax = true;
                    else if (battleBarP1.value <= battleBarP1.minValue) p1barMax = false;
                    if (!p1barMax)
                        battleBarP1.value += Time.deltaTime * 100 / (playerManager.GetPlayer1().GetSpeed());
                    else
                        battleBarP1.value -= Time.deltaTime * 100 / (playerManager.GetPlayer1().GetSpeed());
                }

                if (!p2pressed)
                {
                    if (battleBarP2.value >= battleBarP2.maxValue) p2barMax = true;
                    else if (battleBarP2.value <= battleBarP2.minValue) p2barMax = false;

                    if (!p2barMax)
                        battleBarP2.value += Time.deltaTime * 100 / (playerManager.GetPlayer2().GetSpeed());
                    else
                        battleBarP2.value -= Time.deltaTime * 100 / (playerManager.GetPlayer2().GetSpeed());
                }

            }
        }
    }
    bool p1pressed = false;
    bool p2pressed = false;
    public void WaitForInput()
    {
        if (BattleManager.instance.IsAttackState())
        {
            if (playerManager.GetPlayer1().IsHitPressed())
            {
                p1pressed = true;
                Debug.Log("P1: " + bbSliderP1.GetBarState().ToString());
            }
            if (playerManager.GetPlayer2().IsHitPressed())
            {
                p2pressed = true; 
                Debug.Log("P2: " + bbSliderP2.GetBarState().ToString());
            }
            if (p1pressed && p2pressed) StartCoroutine(EndWaitForInput());
        }
        else
        {
            p1pressed = false;
            p2pressed = false;
        }
    }

    IEnumerator EndWaitForInput()
    {
        yield return new WaitForSeconds(3);
        BattleManager.instance.EndAttackState();
    }
}

using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static BattleBarSlider;

public class BattleBarManager : MonoBehaviour
{
    [Header("Player 1")]
    [SerializeField] private Slider battleBarP1;
    [SerializeField] private BattleBarSlider bbSliderP1;
    public BattleBarSlider GetSliderP1() {  return bbSliderP1; }

    [SerializeField] private FloatingTextUI floatingTextP1;

    [Header("Player 2")]
    [SerializeField] private Slider battleBarP2;
    [SerializeField] private BattleBarSlider bbSliderP2;
    public BattleBarSlider GetSliderP2() { return bbSliderP2; }

    [SerializeField] private FloatingTextUI floatingTextP2;

    [SerializeField] private CanvasGroup canvasGroup;

    private PlayerManager playerManager;
    private AktionManager aktionManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(PlayerManager pm, AktionManager am)
    {
        playerManager = pm;
        aktionManager = am;

        canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        BattleBarSliderMoving();
        WaitForInput();
    }

    public void ActivateBattleBars(bool b)
    {

        if (b) canvasGroup.DOFade(1, 0.2f);
        else canvasGroup.DOFade(0, 0.1f);

    }

    bool p1barMax = false;
    bool p2barMax = false;
    [SerializeField] private float barSpeedP1;
    [SerializeField] private float barSpeedP2;
    void BattleBarSliderMoving()
    {
        if (BattleManager.instance.IsAttackState())
        {
            AttackAktion attack = aktionManager.currentAktion as AttackAktion;

            barSpeedP1 = 100 / (playerManager.GetPlayer1().GetSpeed() / 2.5f );

            barSpeedP2 = 100 / (playerManager.GetPlayer2().GetSpeed() / 2.5f );

            if (battleBarP1.gameObject.activeSelf && battleBarP2.gameObject.activeSelf)
            {
                if (!p1pressed)
                {
                    if (battleBarP1.value >= battleBarP1.maxValue) p1barMax = true;
                    else if (battleBarP1.value <= battleBarP1.minValue) p1barMax = false;
                    if (!p1barMax)
                        battleBarP1.value += Time.deltaTime * barSpeedP1;
                    else
                        battleBarP1.value -= Time.deltaTime * barSpeedP1;
                }

                if (!p2pressed)
                {
                    if (battleBarP2.value >= battleBarP2.maxValue) p2barMax = true;
                    else if (battleBarP2.value <= battleBarP2.minValue) p2barMax = false;

                    if (!p2barMax)
                        battleBarP2.value += Time.deltaTime * barSpeedP2;
                    else
                        battleBarP2.value -= Time.deltaTime * barSpeedP2;
                }

            }
        }
    }

    bool p1pressed = false;
    bool p2pressed = false;
    public Coroutine currentCoroutine;
    bool endingAttack = false;
    public void WaitForInput()
    {
        if (BattleManager.instance.IsAttackState())
        {
            if (playerManager.GetPlayer1().IsHitPressed() && !p1pressed) // Add !p1pressed check
            {
                p1pressed = true;
                Debug.Log("P1: " + bbSliderP1.GetBarState().ToString());
                SetBarResultText(1, bbSliderP1.GetBarState());
            }
            if (playerManager.GetPlayer2().IsHitPressed() && !p2pressed) // Add !p2pressed check
            {
                p2pressed = true;
                Debug.Log("P2: " + bbSliderP2.GetBarState().ToString());
                SetBarResultText(2, bbSliderP2.GetBarState());
            }

            if (p1pressed && p2pressed && !endingAttack) // Check !endingAttack instead of coroutine
            {
                endingAttack = true;
                currentCoroutine = StartCoroutine(ExitBarState());
                Debug.Log("Both players pressed, starting exit coroutine");
            }
        }
        else
        {
            SetInputAcceptState(1, false);
            SetInputAcceptState(2, false);
        }
    }

    public void SetInputAcceptState(int i, bool b)
    {
        if (i == 1) p1pressed = b;
        else if (i == 2) p2pressed = b;

        // Reset when both are set to false (new round)
        if (!p1pressed && !p2pressed)
        {
            endingAttack = false;
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }
    }

    void SetBarResultText(int i, BattleBarSlider.BarState barState)
    {
        StartCoroutine(BarResult(i, barState));
    }

    IEnumerator BarResult(int i, BattleBarSlider.BarState barState)
    {
        float l;
        if (barState == BattleBarSlider.BarState.Good) l = .75f;
        else if (barState == BattleBarSlider.BarState.Mid) l = .65f;
        else l = .55f;

        yield return new WaitForSeconds(.1f);

        if (i == 1)
        {
            floatingTextP1.SetText(barState.ToString(), l);
        }
        else if (i == 2)
        {
            floatingTextP2.SetText(barState.ToString(), l);
        }
    }

    IEnumerator ExitBarState()
    {
        Debug.Log("Waiting 2 seconds before ending attack state...");
        yield return new WaitForSeconds(2);

        Debug.Log("Exiting Attack State");
        BattleManager.instance.EndAttackState();

        // Reset flags
        currentCoroutine = null;
        endingAttack = false;
    }
}

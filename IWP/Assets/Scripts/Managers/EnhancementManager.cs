using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EnhancementManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainCanvas;

    [SerializeField] private List<Enhancement> enhancements;

    private PlayerManager playerManager;
    private CanvasManager canvasManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(PlayerManager pm, CanvasManager cm)
    {
        playerManager = pm;
        canvasManager = cm;
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        FadeEnhancementCanvas();

        GetInput();
    }

    void GetInput()
    {
        if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.Enhancement)
        {
            if (playerManager.GetPlayer1().IsConfirmPressed() || playerManager.GetPlayer2().IsConfirmPressed()) StartNextRound();
        }
    }
    void StartNextRound()
    {
        Debug.Log("New Round start");
        BattleManager.instance.ChangeState(BattleManager.BattleStates.RoundStart);
        StartCoroutine(canvasManager.IntroSequence());
    }

    void FadeEnhancementCanvas()
    {
        if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.Enhancement)
        {
            mainCanvas.DOFade(1, 0.5f);
        }
        else mainCanvas.DOFade(0, 0.5f);
    }
}

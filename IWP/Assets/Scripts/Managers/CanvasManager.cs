using DG.Tweening;
using DG.Tweening.Core;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BattleManager;

public class CanvasManager : MonoBehaviour
{
    [Header("Player 1")]
    [SerializeField] private Canvas canvasP1;
    [SerializeField] private CanvasGroup aktionListP1;
    [SerializeField] private TMP_Text battleBarTextP1;

    [SerializeField] private int aktionListIndexP1;
    [SerializeField] private Aktion currentAktionP1;
    [SerializeField] private List<GameObject> aktionUIListP1;
    [SerializeField] private RectTransform selectionArrowP1;
    public int GetP1ListIndex() { return aktionListIndexP1; }

    [Header("Player 2")]
    [SerializeField] private Canvas canvasP2;
    [SerializeField] private CanvasGroup aktionListP2;
    [SerializeField] private TMP_Text battleBarTextP2;

    [SerializeField] private int aktionListIndexP2;
    [SerializeField] private Aktion currentAktionP2;
    [SerializeField] private List<GameObject> aktionUIListP2;
    [SerializeField] private RectTransform selectionArrowP2;
    public int GetP2ListIndex() { return aktionListIndexP2; }

    [Header("HUD")]
    [SerializeField] private GameObject aktionSlotUIPrefab;
    [SerializeField] private Canvas canvasIntro;
    [SerializeField] private Image readyImage;
    [SerializeField] private Image fightImage;

    private PlayerManager playerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(PlayerManager pm)
    {
        playerManager = pm;

        StartCoroutine(IntroSequence());

        for(int i = 0; i < aktionListP1.transform.childCount; i++)
        {
            aktionUIListP1.Add(aktionListP1.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < aktionListP2.transform.childCount; i++)
        {
            aktionUIListP2.Add(aktionListP2.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        BattleBarTextChange(BattleManager.instance.GetCurrState());

        AktionListUpdate();

        UpdateAktionListArrow();
    }

    void UpdateAktionListArrow()
    {
        if (BattleManager.instance.GetCurrState() == BattleStates.P1turn)
        {
            if (playerManager.GetPlayer1().GetNavigateInput() == Vector2.up)
            {
                aktionListIndexP1 -= 1;
                aktionListIndexP1 = Mathf.Clamp(aktionListIndexP1, 0, aktionUIListP1.Count - 1);
            }
            else if (playerManager.GetPlayer1().GetNavigateInput() == Vector2.down)
            {
                aktionListIndexP1 += 1;
                aktionListIndexP1 = Mathf.Clamp(aktionListIndexP1, 0, aktionUIListP1.Count - 1);
            }
            selectionArrowP1.SetParent(aktionUIListP1[aktionListIndexP1].GetComponent<RectTransform>());
            selectionArrowP1.anchoredPosition = new Vector2(700, 0);
        }
        else if (BattleManager.instance.GetCurrState() == BattleStates.P2turn)
        {
            if (playerManager.GetPlayer2().GetNavigateInput() == Vector2.up)
            {
                aktionListIndexP2 -= 1;
                aktionListIndexP2 = Mathf.Clamp(aktionListIndexP2, 0, aktionUIListP2.Count - 1);
            }
            else if (playerManager.GetPlayer2().GetNavigateInput() == Vector2.down)
            {
                aktionListIndexP2 += 1;
                aktionListIndexP2 = Mathf.Clamp(aktionListIndexP2, 0, aktionUIListP2.Count - 1);
            }
            selectionArrowP2.SetParent(aktionUIListP2[aktionListIndexP2].GetComponent<RectTransform>());
            selectionArrowP2.anchoredPosition = new Vector2(-100, 0);

        }
    }

    void AktionListUpdate()
    {
        if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.P1turn)
        {
            ActivateAktionLists(1, 0);

        }
        else if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.P2turn)
        {
            ActivateAktionLists(0, 1);
        }
        else if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.P2attack || BattleManager.instance.GetCurrState() == BattleManager.BattleStates.P1attack)
        {
            ActivateAktionLists(0, 0);
        }
    }

    public void ActivateAktionLists(int p1, int p2)
    {
        aktionListP1.gameObject.SetActive(p1 == 1);
        aktionListP1.DOFade(p1, 1f);

        aktionListP2.gameObject.SetActive(p2 == 1);
        aktionListP2.DOFade(p2, 1f);
    }

    public void BattleBarTextChange(BattleStates whosAttacking)
    {

        if(whosAttacking == BattleStates.P1attack)
        {
            battleBarTextP1.text = "Attack!";
            battleBarTextP2.text = "Defend!";
        }
        else if(whosAttacking == BattleStates.P2attack)
        {
            battleBarTextP2.text = "Attack!";
            battleBarTextP1.text = "Defend!";
        }
    }

    IEnumerator IntroSequence()
    {
        canvasIntro.gameObject.SetActive(true);
        fightImage.gameObject.SetActive(false);
        readyImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);
        
        fightImage.gameObject.SetActive(true);
        readyImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        canvasIntro.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();

        BattleManager.instance.OnFirstTurn();
    }
}

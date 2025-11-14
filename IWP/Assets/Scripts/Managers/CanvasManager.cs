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
    [Header("Player 1 Status Bar")]
    [SerializeField] private Slider healthBarP1;
    [SerializeField] private Slider manaBarP1;

    [Header("Player 1 Aktion List")]
    [SerializeField] private Canvas canvasP1;
    [SerializeField] private TMP_Text battleBarTextP1;

    [SerializeField] private CanvasGroup aktionListP1;
    [SerializeField] private int aktionListIndexP1;
    [SerializeField] private Aktion currentAktionP1;
    [SerializeField] private List<GameObject> aktionUIListP1;
    public Aktion GetP1Aktion() {  return currentAktionP1; }
    public int GetP1ListIndex() { return aktionListIndexP1; }

    [Header("Player 2 Status Bar")]
    [SerializeField] private Slider healthBarP2;
    [SerializeField] private Slider manaBarP2;

    [Header("Player 2 Aktion List")]
    [SerializeField] private Canvas canvasP2;
    [SerializeField] private TMP_Text battleBarTextP2;

    [SerializeField] private CanvasGroup aktionListP2;
    [SerializeField] private int aktionListIndexP2;
    [SerializeField] private Aktion currentAktionP2;
    [SerializeField] private List<GameObject> aktionUIListP2;
    public Aktion GetP2Aktion() { return currentAktionP2; }
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

    public void OnFirstTurn()
    {
        healthBarP1.value = healthBarP1.maxValue = playerManager.GetPlayer1().GetOriginalHealth();
        manaBarP1.value = manaBarP1.maxValue = playerManager.GetPlayer1().GetOriginalMana();

        healthBarP2.value = healthBarP2.maxValue = playerManager.GetPlayer2().GetOriginalHealth();
        manaBarP2.value = manaBarP2.maxValue = playerManager.GetPlayer2().GetOriginalMana();

        SetAktionList();
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        BattleBarTextChange(BattleManager.instance.GetCurrState());

        AktionListUpdate();

        UpdateSelectionAktionList();
    }

    public void UpdatePlayerBars(Character player)
    {
        if (player == playerManager.GetPlayer1())
        {
            healthBarP1.DOValue(player.GetHealth(), 0.5f);
            manaBarP1.DOValue(player.GetMana(), 0.5f);
        }
        else if (player == playerManager.GetPlayer2())
        {
            healthBarP2.DOValue(player.GetHealth(), 0.5f);
            manaBarP2.DOValue(player.GetMana(), 0.5f);
        }
    }

    void SetAktionList()
    {
        // player 1
        for (int i = playerManager.GetPlayer1().GetAktionList().Count - 1; i >= 0; i--)
        {
            GameObject go = Instantiate(aktionSlotUIPrefab);

            go.transform.SetParent(aktionListP1.transform);

            go.transform.localScale = Vector3.one;
            aktionUIListP1.Add(go);

            AktionSlotUI ui = go.GetComponent<AktionSlotUI>();
            ui.SetText(playerManager.GetPlayer1().GetAktion(i));

            Debug.Log("Penis 1 added " + go.name);
        }

        //player 2
        for (int x = playerManager.GetPlayer2().GetAktionList().Count - 1; x >= 0; x--)
        {
            GameObject go2 = Instantiate(aktionSlotUIPrefab);

            go2.transform.SetParent(aktionListP2.transform);

            go2.transform.localScale = Vector3.one;
            aktionUIListP2.Add(go2);

            AktionSlotUI ui2 = go2.GetComponent<AktionSlotUI>();
            ui2.SetText(playerManager.GetPlayer2().GetAktion(x));

            Debug.Log("Penis 2 added " + go2.name);
        }
    }

    void UpdateSelectionAktionList()
    {
        if (BattleManager.instance.GetCurrState() == BattleStates.P1turn)
        {
            if (playerManager.GetPlayer1().GetNavigateInput() == Vector2.up)
            {
                aktionListIndexP1 -= 1;
                aktionListIndexP1 = Mathf.Clamp(aktionListIndexP1, 0, aktionUIListP1.Count - 1);
                aktionUIListP1[aktionListIndexP1 + 1].transform.DOScale(Vector3.one, 1f);

            }
            else if (playerManager.GetPlayer1().GetNavigateInput() == Vector2.down)
            {
                aktionListIndexP1 += 1;
                aktionListIndexP1 = Mathf.Clamp(aktionListIndexP1, 0, aktionUIListP1.Count - 1);
                aktionUIListP1[aktionListIndexP1 - 1].transform.DOScale(Vector3.one, 1f);

            }
            aktionUIListP1[aktionListIndexP1].transform.DOScale(Vector3.one * .75f, 1f);

            currentAktionP1 = playerManager.GetPlayer1().GetAktion(aktionListIndexP1);
        }
        else if (BattleManager.instance.GetCurrState() == BattleStates.P2turn)
        {
            if (playerManager.GetPlayer2().GetNavigateInput() == Vector2.up)
            {
                aktionListIndexP2 -= 1;
                aktionListIndexP2 = Mathf.Clamp(aktionListIndexP2, 0, aktionUIListP2.Count - 1);
                aktionUIListP2[aktionListIndexP2 + 1].transform.DOScale(Vector3.one, 1f);

            }
            else if (playerManager.GetPlayer2().GetNavigateInput() == Vector2.down)
            {
                aktionListIndexP2 += 1;
                aktionListIndexP2 = Mathf.Clamp(aktionListIndexP2, 0, aktionUIListP2.Count - 1);
                aktionUIListP2[aktionListIndexP2 - 1].transform.DOScale(Vector3.one, 1f);

            }
            aktionUIListP2[aktionListIndexP2].transform.DOScale(Vector3.one * .75f, 1f);

            currentAktionP2 = playerManager.GetPlayer2().GetAktion(aktionListIndexP2);
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

        if (p1 == 0)
        {
            foreach (GameObject t in aktionUIListP1)
            {
                t.transform.localScale = Vector3.one;
            }
        }

        aktionListP2.gameObject.SetActive(p2 == 1);
        aktionListP2.DOFade(p2, 1f);
        if (p2 == 0)
        {
            foreach (GameObject t in aktionUIListP2)
            {
                t.transform.localScale = Vector3.one;
            }
        }
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

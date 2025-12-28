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
    [SerializeField] private TMP_Text healthPointsP1;
    [SerializeField] private Slider apBarP1;

    [Header("Player 1 Aktion List")]
    [SerializeField] private Canvas canvasP1;
    [SerializeField] private TMP_Text battleBarTextP1;

    [SerializeField] private CanvasGroup aktionListP1;
    [SerializeField] private int aktionListIndexP1;
    [SerializeField] private Aktion currentAktionP1;
    [SerializeField] private List<GameObject> aktionUIListP1;
    public Aktion GetP1Aktion() {  return currentAktionP1; }
    public int GetP1ListIndex() { return aktionListIndexP1; }

    [SerializeField] private FloatingTextUI damageNumberUIP1;
    [SerializeField] private HorizontalLayoutGroup winIconsP1;
    [SerializeField] private List<GameObject> winIconsListP1;

    [Header("Player 2 Status Bar")]
    [SerializeField] private Slider healthBarP2; 
    [SerializeField] private TMP_Text healthPointsP2;
    [SerializeField] private Slider apBarP2;

    [Header("Player 2 Aktion List")]
    [SerializeField] private Canvas canvasP2;
    [SerializeField] private TMP_Text battleBarTextP2;

    [SerializeField] private CanvasGroup aktionListP2;
    [SerializeField] private int aktionListIndexP2;
    [SerializeField] private Aktion currentAktionP2;
    [SerializeField] private List<GameObject> aktionUIListP2;
    public Aktion GetP2Aktion() { return currentAktionP2; }
    public int GetP2ListIndex() { return aktionListIndexP2; }

    [SerializeField] private FloatingTextUI damageNumberUIP2;
    [SerializeField] private HorizontalLayoutGroup winIconsP2;
    [SerializeField] private List<GameObject> winIconsListP2;

    [Header("HUD")]
    [SerializeField] private GameObject aktionSlotUIPrefab;
    [SerializeField] private Canvas canvasIntro;
    [SerializeField] private Image readyImage;
    [SerializeField] private Image fightImage;
    [SerializeField] private TMP_Text turnCounterText;

    private PlayerManager playerManager;
    private EnhancementManager enhancementManager;
    private BattleBarManager barManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(PlayerManager pm, EnhancementManager em, BattleBarManager bm)
    {
        playerManager = pm;
        enhancementManager = em;
        barManager = bm;

        for (int i = 0; i < aktionListP1.transform.childCount; i++)
        {
            aktionUIListP1.Add(aktionListP1.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < aktionListP2.transform.childCount; i++)
        {
            aktionUIListP2.Add(aktionListP2.transform.GetChild(i).gameObject);
        }

        //for (int i = 0)

        SetAktionList();
        SetIcons();
    }

    public void OnFirstTurn()
    {
        healthBarP1.value = healthBarP1.maxValue = playerManager.GetPlayer1().GetOriginalHealth();
        apBarP1.value = apBarP1.maxValue = playerManager.GetPlayer1().GetOriginalAP();

        healthBarP2.value = healthBarP2.maxValue = playerManager.GetPlayer2().GetOriginalHealth();
        apBarP2.value = apBarP2.maxValue = playerManager.GetPlayer2().GetOriginalAP();

        healthPointsP1.text = $"{Mathf.Max(playerManager.GetPlayer1().GetHealth(), 0)}/{playerManager.GetPlayer1().GetOriginalHealth()}";
        healthPointsP2.text = $"{Mathf.Max(playerManager.GetPlayer2().GetHealth(), 0)}/{playerManager.GetPlayer2().GetOriginalHealth()}";

    }

    // Update is called once per frame
    public void OnUpdate()
    {
        turnCounterText.text = $"Turn: {BattleManager.instance.GetTurnCounter()}";

        BattleBarTextChange(BattleManager.instance.GetCurrState());

        AktionListUpdate();

        UpdateSelectionAktionList();

    }

    #region BattleBars
    int p1hp;
    int p2hp;
    public void UpdatePlayerBars(Character player)
    {
        if (player == playerManager.GetPlayer1())
        {
            healthBarP1.DOValue(player.GetHealth(), 0.5f);
            apBarP1.DOValue(player.GetAP(), 0.5f);

            healthPointsP1.text = $"{Mathf.Max(player.GetHealth(), 0)}/{player.GetOriginalHealth()}";
        }
        else if (player == playerManager.GetPlayer2())
        {
            healthBarP2.DOValue(player.GetHealth(), 0.5f);
            apBarP2.DOValue(player.GetAP(), 0.5f);

            healthPointsP2.text = $"{Mathf.Max(player.GetHealth(), 0)}/{player.GetOriginalHealth()}";
        }
    }

    public void BattleBarTextChange(BattleStates whosAttacking)
    {

        if (whosAttacking == BattleStates.P1attack)
        {
            battleBarTextP1.text = "Attack!";
            battleBarTextP2.text = "Defend!";
        }
        else if (whosAttacking == BattleStates.P2attack)
        {
            battleBarTextP2.text = "Attack!";
            battleBarTextP1.text = "Defend!";
        }
    }
    #endregion

    #region AktionList
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
        }

        aktionUIListP1.Reverse();

        //player 2
        for (int x = playerManager.GetPlayer2().GetAktionList().Count - 1; x >= 0; x--)
        {

            GameObject go2 = Instantiate(aktionSlotUIPrefab);

            go2.transform.SetParent(aktionListP2.transform);

            go2.transform.localScale = Vector3.one;
            aktionUIListP2.Add(go2);

            AktionSlotUI ui2 = go2.GetComponent<AktionSlotUI>();
            ui2.SetText(playerManager.GetPlayer2().GetAktion(x));
        }

        aktionUIListP2.Reverse();

    }

    public void ResetList()
    {
        Debug.Log("Resetting List...");

        foreach (GameObject go in aktionUIListP1) { Destroy(go); }
        aktionUIListP1.Clear();
        foreach (GameObject go in aktionUIListP2) { Destroy(go); }
        aktionUIListP2.Clear();
        SetAktionList();
    }

    private Vector2 previousInputP1 = Vector2.zero;
    private Vector2 previousInputP2 = Vector2.zero;

    void UpdateSelectionAktionList()
    {
        if (BattleManager.instance.GetCurrState() == BattleStates.P1turn)
        {
            Vector2 currentInputP1 = playerManager.GetPlayer1().GetNavigateInput();

            // Only trigger when input JUST changed from zero to up/down
            if (currentInputP1 == Vector2.up && previousInputP1 != Vector2.up)
            {
                aktionListIndexP1 -= 1;
                aktionListIndexP1 = Mathf.Clamp(aktionListIndexP1, 0, aktionUIListP1.Count - 1);
            }
            else if (currentInputP1 == Vector2.down && previousInputP1 != Vector2.down)
            {
                aktionListIndexP1 += 1;
                aktionListIndexP1 = Mathf.Clamp(aktionListIndexP1, 0, aktionUIListP1.Count - 1);
            }

            aktionUIListP1[aktionListIndexP1].transform.DOScale(Vector3.one, 0.2f);
            foreach (GameObject go in aktionUIListP1)
            {
                if (go != aktionUIListP1[aktionListIndexP1]) go.transform.DOScale(Vector3.one * 0.75f, 0.2f);
            }

            currentAktionP1 = playerManager.GetPlayer1().GetAktion(aktionListIndexP1);

            // Store current input for next frame
            previousInputP1 = currentInputP1;

            if (playerManager.GetPlayer1().IsConfirmPressed())
            {
                if (currentAktionP1 as AttackAktion != null)
                {
                    BattleManager.instance.ActivateBattleBarState();
                    barManager.ActivateBattleBars(true);
                }
                else
                {
                    BattleManager.instance.EnterStatusAktionState(1);
                }
            }
        }
        else if (BattleManager.instance.GetCurrState() == BattleStates.P2turn)
        {
            Vector2 currentInputP2 = playerManager.GetPlayer2().GetNavigateInput();

            // Only trigger when input JUST changed from zero to up/down
            if (currentInputP2 == Vector2.up && previousInputP2 != Vector2.up)
            {
                aktionListIndexP2 -= 1;
                aktionListIndexP2 = Mathf.Clamp(aktionListIndexP2, 0, aktionUIListP2.Count - 1);
            }
            else if (currentInputP2 == Vector2.down && previousInputP2 != Vector2.down)
            {
                aktionListIndexP2 += 1;
                aktionListIndexP2 = Mathf.Clamp(aktionListIndexP2, 0, aktionUIListP2.Count - 1);
            }

            aktionUIListP2[aktionListIndexP2].transform.DOScale(Vector3.one, 0.2f);
            foreach (GameObject go in aktionUIListP2)
            {
                if (go != aktionUIListP2[aktionListIndexP2]) go.transform.DOScale(Vector3.one * 0.75f, 0.2f);
            }

            currentAktionP2 = playerManager.GetPlayer2().GetAktion(aktionListIndexP2);

            // Store current input for next frame
            previousInputP2 = currentInputP2;

            if (playerManager.GetPlayer2().IsConfirmPressed())
            {
                if (currentAktionP2 as AttackAktion != null)
                {
                    BattleManager.instance.ActivateBattleBarState();
                    barManager.ActivateBattleBars(true);
                }
                else
                {
                    BattleManager.instance.EnterStatusAktionState(2);
                }
            }
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
        else
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
    #endregion

    #region Win Icons
    public void SetIcons()
    {
        for (int i = 0; i < winIconsP1.transform.childCount; i++)
        {
            winIconsListP1.Add(winIconsP1.transform.GetChild(i).gameObject);
            winIconsListP1[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < winIconsP2.transform.childCount; i++)
        {
            winIconsListP2.Add(winIconsP2.transform.GetChild(i).gameObject);
            winIconsListP2[i].gameObject.SetActive(false);
        }
    }

    public void UpdateWinIcons(int i)
    {
        if(i == 1)
        {
            winIconsListP1[BattleManager.instance.GetP1Wins() - 1].gameObject.SetActive(true);
        }
        else if(i == 2)
        {
            winIconsListP2[BattleManager.instance.GetP2Wins() - 1].gameObject.SetActive(true);
        }
    }

    #endregion
    public void ActivateDamageNumber(int i, int damage)
    {
        if (i == 1)
        {
            damageNumberUIP1.SetText(damage.ToString(), 3f);
        }
        else if(i == 2) 
        {
            damageNumberUIP2.SetText(damage.ToString(), 3f);
        }
    }

    public IEnumerator IntroSequence()
    {
        canvasIntro.gameObject.SetActive(true);
        fightImage.gameObject.SetActive(false);
        readyImage.gameObject.SetActive(true);

        OnFirstTurn();

        yield return new WaitForSeconds(2);
        
        fightImage.gameObject.SetActive(true);
        readyImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        fightImage.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();

        BattleManager.instance.OnFirstTurn();
    }
}

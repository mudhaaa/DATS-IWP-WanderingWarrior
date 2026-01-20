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
    [SerializeField] private TMP_Text aktionPointsP1;

    [Header("Player 1 Aktion List")]
    [SerializeField] private Canvas canvasP1;
    [SerializeField] private TMP_Text battleBarTextP1;

    [SerializeField] private CanvasGroup aktionListP1;
    [SerializeField] private VerticalLayoutGroup vlgP1;
    [SerializeField] private RectTransform viewportP1;

    [SerializeField] private int aktionListIndexP1;
    [SerializeField] private Aktion currentAktionP1;
    [SerializeField] private List<RectTransform> aktionUIListP1;
    [SerializeField] private TMP_Text aktionDescP1;
    [SerializeField] private RectTransform aktionArrowP1;

    public Aktion GetP1Aktion() {  return currentAktionP1; }
    public int GetP1ListIndex() { return aktionListIndexP1; }

    [Header("P1 Status Changes")]
    [SerializeField] private Image strengthTimerImageP1;
    [SerializeField] private TMP_Text strengthTimerTextP1;
    [SerializeField] private Image magicTimerImageP1;
    [SerializeField] private TMP_Text magicTimerTextP1;
    [SerializeField] private Image enduranceTimerImageP1;
    [SerializeField] private TMP_Text enduranceTimerTextP1;
    [SerializeField] private Image speedTimerImageP1;
    [SerializeField] private TMP_Text speedTimerTextP1;
    [SerializeField] private Image critTimerImageP1;
    [SerializeField] private TMP_Text critTimerTextP1;

    [Header("P1 Others")]
    [SerializeField] private FloatingTextUI damageNumberUIP1;
    [SerializeField] private HorizontalLayoutGroup winIconsP1;
    [SerializeField] private List<GameObject> winIconsListP1;
    [SerializeField] private TMP_Text turnUpdateP1;

    [Header("Player 2 Status Bar")]
    [SerializeField] private Slider healthBarP2; 
    [SerializeField] private TMP_Text healthPointsP2;
    [SerializeField] private Slider apBarP2;
    [SerializeField] private TMP_Text aktionPointsP2;

    [Header("Player 2 Aktion List")]
    [SerializeField] private Canvas canvasP2;
    [SerializeField] private TMP_Text battleBarTextP2;

    [SerializeField] private CanvasGroup aktionListP2;
    [SerializeField] private VerticalLayoutGroup vlgP2;
    [SerializeField] private RectTransform viewportP2;

    [SerializeField] private int aktionListIndexP2;
    [SerializeField] private Aktion currentAktionP2;
    [SerializeField] private List<RectTransform> aktionUIListP2;

    [SerializeField] private TMP_Text aktionDescP2;
    [SerializeField] private RectTransform aktionArrowP2;

    public Aktion GetP2Aktion() { return currentAktionP2; }
    public int GetP2ListIndex() { return aktionListIndexP2; }

    [Header("P2 Status Changes")]
    [SerializeField] private Image strengthTimerImageP2;
    [SerializeField] private TMP_Text strengthTimerTextP2;
    [SerializeField] private Image magicTimerImageP2;
    [SerializeField] private TMP_Text magicTimerTextP2;
    [SerializeField] private Image enduranceTimerImageP2;
    [SerializeField] private TMP_Text enduranceTimerTextP2;
    [SerializeField] private Image speedTimerImageP2;
    [SerializeField] private TMP_Text speedTimerTextP2;
    [SerializeField] private Image critTimerImageP2;
    [SerializeField] private TMP_Text critTimerTextP2;

    [Header("P2 Others")]
    [SerializeField] private FloatingTextUI damageNumberUIP2;
    [SerializeField] private HorizontalLayoutGroup winIconsP2;
    [SerializeField] private List<GameObject> winIconsListP2;
    [SerializeField] private TMP_Text turnUpdateP2;

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
            aktionUIListP1.Add(aktionListP1.transform.GetChild(i).GetComponent<RectTransform>());
        }
        for (int i = 0; i < aktionListP2.transform.childCount; i++)
        {
            aktionUIListP2.Add(aktionListP2.transform.GetChild(i).GetComponent<RectTransform>());
        }

        //for (int i = 0)

        SetAktionList();
        SetIcons();
        UpdateStatusChangeUI(1);
        UpdateStatusChangeUI(2);
    }

    public void OnFirstTurn()
    {
        healthBarP1.value = healthBarP1.maxValue = playerManager.GetPlayer1().GetOriginalHealth();
        apBarP1.value = playerManager.GetPlayer1().GetOriginalAP();
        apBarP1.maxValue = 10;

        healthBarP2.value = healthBarP2.maxValue = playerManager.GetPlayer2().GetOriginalHealth();
        apBarP2.value = playerManager.GetPlayer2().GetOriginalAP();
        apBarP2.maxValue = 10;

        healthPointsP1.text = $"{Mathf.Max(playerManager.GetPlayer1().GetHealth(), 0)}/{playerManager.GetPlayer1().GetOriginalHealth()}";
        healthPointsP2.text = $"{Mathf.Max(playerManager.GetPlayer2().GetHealth(), 0)}/{playerManager.GetPlayer2().GetOriginalHealth()}";

        aktionPointsP1.text = $"{Mathf.Max(playerManager.GetPlayer1().GetAP(), 0)}";
        aktionPointsP2.text = $"{Mathf.Max(playerManager.GetPlayer2().GetAP(), 0)}";

        UpdatePlayerBars(playerManager.GetPlayer1());
        UpdatePlayerBars(playerManager.GetPlayer2());

        UpdateStatusChangeUI(1);
        UpdateStatusChangeUI(2);
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        turnCounterText.text = $"Turn: {BattleManager.instance.GetTurnCounter()}";

        BattleBarTextChange(BattleManager.instance.GetCurrState());

        AktionListUpdate();

        UpdateSelectionAktionList();

    }

    #region Bars
    int p1hp;
    int p2hp;
    public void UpdatePlayerBars(Character player)
    {
        if (player == playerManager.GetPlayer1())
        {
            healthBarP1.DOValue(player.GetHealth(), 0.5f);
            apBarP1.DOValue(player.GetAP(), 0.5f);

            healthPointsP1.text = $"{Mathf.Max(player.GetHealth(), 0)}/{player.GetOriginalHealth()}";
            aktionPointsP1.text = $"{Mathf.Max(playerManager.GetPlayer1().GetAP(), 0)}";
        }
        else if (player == playerManager.GetPlayer2())
        {
            healthBarP2.DOValue(player.GetHealth(), 0.5f);
            apBarP2.DOValue(player.GetAP(), 0.5f);

            healthPointsP2.text = $"{Mathf.Max(player.GetHealth(), 0)}/{player.GetOriginalHealth()}";
            aktionPointsP2.text = $"{Mathf.Max(playerManager.GetPlayer2().GetAP(), 0)}";
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

            RectTransform rt = go.GetComponent<RectTransform>();
            aktionUIListP1.Add(rt);

            AktionSlotUI ui = go.GetComponent<AktionSlotUI>();
            ui.SetText(playerManager.GetPlayer1().GetAktion(i));
        }

        aktionUIListP1.Reverse();
        aktionArrowP1.SetParent(aktionUIListP1[0].transform);
        aktionArrowP1.DOAnchorPos(new Vector3(60, 0, 0), 0.1f);

        //player 2
        for (int x = playerManager.GetPlayer2().GetAktionList().Count - 1; x >= 0; x--)
        {

            GameObject go2 = Instantiate(aktionSlotUIPrefab);

            go2.transform.SetParent(aktionListP2.transform);

            go2.transform.localScale = Vector3.one;

            RectTransform rt = go2.GetComponent<RectTransform>();
            aktionUIListP2.Add(rt);

            AktionSlotUI ui2 = go2.GetComponent<AktionSlotUI>();
            ui2.SetText(playerManager.GetPlayer2().GetAktion(x));
        }

        aktionUIListP2.Reverse();
        aktionArrowP2.SetParent(aktionUIListP2[0].transform);
        aktionArrowP2.DOAnchorPos(new Vector3(60, 0, 0), 0.1f);

    }

    public void ResetList()
    {
        Debug.Log("Resetting List...");

        foreach (RectTransform go in aktionUIListP1) { Destroy(go.gameObject); }
        aktionUIListP1.Clear();
        foreach (RectTransform go in aktionUIListP2) { Destroy(go.gameObject); }
        aktionUIListP2.Clear();
        SetAktionList();

        //ScrollToIndex(0, true);
        //ScrollToIndex(0, false);
    }

    private Vector2 previousInputP1 = Vector2.zero;
    private Vector2 previousInputP2 = Vector2.zero;

    public bool reachedTopP1 = true;
    public bool reachedBotP1 = false;

    public bool reachedTopP2 = true;
    public bool reachedBotP2 = false;

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

                // Arrow move to selected Aktion
                aktionArrowP1.SetParent(aktionUIListP1[aktionListIndexP1].transform);
                aktionArrowP1.DOAnchorPos(new Vector3(60, 0, 0), 0.1f);

                if (aktionListIndexP1 >= 0 && !reachedTopP1)
                {
                    if (aktionListIndexP1 == 0) reachedTopP1 = true;
                    reachedBotP1 = false;
                }
            }
            else if (currentInputP1 == Vector2.down && previousInputP1 != Vector2.down)
            {
                aktionListIndexP1 += 1;
                aktionListIndexP1 = Mathf.Clamp(aktionListIndexP1, 0, aktionUIListP1.Count - 1);

                // Arrow move to selected Aktion
                aktionArrowP1.SetParent(aktionUIListP1[aktionListIndexP1].transform);
                aktionArrowP1.DOAnchorPos(new Vector3(60, 0, 0), 0.1f);

                if (aktionListIndexP1 <= aktionListP1.transform.childCount && !reachedBotP1)
                {
                    if (aktionListIndexP1 == aktionListP1.transform.childCount - 1) reachedBotP1 = true;
                    reachedTopP1 = false;
                }
            }


            // Set Description
            currentAktionP1 = playerManager.GetPlayer1().GetAktion(aktionListIndexP1);
            aktionDescP1.text = currentAktionP1.GetDesc();

            // Store current input for next frame
            previousInputP1 = currentInputP1;

            if (playerManager.GetPlayer1().IsConfirmPressed())
            {
                if (currentAktionP1.GetAPCost() <= playerManager.GetPlayer1().GetAP())
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
                else
                {
                    ActivateDamageNumber(1, " Not enough AP!");
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

                // Arrow move to selected Aktion
                aktionArrowP2.SetParent(aktionUIListP2[aktionListIndexP2].transform);
                aktionArrowP2.DOAnchorPos(new Vector3(-60, 0, 0), 0.1f);

                if (aktionListIndexP2 >= 0 && !reachedTopP2)
                {
                    if (aktionListIndexP2 == 0) reachedTopP2 = true;
                    reachedBotP2 = false;
                }
            }
            else if (currentInputP2 == Vector2.down && previousInputP2 != Vector2.down)
            {
                aktionListIndexP2 += 1;
                aktionListIndexP2 = Mathf.Clamp(aktionListIndexP2, 0, aktionUIListP2.Count - 1);

                // Arrow move to selected Aktion
                aktionArrowP2.SetParent(aktionUIListP2[aktionListIndexP2].transform);
                aktionArrowP2.DOAnchorPos(new Vector3(-60, 0, 0), 0.1f);

                if (aktionListIndexP2 <= aktionListP2.transform.childCount && !reachedBotP2)
                {
                    if (aktionListIndexP2 == aktionListP2.transform.childCount - 1) reachedBotP2 = true;
                    reachedTopP2 = false;
                }
            }


            // Set Description
            currentAktionP2 = playerManager.GetPlayer2().GetAktion(aktionListIndexP2);

            aktionDescP2.text = currentAktionP2.GetDesc();

            // Store current input for next frame
            previousInputP2 = currentInputP2;

            if (playerManager.GetPlayer2().IsConfirmPressed())
            {
                if (currentAktionP2.GetAPCost() <= playerManager.GetPlayer2().GetAP())
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
                else
                {
                    ActivateDamageNumber(2, " Not enough AP!");
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
        viewportP1.gameObject.SetActive(p1 == 1);
        aktionListP1.DOFade(p1, 1f);
        aktionArrowP1.gameObject.SetActive(p1 == 1);
        aktionArrowP1.GetComponent<Image>().enabled = p1 == 1;

        viewportP2.gameObject.SetActive(p2 == 1);
        aktionListP2.DOFade(p2, 1f);
        aktionArrowP2.gameObject.SetActive(p2 == 1);
        aktionArrowP2.GetComponent<Image>().enabled = p2 == 1;

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

    #region Turn Update 
    public IEnumerator TurnUpdates(int n)
    {
        if(n == 2)playerManager.GetPlayer1().DownStatChangeTimers();
        if(n == 1)playerManager.GetPlayer2().DownStatChangeTimers();

        UpdateStatusChangeUI(1);
        UpdateStatusChangeUI(2);

        Debug.Log("Activating Turn Updates");

        if (playerManager.GetPlayer1().GetTurnUpdateLists().Count > 0)
        {
            turnUpdateP1.gameObject.transform.parent.gameObject.SetActive(true);

            for (int i = 0; i < playerManager.GetPlayer1().GetTurnUpdateLists().Count; i++)
            {
                turnUpdateP1.text = playerManager.GetPlayer1().GetTurnUpdateLists()[i];
                yield return new WaitForSeconds(2);
                turnUpdateP1.text = "";
                yield return new WaitForSeconds(.5f);

            }
        }

        turnUpdateP1.gameObject.transform.parent.gameObject.SetActive(false);

        yield return null;

        if (playerManager.GetPlayer2().GetTurnUpdateLists().Count > 0)
        {
            turnUpdateP2.gameObject.transform.parent.gameObject.SetActive(true);

            for (int i = 0; i < playerManager.GetPlayer2().GetTurnUpdateLists().Count; i++)
            {
                turnUpdateP2.text = playerManager.GetPlayer2().GetTurnUpdateLists()[i];
                yield return new WaitForSeconds(2);
                turnUpdateP2.text = "";
                yield return new WaitForSeconds(.5f);

            }
        }

        turnUpdateP2.gameObject.transform.parent.gameObject.SetActive(false);

        yield return null;

        playerManager.GetPlayer1().GetTurnUpdateLists().Clear();
        playerManager.GetPlayer2().GetTurnUpdateLists().Clear();
        BattleManager.instance.EndOfTurn(n);

        yield return null;

    }
    #endregion

    #region Status Changes
    public void UpdateStatusChangeUI(int i)
    {
        UpdateStrengthTimerUI(i);
        UpdateMagicTimerUI(i);
        UpdateEnduranceTimerUI(i);
        UpdateSpeedTimerUI(i);
        UpdateCritTimerUI(i);
    }

    void UpdateStrengthTimerUI(int i)
    {
        Character player = i == 1 ? playerManager.GetPlayer1() : playerManager.GetPlayer2();
        Image image = i == 1 ? strengthTimerImageP1 : strengthTimerImageP2;
        TMP_Text text = i == 1 ? strengthTimerTextP1 : strengthTimerTextP2;

        // Check if stat is buffed or nerfed
        bool strengthBuffed = player.GetStrengthBuffTimer() > 0;
        bool strengthNerfed = player.GetStrengthNerfTimer() > 0;

        // Activate image if either is true
        GameObject imageParent = image.transform.parent.gameObject;
        imageParent.SetActive(strengthBuffed || strengthNerfed);

        // If buffed, green colour, else red
        if (strengthNerfed)
        {
            image.color = Color.red;
            // Update timer
            text.text = player.GetStrengthNerfTimer().ToString();
        }
        else if (strengthBuffed)
        {
            image.color = Color.green;
            // Update timer
            text.text = player.GetStrengthBuffTimer().ToString();
        }
    }
    void UpdateMagicTimerUI(int i)
    {
        Character player = i == 1? playerManager.GetPlayer1() : playerManager.GetPlayer2();
        Image image = i == 1 ? magicTimerImageP1 : magicTimerImageP2;
        TMP_Text text = i == 1 ? magicTimerTextP1 : magicTimerTextP2;

        // Check if stat is buffed or nerfed
        bool magicBuffed = player.GetMagicBuffTimer() > 0;
        bool magicNerfed = player.GetMagicNerfTimer() > 0;

        // Activate image if either is true
        GameObject imageParent = image.transform.parent.gameObject;
        imageParent.SetActive(magicBuffed || magicNerfed);

        // If buffed, green colour, else red
        if (magicNerfed)
        {
            image.color = Color.red;
            // Update timer
            text.text = player.GetMagicNerfTimer().ToString();
        }
        else if (magicBuffed)
        {
            image.color = Color.green;
            // Update timer
            text.text = player.GetMagicBuffTimer().ToString();
        }
    }
    void UpdateEnduranceTimerUI(int i)
    {
        Character player = i == 1? playerManager.GetPlayer1() : playerManager.GetPlayer2();
        Image image = i == 1 ? enduranceTimerImageP1 : enduranceTimerImageP2;
        TMP_Text text = i == 1 ? enduranceTimerTextP1 : enduranceTimerTextP2;

        // Check if stat is buffed or nerfed
        bool enduranceBuffed = player.GetEnduranceBuffTimer() > 0;
        bool enduranceNerfed = player.GetEnduranceNerfTimer() > 0;

        // Activate image if either is true
        GameObject imageParent = image.transform.parent.gameObject;
        imageParent.SetActive(enduranceBuffed || enduranceNerfed);

        // If buffed, green colour, else red
        if (enduranceNerfed)
        {
            image.color = Color.red;
            // Update timer
            text.text = player.GetEnduranceNerfTimer().ToString();
        }
        else if (enduranceBuffed)
        {
            image.color = Color.green;
            // Update timer
            text.text = player.GetEnduranceBuffTimer().ToString();
        }
    }
    void UpdateSpeedTimerUI(int i)
    {
        Character player = i == 1? playerManager.GetPlayer1() : playerManager.GetPlayer2();
        Image image = i == 1 ? speedTimerImageP1 : speedTimerImageP2;
        TMP_Text text = i == 1 ? speedTimerTextP1 : speedTimerTextP2;

        // Check if stat is buffed or nerfed
        bool speedBuffed = player.GetSpeedBuffTimer() > 0;
        bool speedNerfed = player.GetSpeedNerfTimer() > 0;

        // Activate image if either is true
        GameObject imageParent = image.transform.parent.gameObject;
        imageParent.SetActive(speedBuffed || speedNerfed);

        // If buffed, green colour, else red
        if (speedNerfed)
        {
            image.color = Color.red;
            // Update timer
            text.text = player.GetSpeedNerfTimer().ToString();
        }
        else if (speedBuffed)
        {
            image.color = Color.green;
            // Update timer
            text.text = player.GetSpeedBuffTimer().ToString();
        }
    }
    void UpdateCritTimerUI(int i)
    {
        Character player = i == 1? playerManager.GetPlayer1() : playerManager.GetPlayer2();
        Image image = i == 1 ? critTimerImageP1 : critTimerImageP2;
        TMP_Text text = i == 1 ? critTimerTextP1 : critTimerTextP2;

        // Check if stat is buffed or nerfed
        bool critBuffed = player.GetCritBuffTimer() > 0;
        bool critNerfed = player.GetCritNerfTimer() > 0;

        // Activate image if either is true
        GameObject imageParent = image.transform.parent.gameObject;
        imageParent.SetActive(critBuffed || critNerfed);

        // If buffed, green colour, else red
        if (critNerfed)
        {
            image.color = Color.red;
            // Update timer
            text.text = player.GetCritNerfTimer().ToString();
        }
        else if (critBuffed)
        {
            image.color = Color.green;
            // Update timer
            text.text = player.GetCritBuffTimer().ToString();
        }
    }
    #endregion

    #region Victory
    [Header("Victory Screen")]
    [SerializeField] private CanvasGroup victoryP1;
    [SerializeField] private CanvasGroup victoryP2;
    public void ActivateVictoryUI(int i)
    {
        if (i == 1) victoryP1.DOFade(1, 0.1f); victoryP1.gameObject.SetActive(true);
        if (i == 2) victoryP2.DOFade(1, 0.1f); victoryP2.gameObject.SetActive(true);
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

    public void ActivateDamageNumber(int i, string str)
    {
        if (i == 1)
        {
            damageNumberUIP1.SetText(str, 1f);
        }
        else if(i == 2) 
        {
            damageNumberUIP2.SetText(str, 1f);
        }
    }

    public IEnumerator IntroSequence()
    {
        UpdatePlayerBars(playerManager.GetPlayer1());
        UpdatePlayerBars(playerManager.GetPlayer2());

        UpdateStatusChangeUI(1);
        UpdateStatusChangeUI(2);

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

using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancementManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainCanvas;

    [SerializeField] private List<Enhancement> enhancements;

    private PlayerManager playerManager;
    private CanvasManager canvasManager;

    [Header("Player 1")]
    [SerializeField] private HorizontalLayoutGroup uiChoicesGroupP1;
    [SerializeField] private List<EnhancementUI> uiChoicesP1;
    [SerializeField] private List<Enhancement> enhRandChoicesP1;
    [SerializeField] private int choiceIndexP1;
    [SerializeField] private Enhancement currChoiceP1;
    [SerializeField] private List<Enhancement> possibleChoicesP1;
    [SerializeField] private TMP_Text descriptionP1;
    [SerializeField] private TMP_Text aktionDescriptionP1;

    [Header("Player 2")]
    [SerializeField] private HorizontalLayoutGroup uiChoicesGroupP2;
    [SerializeField] private List<EnhancementUI> uiChoicesP2; // list of ui
    [SerializeField] private List<Enhancement> enhRandChoicesP2; // list of enhancement SO
    [SerializeField] private int choiceIndexP2;
    [SerializeField] private Enhancement currChoiceP2;
    [SerializeField] private List<Enhancement> possibleChoicesP2;
    [SerializeField] private TMP_Text descriptionP2;
    [SerializeField] private TMP_Text aktionDescriptionP2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(PlayerManager pm, CanvasManager cm)
    {
        playerManager = pm;
        canvasManager = cm;

        possibleChoicesP1 = new List<Enhancement>(enhancements);
        possibleChoicesP2 = new List<Enhancement>(enhancements);
        SetEnhancementChoices();

        for (int i = 0; i < uiChoicesGroupP1.transform.childCount; i++)
        {
            EnhancementUI ui = uiChoicesGroupP1.transform.GetChild(i).GetComponent<EnhancementUI>();
            if (ui != null)
            {
                uiChoicesP1.Add(ui);
            }
        }
        for (int i = 0; i < uiChoicesGroupP2.transform.childCount; i++)
        {
            EnhancementUI ui = uiChoicesGroupP2.transform.GetChild(i).GetComponent<EnhancementUI>();
            if (ui != null)
            {
                uiChoicesP2.Add(ui);
            }
        }

        SetUIList();

    }

    public void OnFirstTurn()
    {
        SetEnhancementChoices();
        SetUIList();
        p1HasPressed = false;
        p2HasPressed = false;
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        FadeEnhancementCanvas();
        if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.Enhancement)
        {
            UpdateSelectionEnhancements();
            GetInput();
        }
    }


    private Vector2 previousInputP1 = Vector2.zero;
    private Vector2 previousInputP2 = Vector2.zero;
    void UpdateSelectionEnhancements()
    {
        // player 1
        if (!p1HasPressed)
        {
            Vector2 currInputP1 = playerManager.GetPlayer1().GetNavigateInput();

            if (currInputP1 == Vector2.right && previousInputP1 != Vector2.right)
            {
                choiceIndexP1 += 1;
                choiceIndexP1 = Mathf.Clamp(choiceIndexP1, 0, uiChoicesP1.Count - 1);
                UpdateEnhancementText();


            }
            else if (currInputP1 == Vector2.left && previousInputP1 != Vector2.left)
            {
                choiceIndexP1 -= 1;
                choiceIndexP1 = Mathf.Clamp(choiceIndexP1, 0, uiChoicesP1.Count - 1);
                UpdateEnhancementText();
            }

            uiChoicesP1[choiceIndexP1].gameObject.transform.DOScale(Vector3.one, .2f);
            foreach (EnhancementUI ui in uiChoicesP1)
            {
                ui.gameObject.transform.DOScale(Vector3.one * .75f, .2f);
            }

            currChoiceP1 = enhRandChoicesP1[choiceIndexP1];

            // Store current input for next frame
            previousInputP1 = currInputP1;
        }
        // player 2
        if (!p2HasPressed)
        {
            Vector2 currInputP2 = playerManager.GetPlayer2().GetNavigateInput();

            if (currInputP2 == Vector2.right && previousInputP2 != Vector2.right)
            {
                choiceIndexP2 += 1;
                choiceIndexP2 = Mathf.Clamp(choiceIndexP2, 0, uiChoicesP2.Count - 1);
                UpdateEnhancementText();
            }
            else if (currInputP2 == Vector2.left && previousInputP2 != Vector2.left)
            {
                choiceIndexP2 -= 1;
                choiceIndexP2 = Mathf.Clamp(choiceIndexP2, 0, uiChoicesP2.Count - 1);
                UpdateEnhancementText();


            }

            uiChoicesP2[choiceIndexP2].gameObject.transform.DOScale(Vector3.one, .2f);
            foreach (EnhancementUI ui in uiChoicesP2)
            {
                ui.gameObject.transform.DOScale(Vector3.one * .75f, .2f);
            }
            
            currChoiceP2 = enhRandChoicesP2[choiceIndexP2];

            // Store current input for next frame
            previousInputP2 = currInputP2;
        }    
    }


    bool p1HasPressed;
    bool p2HasPressed;

    void GetInput()
    {
        if (playerManager.GetPlayer1().IsConfirmPressed())
        {
            if (!p1HasPressed)
            {
                p1HasPressed = true;
            }
            else
            {
                p1HasPressed = false;
            }
        }
        if (playerManager.GetPlayer2().IsConfirmPressed())
        {
            if (!p2HasPressed)
            {
                p2HasPressed = true;
            }
            else
            {
                p2HasPressed = false;
            }
        }
        if (p1HasPressed && p2HasPressed)
        {
            playerManager.GetPlayer1().AddEnhancement(currChoiceP1);
            possibleChoicesP2.Remove(currChoiceP2);

            playerManager.GetPlayer2().AddEnhancement(currChoiceP2);
            possibleChoicesP2.Remove(currChoiceP2);
            
            StartNextRound();
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
            mainCanvas.DOFade(1, 1f);
            UpdateEnhancementText();
        }
        else mainCanvas.DOFade(0, 1f);
    }

    void SetUIList()
    {
        // player 1
        for (int i = 0; i < uiChoicesP1.Count; i++)
        {
            uiChoicesP1[i].SetUI(enhRandChoicesP1[i]);
        }

        // player 2
        for (int i = 0; i < uiChoicesP2.Count; i++)
        {
            uiChoicesP2[i].SetUI(enhRandChoicesP2[i]);

        }
    }
    public void SetEnhancementChoices()
    {
        enhRandChoicesP1.Clear();
        enhRandChoicesP2.Clear();

        // Player 1 choices
        enhRandChoicesP1 = GetRandomUniqueEnhancements(3, 1);
        Debug.Log($"P1 Choices: {string.Join(", ", enhRandChoicesP1.Select(e => e.EnhancementName()))}");

        // Player 2 choices
        enhRandChoicesP2 = GetRandomUniqueEnhancements(3, 2);
        Debug.Log($"P2 Choices: {string.Join(", ", enhRandChoicesP2.Select(e => e.EnhancementName()))}");
    }

    List<Enhancement> GetRandomUniqueEnhancements(int count, int player)
    {
        // Create a shuffled copy of the player's possible enhancements list
        List<Enhancement> shuffled = new List<Enhancement>(player == 1 ? possibleChoicesP1 : possibleChoicesP2);

        // Shuffle using Fisher-Yates algorithm
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Enhancement temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }
        // Take the first 'count' items (guaranteed unique)
        return shuffled.GetRange(0, Mathf.Min(count, shuffled.Count));
    }

    void UpdateEnhancementText()
    {
        descriptionP1.text = uiChoicesP1[choiceIndexP1].GetDesc();
        descriptionP2.text = uiChoicesP2[choiceIndexP2].GetDesc();

        foreach (EnhancementEffect effect in enhRandChoicesP1[choiceIndexP1].EnhancementEffects())
        {
            if(effect.GetEnhancementType() == EnhancementType.AktionGain)
            {
                effect.GetAktion().GetName();
                aktionDescriptionP1.text = $"{effect.GetAktion().GetName()} - {effect.GetAktion().GetDesc()}";
            }
            else
            {
                aktionDescriptionP1.text = "";
            }
        }

        foreach (EnhancementEffect effect in enhRandChoicesP2[choiceIndexP2].EnhancementEffects())
        {
            if(effect.GetEnhancementType() == EnhancementType.AktionGain)
            {
                effect.GetAktion().GetName();
                aktionDescriptionP2.text = $"{effect.GetAktion().GetName()} - {effect.GetAktion().GetDesc()}";
            }
            else
            {
                aktionDescriptionP2.text = "";
            }
        }
    }
}
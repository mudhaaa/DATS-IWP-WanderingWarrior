using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] string destination;
    [SerializeField] private CanvasGroup currentScreen;
    [SerializeField] private CanvasGroup blackScreen;

    [SerializeField] private CanvasGroup titleScreen;
    [SerializeField] private CanvasGroup mainScreen;
    [SerializeField] private CanvasGroup characterSelectScreen;
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private CanvasGroup tutorialScreen;

    [Header("Character Select")]
    [SerializeField] private List<RectTransform> characterSelectIcons;
    [SerializeField] private InputActionAsset inputs;
    [SerializeField] private List<CharacterSelectUI> characterSelectUIs;

    [Header("P1 Character Select")]
    [SerializeField] private Image selectedCharP1;
    [SerializeField] private TMP_Text charNameTextP1;
    [SerializeField] private TMP_Text charDescTextP1;

    [SerializeField] private RectTransform arrowTransformP1;
    [SerializeField] private int currIndexP1;
    private InputAction moveInputP1;
    private InputAction confInputP1;

    [Header("P2 Character Select")]
    [SerializeField] private Image selectedCharP2;
    [SerializeField] private TMP_Text charNameTextP2;
    [SerializeField] private TMP_Text charDescTextP2;

    [SerializeField] private RectTransform arrowTransformP2;
    [SerializeField] private int currIndexP2;
    private InputAction moveInputP2;
    private InputAction confInputP2;
    private void Start()
    {
        currentScreen = null;
        blackScreen.alpha = 1;

        destination = CharacterSelectManager.instance.CharacterSelect() == true ? "Character Select" : "Title";
        ChangeScreen(destination);
        CharacterSelectManager.instance.SetCharacterSelect();

        moveInputP1 = inputs.FindActionMap("player1").FindAction("Navigate");
        moveInputP1.Enable();
        confInputP1 = inputs.FindActionMap("player1").FindAction("Confirm");
        confInputP1.Enable();

        moveInputP2 = inputs.FindActionMap("player2").FindAction("Navigate");
        moveInputP2.Enable();
        confInputP2 = inputs.FindActionMap("player2").FindAction("Confirm");
        confInputP2.Enable();

        moveDistance = arrowTransformP1.anchoredPosition.y - 25;
        moveDistance2 = arrowTransformP2.anchoredPosition.y + 25;
    }
    private void Update()
    {
        if (currentScreen == characterSelectScreen)
        {
            UpdateCharacterSelect();
        }
    }

    #region Character Select
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveDistance2;
    [SerializeField] Ease Ease;
    [SerializeField] float duration;

    [SerializeField] private bool doneSelectedP1;
    [SerializeField] private bool doneSelectedP2;
    void UpdateCharacterSelect()
    {
        // Player 1 

        if (confInputP1.triggered) doneSelectedP1 = !doneSelectedP1;
        if (!doneSelectedP1)
        {
            // Animate arrow
            arrowTransformP1.DOAnchorPosY(moveDistance, duration)
                        .SetEase(Ease);
        }
        else
        {
            arrowTransformP1.DOAnchorPosY(-moveDistance, duration)
                        .SetEase(Ease);
        }
        if (!doneSelectedP1)
        {
            // Read movement input
            if (moveInputP1.ReadValue<Vector2>() == Vector2.left)
            {
                currIndexP1 = Mathf.Clamp(currIndexP1 - 1, 0, characterSelectIcons.Count - 1);
            }
            if (moveInputP1.ReadValue<Vector2>() == Vector2.right)
            {
                currIndexP1 = Mathf.Clamp(currIndexP2 + 1, 0, characterSelectIcons.Count - 1);
            }
        }
        // Move the arrow's position to be centered to icon
        arrowTransformP1.SetParent(characterSelectIcons[currIndexP1]);
        arrowTransformP1.DOMoveX(characterSelectIcons[currIndexP1].position.x, 0.1f);

        // Update player texts & image
        charNameTextP1.text = characterSelectUIs[currIndexP1].GetText();
        charDescTextP1.text = characterSelectUIs[currIndexP1].GetDesc();

        selectedCharP1.sprite = characterSelectUIs[currIndexP1].GetImage();
        selectedCharP1.SetNativeSize();

        // Player 2

        if (confInputP2.triggered) doneSelectedP2 = !doneSelectedP2;

        if (!doneSelectedP2)
        {
            // Animate arrow
            arrowTransformP2.DOAnchorPosY(moveDistance2, duration)
                            .SetEase(Ease);
        }
        else
        {
            arrowTransformP2.DOAnchorPosY(-moveDistance2, duration)
                            .SetEase(Ease);
        }

        if (!doneSelectedP2)
        {
            // Read movement input
            if (moveInputP2.ReadValue<Vector2>() == Vector2.left)
            {
                currIndexP2 = Mathf.Clamp(currIndexP2 - 1, 0, characterSelectIcons.Count - 1);
            }
            if (moveInputP2.ReadValue<Vector2>() == Vector2.right)
            {
                currIndexP2 = Mathf.Clamp(currIndexP2 + 1, 0, characterSelectIcons.Count - 1);
            }
        }

        // Move the arrow's position to be centered to icon
        arrowTransformP2.SetParent(characterSelectIcons[currIndexP2]);
        arrowTransformP2.DOMoveX(characterSelectIcons[currIndexP2].position.x, 0.1f);

        // Update player texts & image
        charNameTextP2.text = characterSelectUIs[currIndexP2].GetText();
        charDescTextP2.text = characterSelectUIs[currIndexP2].GetDesc();

        selectedCharP2.sprite = characterSelectUIs[currIndexP2].GetImage();
        selectedCharP2.SetNativeSize();

        // Final
        if (doneSelectedP1 && doneSelectedP2)
        {
            CharacterKlass klass1 = characterSelectUIs[currIndexP1].GetKlass();
            CharacterKlass klass2 = characterSelectUIs[currIndexP2].GetKlass();
            CharacterSelectManager.instance.SetSelectedClass(klass1, klass2);
            StartBattle();
        }
    }

    bool battleStarting = false;

    void StartBattle()
    {
        StartCoroutine(StartBattleCoroutine());
    }

    IEnumerator StartBattleCoroutine()
    {
        ChangeScreen("Black");
        yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        if (!battleStarting) CharacterSelectManager.instance.StartBattle();
        battleStarting = true;
    }
    #endregion

    #region Screen Change
    public void ChangeScreen(string screenName)
    {
       StartCoroutine(ChangeScreenCoroutine(GetCanvasGroup(screenName)));

    }

    CanvasGroup GetCanvasGroup(string screenName)
    {
        if (screenName == "Title") return titleScreen;
        else if (screenName == "Main") return mainScreen;
        else if (screenName == "Tutorial") return tutorialScreen;
        else if (screenName == "Character Select") return characterSelectScreen;
        else if (screenName == "Loading") return loadingScreen;
        else if (screenName == "Black") return blackScreen;
        else return null;
    }

    IEnumerator ChangeScreenCoroutine(CanvasGroup canvas)
    {
        blackScreen.DOFade(1, 1f);
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        yield return new WaitForSeconds(1f);

        canvas.alpha = 1;
        if (currentScreen != null) currentScreen.interactable = false;
        if (currentScreen != null) currentScreen.blocksRaycasts = false;
        if (currentScreen != null) currentScreen.alpha = 0;

        yield return null;

        blackScreen.DOFade(0, 1f);
        currentScreen = canvas;
        currentScreen.interactable = true;
        currentScreen.blocksRaycasts = true;
    }
    #endregion

    public void QuitApp()
    {
        Application.Quit();
    }
}

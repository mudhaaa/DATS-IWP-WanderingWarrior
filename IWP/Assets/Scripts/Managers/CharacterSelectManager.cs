using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    [Header("Player 1 Characters")]
    [SerializeField] GameObject knightP1;
    [SerializeField] GameObject mageP1;
    //[SerializeField] GameObject bulwarkP1;

    [Header("Canvas")]
    [SerializeField] CanvasGroup characterSelectCG;
    [SerializeField] RectTransform characterSelectArrowP1;
    [SerializeField] RectTransform characterSelectArrowP2;

    [Header("Player 2 Characters")]
    [SerializeField] GameObject knightP2;
    [SerializeField] GameObject mageP2;
    // [SerializeField] GameObject bulwarkP2;

    public static CharacterKlass Player1SelectedClass { get; private set; }
    public static CharacterKlass Player2SelectedClass { get; private set; }

    public void SetSelectedClass(CharacterKlass p1, CharacterKlass p2)
    {
        Player1SelectedClass = p1;
        Player2SelectedClass = p2;
    }

    public void StartBattle()
    {
        SceneManager.LoadScene("BattleScene");
    }
}

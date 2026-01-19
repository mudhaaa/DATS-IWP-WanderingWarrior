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

    [SerializeField] private CharacterKlass Player1SelectedClass;
    public CharacterKlass Player1() { return Player1SelectedClass;  }
    [SerializeField] private CharacterKlass Player2SelectedClass;
    public CharacterKlass Player2() { return Player2SelectedClass; }

    [SerializeField] private bool goToCharacterSelect = false;
    public bool CharacterSelect() { return goToCharacterSelect; }
    public void SetCharacterSelect() { goToCharacterSelect = false; }

    public void SetSelectedClass(CharacterKlass p1, CharacterKlass p2)
    {
        Debug.Log("FEIn");
        Player1SelectedClass = p1;
        Player2SelectedClass = p2;
    }

    public void StartBattle()
    {
        SceneChangeManager.instance.ChangeScene("Battle");
    }

    public void GoToCharacterSelect()
    {
        goToCharacterSelect = true;
        SceneChangeManager.instance.ChangeScene("Main Menu");

    }
}

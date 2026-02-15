using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(string str)
    {
        SceneManager.LoadScene(str);
    }

    public void Rematch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);        
    }

    public void GoToCharacterSelect()
    {
        CharacterSelectManager.instance.GoToCharacterSelect();
    }

    public void GoToMainMenu()
    {
        CharacterSelectManager.instance.GoToMainMenu();
    }

    public void ChangeSceneAsync(string str)
    {
        StartCoroutine(LoadSceneCoroutine(str));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Prevent scene from activating immediately (optional)
        operation.allowSceneActivation = false;

        // While loading...
        while (!operation.isDone)
        {
            // Get progress (0 to 0.9 while loading, then 0.9 to 1.0 when ready)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);



            //// When loading is done (90%)
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null; // Wait one frame
        }
    }
}

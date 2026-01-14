using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart()
    {
        
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        
    }
}

using UnityEngine;

public class BattleBarSlider : MonoBehaviour
{
    public enum BarState
    {
        Good,
        Mid,
        Bad
    }

    [SerializeField] private BarState currState;  
    public BarState GetBarState() {  return currState; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GoodBar"))
        {
            currState = BarState.Good;
        }
        else if (collision.CompareTag("MidBar"))
        {
            currState = BarState.Mid;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MidBar"))
        {
            currState = BarState.Bad;
        }
        else if (collision.CompareTag("GoodBar"))
        {
            currState = BarState.Mid;
        }
    }
}

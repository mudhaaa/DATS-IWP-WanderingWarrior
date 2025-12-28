using UnityEngine;

public class TurnDataManager : MonoBehaviour
{
    public static TurnDataManager instance;
    private void Awake()
    {
        // Check kalau dah ada instance lain
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Set instance ini
        instance = this;

        // Optional: Kalau nak persist across scenes
        DontDestroyOnLoad(this.gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct TurnData
{
    private Character attacker;
    private Character defender;

    private int damageDealt;

    private BattleBarSlider.BarState attackerState;
    private BattleBarSlider.BarState defenderState;

    private Aktion aktionUsed;
}
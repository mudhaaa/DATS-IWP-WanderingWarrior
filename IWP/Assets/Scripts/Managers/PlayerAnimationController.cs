using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private AktionManager aktionManager;
    [SerializeField] private ParticleSystem knightBurningBlade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aktionManager = BattleManager.instance.AktionManager();
        playerManager = BattleManager.instance.PlayerManager();

        knightBurningBlade = GetComponentInChildren<ParticleSystem>();
        knightBurningBlade.gameObject.SetActive(false);
        knightBurningBlade.Stop();
       
    }
    private void FixedUpdate()
    {
        if (BattleManager.instance.GetCurrState() == BattleManager.BattleStates.Enhancement) { knightBurningBlade.Stop(); }
    }

    public void PlayerAttackFeedback()
    {
         if (aktionManager.currentAktion as AttackAktion == null) return;

        aktionManager.AttackAktionFeedback();
    }

    public void BurningBladeVFX()
    {
        knightBurningBlade.gameObject.SetActive(true);
        knightBurningBlade.Play();
    }
}

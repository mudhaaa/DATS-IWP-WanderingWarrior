using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] private AktionManager aktionManager;
    [SerializeField] private ParticleSystem knightBurningBlade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = GetComponentInParent<Character>();

        aktionManager = BattleManager.instance.AktionManager();

        knightBurningBlade = GetComponentInChildren<ParticleSystem>();
        if (knightBurningBlade != null)
        {
            knightBurningBlade.gameObject.SetActive(false);
            knightBurningBlade.Stop();

        }
    }
    private void FixedUpdate()
    {
        if ((BattleManager.instance.GetCurrState() == BattleManager.BattleStates.Enhancement ||
            !character.InBurningBlade()) && knightBurningBlade != null)
        {
            knightBurningBlade.Stop();
        }
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

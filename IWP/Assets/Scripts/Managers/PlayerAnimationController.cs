using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] private AktionManager aktionManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private ParticleSystem knightBurningBlade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = GetComponentInParent<Character>();

        aktionManager = BattleManager.instance.AktionManager();
        playerManager = BattleManager.instance.PlayerManager();

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

    public void StatusAktionFeedback()
    {
         if (aktionManager.currentAktion as StatusAktion == null) return;

        aktionManager.StatusAktionFeedback();
    }

    public void BurningBladeVFX()
    {
        knightBurningBlade.gameObject.SetActive(true);
        knightBurningBlade.Play();
    }

    public void BurningBladeCamera1()
    {
        BattleManager.instance.ChangeState(BattleManager.BattleStates.AktionAnimation);
        int player = character == playerManager.GetPlayer1() ? 3 : 5;
        BattleManager.instance.CameraManager().ChangeCameraPos(player);
    }

    public void BurningBladeCamera2()
    {
        BattleManager.instance.ChangeState(BattleManager.BattleStates.AktionAnimation);
        int player = character == playerManager.GetPlayer1() ? 4 : 6;
        BattleManager.instance.CameraManager().ChangeCameraPos(player);
    }

    public void FierceSlashCamera1()
    {
        BattleManager.instance.ChangeState(BattleManager.BattleStates.AktionAnimation);
        int player = character == playerManager.GetPlayer1() ? 4 : 6;
        BattleManager.instance.CameraManager().ChangeCameraPos(player);
    }

    public void FierceSlashCamera2()
    {
        BattleManager.instance.ChangeState(BattleManager.BattleStates.AktionAnimation);
        int player = character == playerManager.GetPlayer1() ? 1 : 2;
        BattleManager.instance.CameraManager().ChangeCameraPos(player);
    }

    public void ChangeCameraPos(int i)
    {
        BattleManager.instance.CameraManager().ChangeCameraPos(i);
        BattleManager.instance.ChangeState(BattleManager.BattleStates.AktionAnimation);
    }
}

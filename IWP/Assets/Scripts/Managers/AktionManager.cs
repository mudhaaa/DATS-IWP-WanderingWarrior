using UnityEngine;

public class AktionManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private Character player1;
    private Character player2;

    private BattleBarManager battleBarManager;
    private BattleBarSlider sliderP1;
    private BattleBarSlider sliderP2;

    private CanvasManager canvasManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart(PlayerManager pm, BattleBarManager bm, CanvasManager cm)
    {
        playerManager = pm;
        battleBarManager = bm;
        canvasManager = cm;

        player1 = pm.GetPlayer1();
        player2 = pm.GetPlayer2();

        sliderP1 = bm.GetSliderP1();
        sliderP2 = bm.GetSliderP2();
    }

    // Update is called once per frame
    public void OnUpdate()
    {


    }

    public void AktionEffect(Character player, Aktion aktion)
    {
        if (player == player1)
        {
            if (sliderP1.GetBarState() == BattleBarSlider.BarState.Bad) return;
        }
        else if (player == player2)
        {
            if (sliderP2.GetBarState() == BattleBarSlider.BarState.Bad) return;
        }

        int newHealth = player.GetHealth() - Mathf.CeilToInt(player.GetOriginalHealth() * aktion.GetHealthCost());
        player.SetHealth(newHealth);
        int newMana = player.GetMana() - Mathf.CeilToInt(player.GetOriginalMana() * aktion.GetManaCost());
        player.SetMana(newMana);

        canvasManager.UpdatePlayerBars(player); 

        AttackAktion attack = aktion as AttackAktion;
        if (attack != null) 
        { 
            Debug.Log("This is attack of name " + attack.GetName());
            if (player == player1)
            {
                newHealth = player2.GetHealth() - DamageCalculation(attack, player1, player2, sliderP1.GetBarState(), sliderP2.GetBarState());
                player2.SetHealth(newHealth);
                canvasManager.UpdatePlayerBars(player2);
            }
            else if (player == player2)
            {
                newHealth = player1.GetHealth() - DamageCalculation(attack, player2, player1, sliderP2.GetBarState(), sliderP1.GetBarState());
                player1.SetHealth(newHealth);
                canvasManager.UpdatePlayerBars(player1);
            }
        }

        StatusAktion status = aktion as StatusAktion;
        if (status != null)
        {
            StatusMove(status, player);
        }
    }

    void StatusMove(StatusAktion status, Character target)
    {
        if (status.GetStatusType() == StatusType.Restore)
        {
            if (status.GetStatChange().Contains(Stat.Health))  OnRestoreHealth(target);            
            else if (status.GetStatChange().Contains(Stat.Mana)) OnRestoreMana(target);            
        }
        else if (status.GetStatusType() == StatusType.Increase)
        {
            if (status.GetStatChange().Contains(Stat.Strength))   OnBuffStrength(target);
            if (status.GetStatChange().Contains(Stat.Endurance)) OnBuffEndurance(target);   
            if (status.GetStatChange().Contains(Stat.Magic))         OnBuffMagic(target);
            if (status.GetStatChange().Contains(Stat.Speed))         OnBuffSpeed(target);
        }
        else if (status.GetStatusType() == StatusType.Decrease)
        {
            if (status.GetStatChange().Contains(Stat.Strength))   OnNerfStrength(target);
            if (status.GetStatChange().Contains(Stat.Endurance)) OnNerfEndurance(target);   
            if (status.GetStatChange().Contains(Stat.Magic))         OnNerfMagic(target);
            if (status.GetStatChange().Contains(Stat.Speed))         OnNerfSpeed(target);
        }
    }

    #region Increase
    void OnBuffStrength(Character target)
    {
        if (target.GetStrength() < target.GetOriginalStrength())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetStrength() * 1.3f);
            target.SetStrength(newValue);
        }
    }

    void OnBuffMagic(Character target)
    {
        if (target.GetMagic() < target.GetOriginalMagic())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetMagic() * 1.3f);
            target.SetMagic(newValue);
        }
    }

    void OnBuffEndurance(Character target)
    {
        if (target.GetEndurance() < target.GetOriginalEndurance())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetEndurance() * 1.3f);
            target.SetEndurance(newValue);
        }
    }

    void OnBuffSpeed(Character target)
    {
        if (target.GetSpeed() < target.GetOriginalSpeed())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetSpeed() * 1.3f);
            target.SetSpeed(newValue);
        }
    }
    #endregion

    #region Decrease
    void OnNerfStrength(Character target)
    {
        if (target.GetStrength() < target.GetOriginalStrength())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetStrength() * 1.3f);
            target.SetStrength(newValue);
        }
    }

    void OnNerfMagic(Character target)
    {
        if (target.GetMagic() < target.GetOriginalMagic())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetMagic() * 1.3f);
            target.SetMagic(newValue);
        }
    }

    void OnNerfEndurance(Character target)
    {
        if (target.GetEndurance() < target.GetOriginalEndurance())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetEndurance() * 1.3f);
            target.SetEndurance(newValue);
        }
    }

    void OnNerfSpeed(Character target)
    {
        if (target.GetSpeed() < target.GetOriginalSpeed())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetSpeed() * 1.3f);
            target.SetSpeed(newValue);
        }
    }
    #endregion

    #region Restore
    void OnRestoreHealth(Character target)
    {
        int newValue = Mathf.CeilToInt(target.GetHealth() + target.GetOriginalMana() * .07f);
        target.SetHealth(newValue);
    }

    void OnRestoreMana(Character target)
    {
        int newValue = Mathf.CeilToInt(target.GetMana() + target.GetOriginalHealth() * .05f);
        target.SetMana(newValue);
    }
    #endregion


    int DamageCalculation(AttackAktion attack, Character atk, Character def, BattleBarSlider.BarState attackState, BattleBarSlider.BarState defenseState)
    {
        // attacking stat
        int a = 0;
        switch (attack.GetAttackType())
        {
            case AttackAktion.AttackType.Strength:
                a = atk.GetStrength();
                break;
            case AttackAktion.AttackType.Magic:
                a = atk.GetMagic();
                break;
            case AttackAktion.AttackType.Endurance:
                a = atk.GetEndurance();
                break;
            case AttackAktion.AttackType.None:
                break;
        }

        // damage mult
        float d = attack.GetDamageMultiplier();

        // defender endurance
        int e = def.GetEndurance();

        // value from attack bar
        float attackBar = 0;
        switch (attackState)
        {
            case BattleBarSlider.BarState.Good:
                attackBar = 1.5f;
                break;
            case BattleBarSlider.BarState.Mid:
                attackBar = 1f;
                break;
            case BattleBarSlider.BarState.Bad:
                attackBar = 0;
                break;
        }
        float defenceBar = 0;
        switch (defenseState)
        {
            case BattleBarSlider.BarState.Bad:
                defenceBar = 1.5f;
                break;
            case BattleBarSlider.BarState.Mid:
                defenceBar = 1f;
                break;
            case BattleBarSlider.BarState.Good:
                defenceBar = 0;
                break;
        }

        Debug.Log("Damage Dealt:" + Mathf.Clamp(Mathf.CeilToInt((1 + ((a * d) - e) * attackBar) * defenceBar), 0, 999));
        return Mathf.Clamp(Mathf.CeilToInt((1 + ((a * d) - e) * attackBar) * defenceBar), 0, 999);
    }
}

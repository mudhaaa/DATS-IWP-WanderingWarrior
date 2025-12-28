using System.Collections;
using System.Collections.Generic;
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

    public Character currentAttacker { private set; get; }
    public Character currentDefender { private set; get; }
    public int currentDamageDealt { private set; get; }
    public BattleBarSlider.BarState currentAttackerState { private set; get; }
    public BattleBarSlider.BarState currentDefenderState { private set; get; }
    public Aktion currentAktion { private set; get; }

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
        if (player == player1 && aktion as AttackAktion != null)
        {
            if (sliderP1.GetBarState() == BattleBarSlider.BarState.Bad) return;
        }
        else if (player == player2 && aktion as AttackAktion != null)
        {
            if (sliderP2.GetBarState() == BattleBarSlider.BarState.Bad) return;
        }

        int newAP = player.GetAP() - Mathf.CeilToInt(player.GetOriginalAP() * aktion.GetAPCost());
        player.SetAP(newAP);

        canvasManager.UpdatePlayerBars(player); 

        currentAktion = aktion;

        AttackAktion attack = aktion as AttackAktion;
        if (attack != null) 
        { 
            Debug.Log("This is attack of name " + attack.GetName());
            if (player == player1)
            {
                AttackMove(attack, player1, player2);
            }
            else if (player == player2)
            {
                AttackMove(attack, player2, player1);
            }
        }

        StatusAktion status = aktion as StatusAktion;
        if (status != null)
        {
            Debug.Log("This is status of name " + status.GetName());

            foreach(StatusEffect effect in status.GetStatusEffectList())
            {
                StatusMove(effect, player, effect.IsSelfTarget());
            }

            if (status.IsUnique()) player.PlayAnimation("UniqueStatus");
           
            else player.PlayAnimation("Magic");
        }
    }


    void AttackMove(AttackAktion attack, Character attacker, Character defender)
    {
        BattleBarSlider.BarState defenceSlider = defender == player1 ? sliderP1.GetBarState() : sliderP2.GetBarState();
        BattleBarSlider.BarState attackSlider = attacker == player1 ? sliderP1.GetBarState() : sliderP2.GetBarState();

        currentAttacker = attacker;
        currentDefender = defender;
        currentDefenderState = defenceSlider;
        currentAttackerState = attackSlider;

        //damage calculation
        currentDamageDealt = DamageCalculation(attack, attacker, defender, attackSlider, defenceSlider);
        int newHealth = defender.GetHealth() - currentDamageDealt;
        defender.SetHealth(newHealth);

        StartAttackFeedbackCoroutine(attack, attacker, defender, currentDamageDealt, defenceSlider);

        // call crit effects
        if (attack.GetCritEffectList().Count > 0 && attackSlider == BattleBarSlider.BarState.Good)
        {
            Debug.Log("Activating OnCrit effects");
            foreach (StatusEffect effect in attack.GetCritEffectList())
            {
                StatusMove(effect, attacker, effect.IsSelfTarget());
            }
        }

        // call hit effects
        if (attack.GetHitEffectList().Count > 0 && (attackSlider == BattleBarSlider.BarState.Good || attackSlider == BattleBarSlider.BarState.Mid) )
        {
            Debug.Log("Activating OnHit effects");
            foreach (StatusEffect effect in attack.GetHitEffectList())
            {
                StatusMove(effect, attacker, effect.IsSelfTarget());
            }
        }
    }

    Coroutine currentAktionCoroutine = null;

    public void StartAttackFeedbackCoroutine(AttackAktion attack, Character attacker, Character defender, int damage, BattleBarSlider.BarState defenceSlider)
    {

        currentAktionCoroutine = StartCoroutine(AttackAktionFeedback(attack, attacker, defender, damage, defenceSlider));
    }

    IEnumerator AttackAktionFeedback(AttackAktion attack, Character attacker, Character defender, int damage, BattleBarSlider.BarState defenceSlider)
    {
        // animation play
        if(attack.GetName() == "Fierce Slash")
        {
            attacker.PlayAnimation("UniqueAttack");
        }
        else if (attack.GetAttackType() == AttackAktion.AttackType.Strength || attack.GetAttackType() == AttackAktion.AttackType.Endurance)
        {
            attacker.PlayAnimation("Strength");
        }
        else if (attack.GetAttackType() == AttackAktion.AttackType.Magic)
        {
            attacker.PlayAnimation("Magic");
        }

        yield return new WaitForSeconds(1f);

        
    }

    public void AttackAktionFeedback()
    {
        canvasManager.UpdatePlayerBars(currentDefender);

        canvasManager.ActivateDamageNumber(currentDefender == player1 ? 1 : 2, currentDamageDealt);

        if (currentAktion.GetVFX() != null)
        {
            GameObject vfxGo = Instantiate(currentAktion.GetVFX());
            Vector3 vfxOffset = currentAktion.GetVFXOffset();
            if (currentDefender == player1) vfxOffset = vfxOffset * -1;
            vfxGo.transform.position = currentDefender.transform.position - vfxOffset;
            vfxGo.transform.LookAt(currentDefender.transform);
            ParticleSystem vfx = vfxGo.GetComponentInChildren<ParticleSystem>();
            vfx.Play();
        }
        
        if (currentDefenderState == BattleBarSlider.BarState.Bad || currentDefender.GetHealth() <= currentDamageDealt)
        {
            currentDefender.PlayAnimation("Block Fail");
        }

        currentAktionCoroutine = null;
    }

    void StatusMove(StatusEffect effect, Character user, bool isSelfTarget)
    {
        Character target;
        if (isSelfTarget)
        {
            // target is if user is player1, player1, else, player2
            target = user == player1 ? player1 : player2;
        }
        else
        {
            // target is if user is player1, player2, else, player1
            target = user == player1 ? player2 : player1;
        }

        if (effect.GetStatusType() == StatusType.Restore)
        {
            Debug.Log("Activating Restore effect");
            if (effect.GetStatType() == Stat.Health)    OnRestoreHealth(target);
            if (effect.GetStatType() == Stat.AP)      OnRestoreAP(target);
        }
        else if (effect.GetStatusType() == StatusType.Reduce)
        {
            Debug.Log("Activating Reduce effect");

            if (effect.GetStatType() == Stat.Health)    OnReduceHealth(target);
            if (effect.GetStatType() == Stat.AP)      OnReduceAP(target);
        }
        else if (effect.GetStatusType() == StatusType.Increase)
        {
            Debug.Log("Activating Increase effect");

            if (effect.GetStatType() == Stat.Strength)  OnBuffStrength(target);
            if (effect.GetStatType() == Stat.Endurance) OnBuffEndurance(target);
            if (effect.GetStatType() == Stat.Magic)     OnBuffMagic(target);
            if (effect.GetStatType() == Stat.Speed)     OnBuffSpeed(target);
            if (effect.GetStatType() == Stat.Crit)      OnBuffCrit(target);
        }
        else if (effect.GetStatusType() == StatusType.Decrease)
        {
            Debug.Log("Activating Decrease effect");

            if (effect.GetStatType() == Stat.Strength)  OnNerfStrength(target);
            if (effect.GetStatType() == Stat.Endurance) OnNerfEndurance(target);
            if (effect.GetStatType() == Stat.Magic)     OnNerfMagic(target);
            if (effect.GetStatType() == Stat.Speed)     OnNerfSpeed(target);
            if (effect.GetStatType() == Stat.Crit)      OnNerfCrit(target);
        }
        else if (effect.GetStatusType() == StatusType.Reset)
        {
            Debug.Log("Activating Reset effect");

            target.ResetToOriginalStats();
        }
    }

    #region Increase
    void OnBuffStrength(Character target)
    {
        if (target.GetStrength() <= target.GetOriginalStrength())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetStrength() * 1.3f);
            target.SetStrength(newValue);
            Debug.Log($"[BUFF] {target.name}'s Strength increased");
        }
    }

    void OnBuffMagic(Character target)
    {
        if (target.GetMagic() <= target.GetOriginalMagic())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetMagic() * 1.3f);
            target.SetMagic(newValue);
            Debug.Log($"[BUFF] {target.name}'s Magic increased");
        }
    }

    void OnBuffEndurance(Character target)
    {
        if (target.GetEndurance() <= target.GetOriginalEndurance())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetEndurance() * 1.3f);
            target.SetEndurance(newValue);
            Debug.Log($"[BUFF] {target.name}'s Endurance increased");
        }
    }

    void OnBuffSpeed(Character target)
    {
        if (target.GetSpeed() <= target.GetOriginalSpeed())
        {
            int newValue;
            newValue = Mathf.CeilToInt(target.GetSpeed() * 1.3f);
            target.SetSpeed(newValue);
            Debug.Log($"[BUFF] {target.name}'s Speed increased");
        }
    }

    void OnBuffCrit(Character target)
    {
        target.SetCrit(2);
        Debug.Log($"[BUFF] {target.name}'s Crit increased");
    }
    #endregion

    #region Decrease
    void OnNerfStrength(Character target)
    {
        int newValue;
        newValue = Mathf.CeilToInt(target.GetStrength() * 0.7f);
        target.SetStrength(newValue);
        Debug.Log($"[NERF] {target.name}'s Strength decreased");
    }

    void OnNerfMagic(Character target)
    {
        int newValue;
        newValue = Mathf.CeilToInt(target.GetMagic() * 0.7f);
        target.SetMagic(newValue);
        Debug.Log($"[NERF] {target.name}'s Magic decreased");
    }

    void OnNerfEndurance(Character target)
    {
        int newValue;
        newValue = Mathf.CeilToInt(target.GetEndurance() * 0.7f);
        target.SetEndurance(newValue);
        Debug.Log($"[NERF] {target.name}'s Endurance decreased");
    }

    void OnNerfSpeed(Character target)
    {
        int newValue;
        newValue = Mathf.CeilToInt(target.GetSpeed() * 0.7f);
        target.SetSpeed(newValue);
        Debug.Log($"[NERF] {target.name}'s Speed decreased");
    }
    void OnNerfCrit(Character target)
    {
        target.SetCrit(1.5f);
        Debug.Log($"[NERF] {target.name}'s Crit decreased");
    }
    #endregion

    #region Restore
    void OnRestoreHealth(Character target)
    {
        int newValue = Mathf.CeilToInt(target.GetHealth() + target.GetOriginalHealth() * .07f);
        target.SetHealth(newValue);
        canvasManager.UpdatePlayerBars(target);
        Debug.Log($"[RESTORE] {target.name}'s Health restored");
    }

    void OnRestoreAP(Character target)
    {
        int newValue = Mathf.CeilToInt(target.GetAP() + 1);
        target.SetAP(newValue);
        canvasManager.UpdatePlayerBars(target);
        Debug.Log($"[RESTORE] {target.name}'s AP restored");
    }
    #endregion

    #region Reduce
    void OnReduceHealth(Character target)
    {
        int newValue = Mathf.CeilToInt(target.GetHealth() - target.GetOriginalHealth() * .03f);
        target.SetHealth(newValue);
        canvasManager.UpdatePlayerBars(target);
        Debug.Log($"[REDUCE] {target.name}'s Health reduced");
    }

    void OnReduceAP(Character target)
    {
        int newValue = Mathf.CeilToInt(target.GetAP() - 1);
        target.SetAP(newValue);
        canvasManager.UpdatePlayerBars(target);
        Debug.Log($"[REDUCE] {target.name}'s AP reduced");
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
            // critical hit
            case BattleBarSlider.BarState.Good:
                attackBar = atk.GetCrit();
                break;
            // normal hit
            case BattleBarSlider.BarState.Mid:
                attackBar = 1f;
                break;
            // miss
            case BattleBarSlider.BarState.Bad:
                attackBar = 0;
                break;
        }


        float defenceBar = 0.5f;
        switch (defenseState)
        {
            // full damage
            case BattleBarSlider.BarState.Bad:
                defenceBar = 1.5f;
                break;
            // block
            case BattleBarSlider.BarState.Mid:
                defenceBar = 1f;
                break;
            // full block
            case BattleBarSlider.BarState.Good:
                defenceBar = 0.5f;
                break;
        }

        Debug.Log("Damage Dealt:" + Mathf.Clamp(Mathf.CeilToInt((1 + ((a * d) - e) * attackBar) * defenceBar), 1, 999));
        return Mathf.Clamp(Mathf.CeilToInt((1 + ((a * d) - e) * attackBar) * defenceBar), 1, 999);
    }


}

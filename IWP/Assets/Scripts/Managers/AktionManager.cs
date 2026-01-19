using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
            if (sliderP1.GetBarState() == BattleBarSlider.BarState.Bad)
            {
                Debug.Log("Player 1 failed the attack");

                return;
            }
        }
        else if (player == player2 && aktion as AttackAktion != null)
        {
            if (sliderP2.GetBarState() == BattleBarSlider.BarState.Bad)
            {
                Debug.Log("Player 2 failed the attack");
                return;
            }
        }

        int newAP = player.GetAP() - aktion.GetAPCost();
        player.SetAP(newAP);

        canvasManager.UpdatePlayerBars(player); 

        currentAktion = aktion;

        AttackAktion attack = aktion as AttackAktion;
        if (attack != null) 
        { 
            Debug.Log("This is attack of name " + attack.GetName());
            if (player == player1)
            {
                RefundAP(player2, sliderP2.GetBarState());
                AttackMove(attack, player1, player2);
            }
            else if (player == player2)
            {
                RefundAP(player1, sliderP1.GetBarState());

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

    public void RefundAP(Character player, BattleBarSlider.BarState barState)
    {
        if (barState == BattleBarSlider.BarState.Good)
        {
            int newAP = player.GetAP() + 1;
            player.SetAP(newAP);
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
        if(attack.IsUnique())
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
            PlayVFX();
        }


        if (currentDefenderState == BattleBarSlider.BarState.Bad || currentDefender.GetHealth() <= currentDamageDealt)
        {
            currentDefender.PlayAnimation("Block Fail");
        }
        else
        {
            currentDefender.PlayAnimation("Block Success", 0.05f);
        }

        currentAktionCoroutine = null;
    }

    void PlayVFX()
    {
        GameObject vfxGo = Instantiate(currentAktion.GetVFX());
        Vector3 vfxOffset = currentAktion.GetVFXOffset();

        //if (currentDefender == player1) vfxOffset = new Vector3(vfxOffset.x, vfxOffset.y * -1, vfxOffset.z);

        if (!currentAktion.IsOnUser())
        {
            vfxGo.transform.position = currentDefender.transform.position - vfxOffset;
            vfxGo.transform.LookAt(currentDefender.transform);
        }
        else
        {
            vfxGo.transform.position = currentAttacker.transform.position - vfxOffset;
            vfxGo.transform.LookAt(currentDefender.transform);
        }

        VisualEffect vfx = vfxGo.GetComponent<VisualEffect>();
        if (vfx != null) vfx.Play();

        ParticleSystem ps = vfxGo.GetComponentInChildren<ParticleSystem>();
        if (ps != null) ps.Play();
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
            if (target.StatChangeImmune())
            {
                string msg = "Stat Change Immune!";
                target.GetTurnUpdateLists().Add(msg);   
                Debug.Log(msg); 
                return;
            }
        }
        if (effect.GetStatusType() == StatusType.Restore)
        {
            Debug.Log("Activating Restore effect");
            if (effect.GetStatType() == Stat.Health)  OnRestoreHealth(target, effect);
            if (effect.GetStatType() == Stat.AP)      OnRestoreAP(target, effect);
        }
        else if (effect.GetStatusType() == StatusType.Reduce)
        {
            Debug.Log("Activating Reduce effect");

            if (effect.GetStatType() == Stat.Health)  OnReduceHealth(target, effect);
            if (effect.GetStatType() == Stat.AP)      OnReduceAP(target, effect);
        }
        else if (effect.GetStatusType() == StatusType.Increase)
        {
            Debug.Log("Activating Increase effect");

            if (effect.GetStatType() == Stat.Strength)  OnBuffStrength(target, effect);
            if (effect.GetStatType() == Stat.Endurance) OnBuffEndurance(target, effect);
            if (effect.GetStatType() == Stat.Magic)     OnBuffMagic(target, effect);
            if (effect.GetStatType() == Stat.Speed)     OnBuffSpeed(target, effect);
            if (effect.GetStatType() == Stat.Crit)      OnBuffCrit(target, effect);
        }
        else if (effect.GetStatusType() == StatusType.Decrease)
        {
            Debug.Log("Activating Decrease effect");

            if (effect.GetStatType() == Stat.Strength)  OnNerfStrength(target, effect);
            if (effect.GetStatType() == Stat.Endurance) OnNerfEndurance(target, effect);
            if (effect.GetStatType() == Stat.Magic)     OnNerfMagic(target, effect);
            if (effect.GetStatType() == Stat.Speed)     OnNerfSpeed(target, effect);
            if (effect.GetStatType() == Stat.Crit)      OnNerfCrit(target, effect);
        }
        else if (effect.GetStatusType() == StatusType.Reset)
        {
            Debug.Log("Activating Reset effect");

            target.ResetToOriginalStats();

            target.ResetStatChangeTimers();
        }
        else if(effect.GetStatusType() == StatusType.StatusImmune)
        {
            target.SetStatChangeImmune(true);
            target.SetStatChangeImmuneTimer(effect.GetBoost().GetTimer());

            string msg = $"{target.name} is immune to Stat Changes!";
            target.GetTurnUpdateLists().Add(msg);
        }
    }

    #region Increase
    void OnBuffStrength(Character target, StatusEffect effect)
    {
        if (target.GetStrengthBuffTimer() == 0)
        { 
            // Checks if the boost gives a unique amount
            if (!effect.GetBoost().IsUniqueAmount())
            {
                int newValue;
                newValue = Mathf.CeilToInt(target.GetStrength() * 1.3f);
                target.SetStrength(newValue);
            }

            target.SetStrengthBuffTimer(effect.GetBoost().GetTimer());
            target.SetStrengthNerfTimer(0);

            string msg = $"{target.name}'s Strength increased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetStrengthBuffTimer(target.GetStrengthBuffTimer() + effect.GetBoost().GetTimer());

            string msg = $"{target.name}'s Strength increase extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);

            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }

    void OnBuffMagic(Character target, StatusEffect effect)
    {
        if (target.GetMagicBuffTimer() == 0)
        {
            // Checks if the boost gives a unique amount
            if (!effect.GetBoost().IsUniqueAmount())
            {
                int newValue;
                newValue = Mathf.CeilToInt(target.GetMagic() * 1.3f);
                target.SetMagic(newValue);
            }

            target.SetMagicBuffTimer(effect.GetBoost().GetTimer());
            target.SetMagicNerfTimer(0);

            string msg = $"{target.name}'s Magic increased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetMagicBuffTimer(target.GetMagicBuffTimer() + effect.GetBoost().GetTimer());

            string msg = $"{target.name}'s Magic increase extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);

            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }

    void OnBuffEndurance(Character target, StatusEffect effect)
    {
        if (target.GetEnduranceBuffTimer() == 0)
        {
            // Checks if the boost gives a unique amount
            if (!effect.GetBoost().IsUniqueAmount())
            {
                int newValue;
                newValue = Mathf.CeilToInt(target.GetEndurance() * 1.3f);
                target.SetEndurance(newValue);
            }

            target.SetEnduranceBuffTimer(effect.GetBoost().GetTimer());
            target.SetEnduranceNerfTimer(0);

            string msg = $"{target.name}'s Endurance increased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetEnduranceBuffTimer(target.GetEnduranceBuffTimer() + effect.GetBoost().GetTimer());

            string msg = $"{target.name}'s Endurance increase extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);

            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }

    void OnBuffSpeed(Character target, StatusEffect effect)
    {
        if (target.GetSpeedBuffTimer() == 0)
        {
            // Checks if the boost gives a unique amount
            if (!effect.GetBoost().IsUniqueAmount())
            {
                int newValue;
                newValue = Mathf.CeilToInt(target.GetSpeed() * 1.3f);
                target.SetSpeed(newValue);
            }

            target.SetSpeedBuffTimer(effect.GetBoost().GetTimer());
            target.SetSpeedNerfTimer(0);

            string msg = $"{target.name}'s Speed increased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetSpeedBuffTimer(target.GetSpeedBuffTimer() + effect.GetBoost().GetTimer());

            string msg = $"{target.name}'s Speed increase extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);

            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }

    void OnBuffCrit(Character target, StatusEffect effect)
    {
        if (target.GetCritBuffTimer() == 0)
        {
            target.SetCrit(2);

            target.SetCritBuffTimer(effect.GetBoost().GetTimer());
            target.SetCritNerfTimer(0);

            string msg = $"{target.name}'s Crit increased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetCritBuffTimer(target.GetCritBuffTimer() + effect.GetBoost().GetTimer());

            string msg = $"{target.name}'s Crit increase extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }
    #endregion

    #region Decrease
    void OnNerfStrength(Character target, StatusEffect effect)
    {
        if (target.GetStrengthNerfTimer() == 0)
        {
            // Checks if the boost gives a unique amount
            if (!effect.GetBoost().IsUniqueAmount())
            {
                int newValue;
                newValue = Mathf.CeilToInt(target.GetStrength() * 0.7f);
                target.SetStrength(newValue);
            }

            target.SetStrengthNerfTimer(effect.GetBoost().GetTimer());
            target.SetStrengthBuffTimer(0);

            string msg = $"{target.name}'s Strength decreased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetStrengthNerfTimer(target.GetStrengthNerfTimer() + effect.GetBoost().GetTimer());
            string msg = $"{target.name}'s Strength decrease extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }

    void OnNerfMagic(Character target, StatusEffect effect)
    {
        if (target.GetMagicNerfTimer() == 0)
        {
            // Checks if the boost gives a unique amount
            if (!effect.GetBoost().IsUniqueAmount())
            {
                int newValue;
                newValue = Mathf.CeilToInt(target.GetMagic() * 0.7f);
                target.SetMagic(newValue);
            }

            target.SetMagicNerfTimer(effect.GetBoost().GetTimer());
            target.SetMagicBuffTimer(0);

            string msg = $"{target.name}'s Magic decreased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetMagicNerfTimer(target.GetMagicNerfTimer() + effect.GetBoost().GetTimer());
            string msg = $"{target.name}'s Magic decrease extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }

    void OnNerfEndurance(Character target, StatusEffect effect)
    {
        if (target.GetEnduranceNerfTimer() == 0)
        {
            // Checks if the boost gives a unique amount
            if (!effect.GetBoost().IsUniqueAmount())
            {
                int newValue;
                newValue = Mathf.CeilToInt(target.GetEndurance() * 0.7f);
                target.SetEndurance(newValue);
            }

            target.SetEnduranceNerfTimer(effect.GetBoost().GetTimer());
            target.SetEnduranceBuffTimer(0);

            string msg = $"{target.name}'s Endurance decreased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetEnduranceNerfTimer(target.GetEnduranceNerfTimer() + effect.GetBoost().GetTimer());
            string msg = $"{target.name}'s Endurance decrease extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }

    void OnNerfSpeed(Character target, StatusEffect effect)
    {
        if (target.GetSpeedNerfTimer() == 0)
        {
            // Checks if the boost gives a unique amount
            if (!effect.GetBoost().IsUniqueAmount())
            {
                int newValue;
                newValue = Mathf.CeilToInt(target.GetSpeed() * 0.7f);
                target.SetSpeed(newValue);
            }

            target.SetSpeedNerfTimer(effect.GetBoost().GetTimer());
            target.SetSpeedBuffTimer(0);

            string msg = $"{target.name}'s Speed decreased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetSpeedNerfTimer(target.GetSpeedNerfTimer() + effect.GetBoost().GetTimer());
            string msg = $"{target.name}'s Speed decrease extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }
    void OnNerfCrit(Character target, StatusEffect effect)
    {
        if (target.GetCritNerfTimer() == 0)
        {
            target.SetCrit(1);

            target.SetCritNerfTimer(effect.GetBoost().GetTimer());
            target.SetCritBuffTimer(0);

            string msg = $"{target.name}'s Crit decreased!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
        }
        else
        {
            // Extends on timer
            target.SetCritNerfTimer(target.GetCritNerfTimer() + effect.GetBoost().GetTimer());
            string msg = $"{target.name}'s Crit decrease extended!";
            target.GetTurnUpdateLists().Add(msg);
            //Debug.Log(msg);
            // Refreshes timer to base
            //target.SetStrengthBuffTimer(effect.GetBoost().GetTimer()); 
        }
    }
    #endregion

    #region Restore
    void OnRestoreHealth(Character target, StatusEffect effect)
    {
        int changeAmt = Mathf.CeilToInt(target.GetOriginalHealth() * effect.GetBoost().GetEffectAmount());

        int newValue = Mathf.CeilToInt(target.GetHealth() + changeAmt);
        target.SetHealth(newValue);
        canvasManager.UpdatePlayerBars(target);

        string msg = $"{target.name}'s Health restored by {changeAmt}!";
        target.GetTurnUpdateLists().Add(msg);
        //Debug.Log(msg);
    }

    void OnRestoreAP(Character target, StatusEffect effect)
    {
        int changeAmt = Mathf.CeilToInt(target.GetOriginalAP() * effect.GetBoost().GetEffectAmount());
        int newValue = Mathf.CeilToInt(target.GetAP() + changeAmt);
        target.SetAP(newValue);
        canvasManager.UpdatePlayerBars(target);

        string msg = $"{target.name}'s AP restored by {changeAmt}!";
        target.GetTurnUpdateLists().Add(msg);
        //Debug.Log(msg);
    }
    #endregion

    #region Reduce
    void OnReduceHealth(Character target, StatusEffect effect)
    {
        int changeAmt = Mathf.CeilToInt(target.GetOriginalHealth() * effect.GetBoost().GetEffectAmount());

        int newValue = Mathf.CeilToInt(target.GetHealth() - changeAmt);
        target.SetHealth(newValue);
        canvasManager.UpdatePlayerBars(target);

        Debug.Log(newValue);
        string msg = $"{target.name}'s Health reduced by {changeAmt}!";
        target.GetTurnUpdateLists().Add(msg);
        //Debug.Log(msg);
    }

    void OnReduceAP(Character target, StatusEffect effect)
    {
        int changeAmt = Mathf.CeilToInt(target.GetOriginalAP() * effect.GetBoost().GetEffectAmount());

        int newValue = Mathf.CeilToInt(target.GetAP() - changeAmt);
        target.SetAP(newValue);
        canvasManager.UpdatePlayerBars(target);

        string msg = $"{target.name}'s AP reduced by {changeAmt}!";
        target.GetTurnUpdateLists().Add(msg);
        //Debug.Log(msg);
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

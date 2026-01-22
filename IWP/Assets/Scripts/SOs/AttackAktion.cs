using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]

public class AttackAktion : Aktion
{
    [Header("Attack")]
    [SerializeField] private DamageMultiplier damageMultiplier;
    public float GetDamageMultiplier()
    {
        if (damageMultiplier == DamageMultiplier.Weak) return 1f;
        else if (damageMultiplier == DamageMultiplier.Medium) return 1.25f;
        else if (damageMultiplier == DamageMultiplier.Heavy) return 1.5f;
        else if(damageMultiplier == DamageMultiplier.Massive) return 1.75f;
        else return 0f;
    }

    [SerializeField] private AttackType attackType;
    public AttackType GetAttackType() { return attackType; }

    // crit effect
    [SerializeField] private List<StatusEffect> critEffects;
    public List<StatusEffect> GetCritEffectList() { return critEffects; }
    public StatusEffect GetCritEffect(int i) { return critEffects[i]; }

    // hit effect
    [SerializeField] private List<StatusEffect> hitEffects;
    public List<StatusEffect> GetHitEffectList() { return hitEffects; }
    public StatusEffect GetHitEffect(int i) { return hitEffects[i]; }

    public enum AttackType
    {
        None,
        Strength,
        Endurance,
        Magic
    }

    public enum DamageMultiplier
    {
        None,
        Weak,
        Medium,
        Heavy,
        Massive
    }
}

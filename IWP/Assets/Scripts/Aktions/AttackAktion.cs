using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]

public class AttackAktion : Aktion
{

    [SerializeField] private DamageMultiplier damageMultiplier;
    public float GetDamageMultiplier()
    {
        if (damageMultiplier == DamageMultiplier.Light) return 0.5f;
        else if (damageMultiplier == DamageMultiplier.Medium) return 1f;
        else if (damageMultiplier == DamageMultiplier.Heavy) return 1.5f;
        else return 0f;
    }

    [SerializeField] private AttackType attackType;
    public AttackType GetAttackType() { return attackType; }

    // crit effect
    [SerializeField] private StatusType onCritEffect;
    public StatusType GetCritEffect() { return onCritEffect; }

    // stat change
    [SerializeField] private List<Stat> statsChange;
    public List<Stat> GetStatChange() { return statsChange; }

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
        Light,
        Medium,
        Heavy
    }
}

using System;
using UnityEngine;

[CreateAssetMenu()]

public class AttackAktion : Aktion
{

    [SerializeField] private float healthCost;
    [SerializeField] private float manaCost;

    public DamageMultiplier damageMultiplier;
    public AttackType attackType;

    public float GetHealthCost() { return healthCost; }
    public float GetManaCost() { return manaCost; }
    public float GetDamageMultiplier() 
    {
        if (damageMultiplier == DamageMultiplier.Light) return 0.5f;
        else if (damageMultiplier == DamageMultiplier.Medium) return 1f;
        else if (damageMultiplier == DamageMultiplier.Heavy) return 1.5f;
        else return 0f;
    }

    public AttackType GetAttackType() { return attackType; }

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

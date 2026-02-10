using System;
using UnityEngine;
using UnityEngine.VFX;


[CreateAssetMenu()]
public class Aktion : ScriptableObject
{
    [SerializeField] private string aktionName;
    [SerializeField] private string description;
    [SerializeField] private int pointCost;
    [SerializeField] private bool isUnique;
    public virtual int GetAPCost() { return pointCost; }
    public virtual string GetName() { return aktionName; }
    public virtual string GetDesc() { return description; }
    public virtual bool IsUnique() { return isUnique; }

    [Header("VFX")]
    [SerializeField] private string animationName;
    [SerializeField] private GameObject aktionVFXPrefab;
    [SerializeField] private bool isOnUser; // bool to check for if the vfx plays on the user or not
    [SerializeField] private Vector3 vfxOffset;
    [SerializeField] bool LookAtTarget = true;
    public virtual string GetAnimName() { return animationName; }
    public virtual bool IsOnUser() { return isOnUser; }
    public virtual GameObject GetVFX() { return aktionVFXPrefab; }
    public virtual Vector3 GetVFXOffset() { return vfxOffset; }
    public virtual bool IsLookAt() { return LookAtTarget; } 
}

[Serializable]
public struct StatusEffect
{
    [SerializeField] private StatusType changeType;
    [SerializeField] private Stat statType;
    [SerializeField] private bool isSelfTarget; // true for self, false for enemy
    [SerializeField] private StatBoost boost;
    public StatusType GetStatusType() { return changeType; }
    public Stat GetStatType() { return statType; }
    public bool IsSelfTarget() { return isSelfTarget; }
    public StatBoost GetBoost() { return boost; }

}

[Serializable]
public struct StatBoost
{
    [SerializeField] private int timer;
    [SerializeField] private bool uniqueEffectAmount; // For scenarios that use damage or another stat etc.
    [SerializeField] private string uniqueBoostType; // "Damage", "Stat" etc.
    [SerializeField] private float effectAmount; // Percentage 

    public int GetTimer() { return timer; }
    public bool IsUniqueAmount() { return uniqueEffectAmount; }
    public string GetBoostType() { return uniqueBoostType; }
    public float GetEffectAmount() { return effectAmount; }
}

public enum Stat
{
    None,
    Strength,
    Magic,
    Endurance,
    Speed,
    Crit,
    Health,
    AP
}

public enum StatusType
{
    None,
    Increase,
    Decrease,
    Restore,
    Reduce,
    Reset,
    StatusImmune
}
using System;
using UnityEngine;


[CreateAssetMenu()]
public class Aktion : ScriptableObject
{
    [SerializeField] private string aktionName;
    [SerializeField] private string description;
    [SerializeField] private int pointCost;
    [SerializeField] private bool isUnique;
    [SerializeField] private GameObject aktionVFXPrefab;
    [SerializeField] private Vector3 vfxOffset;
    public virtual int GetAPCost() { return pointCost; } 
    public virtual string GetName() {  return aktionName; }
    public virtual string GetDesc() { return description; }
    public virtual bool IsUnique() { return isUnique; }
    public virtual GameObject GetVFX() { return aktionVFXPrefab; }
    public virtual Vector3 GetVFXOffset() { return vfxOffset; }
}

[Serializable]
public struct StatusEffect
{
    [SerializeField] private StatusType changeType;
    [SerializeField] private Stat statType;
    [SerializeField] private bool isSelfTarget; // true for self, false for enemy
    public StatusType GetStatusType() { return changeType; }
    public Stat GetStatType() { return statType; }
    public bool IsSelfTarget() { return isSelfTarget; }
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
    Reset
}
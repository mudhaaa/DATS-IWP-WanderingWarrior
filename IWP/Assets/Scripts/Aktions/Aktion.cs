using System;
using UnityEngine;

[CreateAssetMenu()]
public class Aktion : ScriptableObject
{
    [SerializeField] private string aktionName;
    [SerializeField] private string description;
    [SerializeField] private float healthCost;
    [SerializeField] private float manaCost;
    public virtual float GetHealthCost() { return healthCost; }
    public virtual float GetManaCost() { return manaCost; }
    public virtual string GetName() {  return aktionName; }
    public virtual string GetDesc() { return description; }

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
    Mana
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
using System.Collections.Generic;
using UnityEngine;

public class StatusAktion : Aktion
{
    // stat to change
    [SerializeField] private List<Stat> statsChange;
    public List<Stat> GetStatChange() { return statsChange; }

    [SerializeField] private StatusType statusType;   
    public StatusType GetStatusType() { return statusType; }

}
public enum Stat
{
    None,
    Strength,
    Magic,
    Endurance,
    Speed,
    Health,
    Mana
}

public enum StatusType
{
    None,
    Increase,
    Decrease,
    Restore,
    Reduce
}
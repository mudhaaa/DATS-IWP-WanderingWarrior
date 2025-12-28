using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class Enhancement : ScriptableObject
{
    [SerializeField] private Sprite enhancementImage;
    [SerializeField] private string enhancementName;
    [SerializeField] private string enhancementDesc;
    [SerializeField] private List<EnhancementEffect> effects;

    public Sprite EnhancementImage() {  return enhancementImage; }
    public string EnhancementName() { return enhancementName; }
    public string EnhancementDesc() { return enhancementDesc; }
    public List<EnhancementEffect> EnhancementEffects() { return effects; }
    public EnhancementEffect EnhancementEffect(int i) { return effects[i]; }
}

public enum EnhancementType 
{
    StatBoost,
    StatDrop,
    AktionGain
}

[Serializable]
public struct EnhancementEffect
{
    [SerializeField] private EnhancementType enhancementType;
    public EnhancementType GetEnhancementType() { return enhancementType; }

    // stat change
    [SerializeField] private Stat changedStat;
    public Stat GetChangedStat() { return changedStat; }
    [SerializeField] private int flatStatChangeValue;
    public int GetFlatStatChangeValue() { return flatStatChangeValue; }
    [SerializeField] private float percentageStatChangeValue;
    public float GetPercentageStatChangeValue() {return percentageStatChangeValue; }

    // aktion gain
    [SerializeField] private Aktion gainedAktion;
    public Aktion GetAktion() { return gainedAktion; } 
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    EnhancementType enhancementType;
    public EnhancementType GetEnhancementType() { return enhancementType; }
    
    // stat change
    Stat changedStat;
    public Stat GetChangedStat() { return changedStat; }
    int flatStatChangeValue;
    public int GetFlatStatChangeValue() { return flatStatChangeValue; }
    float percentageStatChangeValue;
    public float GetPercentageStatChangeValue() {return percentageStatChangeValue; }

    // aktion gain
    Aktion gainedAktion;
    public Aktion GetAktion() { return gainedAktion; } 
}

using System.Collections.Generic;
using   UnityEngine;

[CreateAssetMenu()]
public class StatusAktion : Aktion
{
    [SerializeField] private List<StatusEffect> statusEffects;
    public List<StatusEffect> GetStatusEffectList() {  return statusEffects; }  
    public StatusEffect GetStatusEffect(int i) { return statusEffects[i];  }

}
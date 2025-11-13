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

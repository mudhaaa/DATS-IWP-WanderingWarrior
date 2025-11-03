using UnityEngine;

[CreateAssetMenu()]
public class Aktion : ScriptableObject
{
    [SerializeField] private string aktionName;
    [SerializeField] private string description;

    public virtual string GetName() {  return aktionName; }
    public virtual string GetDesc() { return description; }

}

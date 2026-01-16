using UnityEngine;

[CreateAssetMenu()]
public class CharacterSelectUI : ScriptableObject
{
    [SerializeField] private CharacterKlass characterKlass;
    [SerializeField] private Sprite image;
    [SerializeField] private string text;
    [SerializeField] private string desc;
    
    public CharacterKlass GetKlass() {  return characterKlass; }
    public Sprite GetImage() { return image; }
    public string GetText() { return text; }
    public string GetDesc() { return desc; }
}

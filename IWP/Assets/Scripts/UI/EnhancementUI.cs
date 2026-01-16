using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnhancementUI : MonoBehaviour
{
    [SerializeField] private Image eImage;
    [SerializeField] private TMP_Text eName;
    [SerializeField] private string eDescription;

    public void SetUI(Enhancement e)
    {
        eImage.sprite = e.EnhancementImage();
        eName.text = e.EnhancementName();
        eDescription = e.EnhancementDesc();
    }

    public string GetDesc() {  return eDescription; }
}

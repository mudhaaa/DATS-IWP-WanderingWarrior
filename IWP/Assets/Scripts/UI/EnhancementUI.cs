using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnhancementUI : MonoBehaviour
{
    [SerializeField] private Image eImage;
    [SerializeField] private TMP_Text eName;
    [SerializeField] private TMP_Text eDescription;

    public void SetUI(Enhancement e)
    {
        //eImage.sprite = e.EnhancementImage();
        eName.text = e.EnhancementName();
        //WeDescription.text = e.EnhancementDesc();
    }
}

using TMPro;
using UnityEngine;

public class AktionSlotUI : MonoBehaviour
{
    [SerializeField] private Aktion aktion;
    [SerializeField] private TMP_Text aktionName;
    [SerializeField] private TMP_Text aktionCost;
    [SerializeField] private TMP_Text aktionDesc;

    int cost;
    public void SetText(Aktion aktion)
    {
        this.aktion = aktion;

        cost = aktion.GetAPCost();

        name = aktion.GetName();
        aktionName.text = aktion.GetName();
        aktionCost.text = "Cost: " + cost + "AP";
    }

}

using TMPro;
using UnityEngine;

public class AktionSlotUI : MonoBehaviour
{
    [SerializeField] private Aktion aktion;
    [SerializeField] private TMP_Text aktionName;
    [SerializeField] private TMP_Text aktionCost;
    [SerializeField] private TMP_Text aktionDesc;

    int cost;
    bool costType; // true if mana, false if health
    public void SetText(Aktion aktion)
    {
        this.aktion = aktion;

        costType = aktion.GetManaCost() > 0; 
        cost = costType ? (int)(aktion.GetManaCost()*100) : (int)(aktion.GetHealthCost() * 100);

        name = aktion.GetName();
        aktionName.text = aktion.GetName();
        aktionCost.text = "Cost: " + cost + (costType ? "% Mana" : "% Health");
        aktionDesc.text = aktion.GetDesc();
    }
}

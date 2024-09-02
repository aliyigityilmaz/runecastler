using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUIManager : MonoBehaviour
{
    public TextMeshProUGUI woodAmountText;
    public TextMeshProUGUI stoneAmountText;
    public ResourceSystem resourceSystem;

    void Start()
    {
        UpdateResourceUI();
    }

    public void UpdateResourceUI()
    {
        int woodAmount = resourceSystem.resources.Find(r => r.resourceType == ResourceType.Wood).amount;
        int stoneAmount = resourceSystem.resources.Find(r => r.resourceType == ResourceType.Stone).amount;

        woodAmountText.text = woodAmount.ToString();
        stoneAmountText.text = stoneAmount.ToString();
    }

    // Example function to call when resources change
    public void OnResourceChanged()
    {
        UpdateResourceUI();
    }
}
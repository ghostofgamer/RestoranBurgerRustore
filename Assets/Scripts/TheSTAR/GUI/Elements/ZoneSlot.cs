using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using TheSTAR.GUI;
using TheSTAR.Utility;

public class ZoneSlot : MonoBehaviour
{
    [SerializeField] private PointerButton buyButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject buyGroup;
    [SerializeField] private GameObject ownedGroup;

    [Space]
    [SerializeField] private GameObject availableByExpandContainer;
    [SerializeField] private GameObject availableByLevelContainer;
    [SerializeField] private TextMeshProUGUI neededLevelText;
    [SerializeField] private TextMeshProUGUI neededExpandText;

    private int index;
    private Action<int> onBuyClickAction;

    public int Index => index;

    public void Init(
        int index, 
        string nameText, 
        DollarValue cost, 
        Action<int> onBuyClickAction, 
        ScrollRect scrollRect)
    {
        this.index = index;
        this.nameText.text = nameText;
        costText.text = TextUtility.FormatPrice(cost);
        this.onBuyClickAction = onBuyClickAction;

        buyButton.Init(OnBuyButtonClick);
        buyButton.SetScrollable(scrollRect);
    }

    public void SetVisual(bool owned)
    {
        buyGroup.SetActive(!owned);
        ownedGroup.SetActive(owned);
    }

    private void OnBuyButtonClick()
    {
        onBuyClickAction?.Invoke(index);
    }

    public void SetLockedByLevel(int neededLevelIndex)
    {
        availableByLevelContainer.SetActive(false);
        neededLevelText.gameObject.SetActive(true);
        neededLevelText.text = $"Level {neededLevelIndex + 1} is required";
    }

    public void SetUnlockedByLevel()
    {
        availableByLevelContainer.SetActive(true);
        neededLevelText.gameObject.SetActive(false);
    }

    public void SetLockedByExpand(int neededExpandCount)
    {
        availableByExpandContainer.SetActive(false);
        neededExpandText.gameObject.SetActive(true);
        neededExpandText.text = $"Zone {neededExpandCount} is required";
    }

    public void SetUnlockedByExpand()
    {
        availableByExpandContainer.SetActive(true);
        neededExpandText.gameObject.SetActive(false);
    }
}
using System;
using TheSTAR.Utility;
using TheSTAR.GUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TheSTAR.Sound;

public class ComputerProductUI : MonoBehaviour
{
    [SerializeField] private GameObject _soonPanel;
    [SerializeField] private GameObject _closeTutorPanel;
    [SerializeField] private GameObject _hand;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private PointerButton getButton;
    [SerializeField] private TextMeshProUGUI costPerUnitText;
    [SerializeField] private TextMeshProUGUI costTotalText;
    [SerializeField] private TextMeshProUGUI boxValueText;
    [SerializeField] private Image productIcon;
    [SerializeField] private GameObject buyItemContainer;
    [SerializeField] private TextMeshProUGUI neededLevelText;
    [SerializeField] private ValueSwitcher valueSwitcher;
    [SerializeField] private GameObject tutorPointObject;

    private ItemType itemType;
    private Action<ItemType, int> getAction;

    public ItemType ItemType => itemType;
    public Transform GetTran => getButton.transform;

    private DollarValue oneUnitCost;
    private int boxValue;
    private int boxesCount = 1;
    private TutorialController _tutorialController;
    
    public void Init(SoundController sounds, ItemType itemType, Action<ItemType, int> getAction, ScrollRect scrollRect, TutorialController tutorialController)
    {
        this.itemType = itemType;
        this.getAction = getAction;
        getButton.Init(sounds);
        getButton.Init(OnBuyClick);
        getButton.SetScrollable(scrollRect);
        _tutorialController = tutorialController;
        valueSwitcher.Init(sounds);
        valueSwitcher.Init(OnChangeBoxesValue, scrollRect);
    }

    public void SetVisual(string title, DollarValue oneUnitCost, int boxValue, Sprite icon)
    {
        this.oneUnitCost = oneUnitCost;
        this.boxValue = boxValue;
        titleText.text = title;
        productIcon.sprite = icon;
        costPerUnitText.text = TextUtility.FormatPrice(oneUnitCost, true);
        boxValueText.text = boxValue.ToString();

        UpdateTotalCostText();
    }

    private void UpdateTotalCostText()
    {
        if (boxesCount < 1) boxesCount = 1;
        costTotalText.text = TextUtility.FormatPrice(oneUnitCost * (boxValue * boxesCount), true);
    }
    
    public void SetValueSoonPanel(bool value)
    {
        _soonPanel.SetActive(value);
    }
    
    public void SetValueTutorPanel(bool valuePanel, bool hand)
    {
        _closeTutorPanel.SetActive(valuePanel);
        _hand.SetActive(hand);
    }

    private void OnBuyClick()
    {
        if (boxesCount < 1) boxesCount = 1;
        getAction?.Invoke(itemType, boxesCount);
        
        if (!_tutorialController.IsCompleted(TutorialType.FirstDelivery))
            SetValueTutorPanel(false, false);
    }

    public void SetLocked(int neededLevel)
    {
        buyItemContainer.SetActive(false);
        neededLevelText.gameObject.SetActive(true);
        neededLevelText.text = $"Level {neededLevel + 1} is required";
    }

    public void SetUnlocked()
    {
        buyItemContainer.SetActive(true);
        neededLevelText.gameObject.SetActive(false);
    }

    private void OnChangeBoxesValue(int value)
    {
        this.boxesCount = value;
        UpdateTotalCostText();
    }

    public void SetTutorial(bool tutorial)
    {
        tutorPointObject.SetActive(tutorial);
    }
}
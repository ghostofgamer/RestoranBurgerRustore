using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeBookItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image iconImg;
    [SerializeField] private GameObject lockFrameObject;
    [SerializeField] private Button button;

    private Sprite lockedIcon;
    private Sprite availableIcon;

    private bool available;
    private int index;

    public void Init(int index, string name, Sprite availableIcon, Sprite lockedIcon, Action<int> onClickAction)
    {
        this.index = index;
        nameText.text = name;
        this.availableIcon = availableIcon;
        this.lockedIcon = lockedIcon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (!available) return;
            onClickAction?.Invoke(this.index);
        });
    }

    public void SetAvailable(bool available)
    {
        this.available = available;
        iconImg.sprite = available ? availableIcon : lockedIcon;
        lockFrameObject.SetActive(!available);
    }
}
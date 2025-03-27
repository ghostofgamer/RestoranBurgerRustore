using System;
using UnityEngine;
using UnityEngine.UI;
using TheSTAR.GUI;

public class SelectItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private PointerButton button;

    public void Init(int id, Action<int> clickAction)
    {
        button.Init(() => clickAction?.Invoke(id));
    }

    public void Init(Sprite iconSprite) => iconImg.sprite = iconSprite;
}
using System;
using UnityEngine;
using UnityEngine.UI;
using TheSTAR.Utility;
using TheSTAR.GUI;
using DG.Tweening;
using TMPro;

public class CurrencyCounter : MonoBehaviour
{
    [SerializeField] private PointerButton counterBtn;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image iconImg;
    [SerializeField] private Animator anim;
    [SerializeField] private CounterMessage message;

    private Tweener smoothChangeTweener = null;
    private int? currentValue = null;
    public RectTransform IconTran => iconImg.rectTransform;

    public void Init(Action clickAction)
    {
        counterBtn.Init(clickAction);
    }

    public void SetValue(int value, bool useJumpAnim = true, bool useIncomeMessage = false)
    {
        if (currentValue == null)
        {
            currentValue = value;
            valueText.text = TextUtility.NumericValueToText(value, NumericTextFormatType.CompactFromK);
        }
        else
        {
            if (value > currentValue)
            {
                if (useJumpAnim) anim.SetTrigger("Jump");

                if (useIncomeMessage) IncomeMessage(value - (int)currentValue);
            }

            if (smoothChangeTweener != null)
            {
                smoothChangeTweener.Kill();
                smoothChangeTweener = null;
            }

            smoothChangeTweener = DOVirtual.Int((int)currentValue, value, 1, (tempValue) =>
            {
                valueText.text = TextUtility.NumericValueToText(tempValue, NumericTextFormatType.CompactFromK);
            });
            currentValue = value;
        }
    }

    public void IncomeMessage(int messageValue)
    {
        message.Message(messageValue);
    }
}
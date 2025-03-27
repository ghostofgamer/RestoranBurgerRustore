using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TheSTAR.Utility;

public class IncomeMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Animator anim;

    public void ShowMessage(Vector3 pos, int value)
    {
        if (value <= 0) return;

        transform.position = pos;
        valueText.text = $"+{TextUtility.NumericValueToText(value, NumericTextFormatType.RoundToInt)}";
        gameObject.SetActive(true);
        anim.SetTrigger("Show");
    }
}
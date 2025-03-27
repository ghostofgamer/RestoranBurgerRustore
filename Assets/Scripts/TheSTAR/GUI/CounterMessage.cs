using UnityEngine;
using TMPro;
using TheSTAR.Utility;

public class CounterMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Animator anim;

    public void Message(int value)
    {
        messageText.text = $"+{TextUtility.NumericValueToText(value, NumericTextFormatType.CompactFromK)}";
        anim.SetTrigger("Message");
    }
}
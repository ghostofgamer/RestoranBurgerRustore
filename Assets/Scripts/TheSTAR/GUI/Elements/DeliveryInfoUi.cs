using TheSTAR.GUI;
using TMPro;
using UnityEngine;
using TheSTAR.Utility;
using System;

public class DeliveryInfoUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private PointerButton skipForAdBtn;
    [SerializeField] private PointerButton skipForFreeBtn;

    private const string nearestDeliveryText = "DELIVERY VIA:";

    public void Init(Action onClickSkipForAd, Action onClickSkipForFree)
    {
        skipForAdBtn.Init(onClickSkipForAd);
        skipForFreeBtn.Init(onClickSkipForFree);
    }

    public void SetInfo(GameTimeSpan time)
    {
        topText.text = $"{nearestDeliveryText} {TextUtility.TimeToText(time)}";
    }

    public void SetForFree(bool forFree)
    {
        skipForAdBtn.gameObject.SetActive(!forFree);
        skipForFreeBtn.gameObject.SetActive(forFree);
    }
}
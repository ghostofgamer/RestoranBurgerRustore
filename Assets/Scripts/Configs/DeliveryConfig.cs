using UnityEngine;
using TheSTAR.Utility;
using System;

[CreateAssetMenu(menuName = "Data/Delivery", fileName = "DeliveryConfig")]
public class DeliveryConfig : ScriptableObject
{
    [SerializeField] private UnityDictionary<ItemType, DeliveryData> deliveryItemsData = new();
    //[Obsolete][SerializeField] private GameTimeSpan deliveryDuration;
    [SerializeField] private GameTimeSpan firstDeliveryDuration;
    [SerializeField] private GameTimeSpan beginnerDeliveryDurationMin;
    [SerializeField] private GameTimeSpan beginnerDeliveryDurationMax;
    [SerializeField] private GameTimeSpan defaultDeliveryDurationMin;
    [SerializeField] private GameTimeSpan defaultDeliveryDurationMax;

    public UnityDictionary<ItemType, DeliveryData> DeliveryItemsData => deliveryItemsData;
    //public GameTimeSpan DeliveryDuration => deliveryDuration;
    
    public GameTimeSpan FirstDeliveryDuration => firstDeliveryDuration;
    public GameTimeSpan BeginnerDeliveryDurationMin => beginnerDeliveryDurationMin;
    public GameTimeSpan BeginnerDeliveryDurationMax => beginnerDeliveryDurationMax;
    public GameTimeSpan DefaultDeliveryDurationMin => defaultDeliveryDurationMin;
    public GameTimeSpan DefaultDeliveryDurationMax => defaultDeliveryDurationMax;
}
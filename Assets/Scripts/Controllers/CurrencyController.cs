using System;
using System.Collections.Generic;
using UnityEngine;
using TheSTAR.Data;
using TheSTAR.Utility;
using DG.Tweening;
using ReputationContent;
using Zenject;

public class CurrencyController
{
    private ConfigHelper<GameConfig> gameConfig = new();

    private event Action<CurrencyType, DollarValue> OnChangeCurrencyEvent;
    
    public event Action<DollarValue> OnCurrencyChanged;
    public event Action<DollarValue> OnTipsChanged;

    public event Action<DollarValue> CurrenceReduced;

    public DollarValue Coins => GetCurrencyValue(CurrencyType.Soft);

    private DataController data;
    private AdsManager ads;
    private Reputation _reputation;

    [Inject]
    private void Constuct(DataController data, AdsManager ads)
    {
        this.data = data;
        this.ads = ads;
    }

    public void AddCurrency(DollarValue value) => AddCurrency(default, value);

    public void AddCurrency(CurrencyType currencyType, DollarValue value, bool autoSave = true)
    {
        Debug.Log("деньги сумма " + value);

        if (_reputation != null)
        {
            Debug.Log("репутация заведения  " + _reputation.StarsRestaurant);
            
            float tipsPercentage = Mathf.Max(0, _reputation.StarsRestaurant) * 0.03f;
            
            int totalCents = value.dollars * 100 + value.cents;
            
            float tipsInCents = totalCents * tipsPercentage;
            int tipsDollars = Mathf.FloorToInt(tipsInCents / 100);
            int tipsCents = Mathf.FloorToInt(tipsInCents % 100);
            DollarValue tipsValue = new DollarValue { dollars = tipsDollars, cents = tipsCents };
            OnTipsChanged?.Invoke(tipsValue);
            Debug.Log("чаевые " + tipsValue);
            
            float finalValueInCents = totalCents * (1 + Mathf.Max(0, _reputation.StarsRestaurant) * 0.03f);
            int finalDollars = Mathf.FloorToInt(finalValueInCents / 100);
            int finalCents = Mathf.FloorToInt(finalValueInCents % 100);
            value = new DollarValue { dollars = finalDollars, cents = finalCents };
            Debug.Log("FinalValue " + value);
        }
        
        OnCurrencyChanged?.Invoke(value);

        if (value.dollars < 0)
        {
            ReduceCurrency(currencyType, new(-value.dollars, -value.cents));
            return;
        }

        if (IncomeX2) value *= 2;

        data.gameData.currencyData.AddCurrency(currencyType, value, out var result);
        if (autoSave) data.Save(DataSectionType.Currency);

        Reaction(currencyType, result);
    }

    public void AddCurrencyFastMoney(CurrencyType currencyType, DollarValue value, bool autoSave = true)
    {
        OnCurrencyChanged?.Invoke(value);

        if (value.dollars < 0)
        {
            ReduceCurrency(currencyType, new(-value.dollars, -value.cents));
            return;
        }

        if (IncomeX2) value *= 2;

        data.gameData.currencyData.AddCurrency(currencyType, value, out var result);
        if (autoSave) data.Save(DataSectionType.Currency);

        Reaction(currencyType, result);
    }

    public void ReduceCurrency(CurrencyType currencyType, DollarValue count, Action completeAction = null,
        Action failAction = null)
    {
        if (data.gameData.currencyData.GetCurrencyCount(currencyType) >= count)
        {
            data.gameData.currencyData.AddCurrency(currencyType, new DollarValue(-count.dollars, -count.cents),
                out var result);

            //if (autoSave) 
            data.Save(DataSectionType.Currency);

            completeAction?.Invoke();

            Reaction(currencyType, result);
            CurrenceReduced?.Invoke(count);
        }
        else failAction?.Invoke();
    }

    public void InitReputation(Reputation reputation)
    {
        _reputation = reputation;

        if (_reputation != null)
        {
            Debug.Log("Rep Init");
        }
    }

    public void ClearCurrency(CurrencyType currencyType)
    {
        var count = GetCurrencyValue(currencyType);
        data.gameData.currencyData.AddCurrency(currencyType, new(-count.dollars, -count.cents), out var result);

        Reaction(currencyType, result);
    }

    public DollarValue GetCurrencyValue(CurrencyType currencyType)
    {
        return data.gameData.currencyData.GetCurrencyCount(currencyType);
    }

    /*
    private void InitReaction()
    {
        var currencyTypes = EnumUtility.GetValues<CurrencyType>();

        DollarValue currencyCount;

        foreach (var currencyType in currencyTypes)
        {
            currencyCount = data.gameData.currencyData.GetCurrencyCount(currencyType);

            OnChangeCurrencyEvent?.Invoke(currencyType, currencyCount);
        }
    }
    */

    public void Reaction(CurrencyType currencyType, DollarValue finalValue)
    {
        OnChangeCurrencyEvent?.Invoke(currencyType, finalValue);
    }

    #region Cheat

    [ContextMenu("CheatAddCurrency")]
    private void CheatAddCurrency()
    {
        AddCurrency(CurrencyType.Soft, new(100, 0), true);
    }

    #endregion

    public void StartSimulateIncomeOffer()
    {
        WaitForShowOffer();
    }

    private void WaitForShowOffer()
    {
        DOVirtual.Float(0f, 1f, gameConfig.Get.X2BonusOfferPeriod.TotalSeconds, (value) => { }).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (!IncomeX2)
                {
                    // показываем оффер на 10 секунд
                    ShowIncomeX2OfferEvent();
                    DOVirtual.Float(0f, 1f, gameConfig.Get.ShowX2BonusOfferDuration.TotalSeconds, (value) => { })
                        .SetEase(Ease.Linear).OnComplete(() => { HideIncomeX2OfferEvent(); });
                }

                WaitForShowOffer();
            });
    }

    private bool incomeX2;
    public bool IncomeX2 => incomeX2;

    public void TryGiveIncomeBonusForAds()
    {
        if (IncomeX2) return;

        ads.ShowRewarded("Income x2", (success) =>
        {
            if (success) SetIncomeBonus(gameConfig.Get.X2BonuxDuration.ToTimeSpan());
        });
    }

    private void SetIncomeBonus(TimeSpan duration)
    {
        incomeX2 = true;

        //Debug.Log("Income x2 to " + x2BonusCompleteTime);

        DOVirtual.Int((int)duration.TotalSeconds, 0, (int)duration.TotalSeconds, (value) => IncomeX2Tick(value))
            .SetEase(Ease.Linear).OnComplete(() =>
            {
                incomeX2 = false;
                CompleteIncomeEffectEvent?.Invoke();
            });

        StartIncomeEffectEvent?.Invoke();
    }

    public void Subscribe(Action<CurrencyType, DollarValue> action)
    {
        OnChangeCurrencyEvent += action;

        var currencyTypes = EnumUtility.GetValues<CurrencyType>();
        DollarValue currencyCount;
        foreach (var currencyType in currencyTypes)
        {
            currencyCount = data.gameData.currencyData.GetCurrencyCount(currencyType);
            action.Invoke(currencyType, currencyCount);
        }
    }

    public event Action ShowIncomeX2OfferEvent;
    public event Action HideIncomeX2OfferEvent;
    public event Action StartIncomeEffectEvent;
    public event Action<int> IncomeX2Tick;
    public event Action CompleteIncomeEffectEvent;
}

[Serializable]
public struct CurrencyValue
{
    public CurrencyType rewardType;

    //[HideIf("useRange")] 
    public int value;

    public bool useRange;

    //[ShowIf("useRange")]
    public IntRange valueRange;
}

public interface ICurrencyTransactionReactable
{
    void OnTransactionReact(CurrencyType currencyType, DollarValue finalValue);
}

public enum CurrencyType
{
    Soft
}
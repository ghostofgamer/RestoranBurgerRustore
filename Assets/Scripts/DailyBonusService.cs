using System;
using Zenject;
using TheSTAR.Data;

public class DailyBonusService
{
    private DataController data;
    private NotificationController notifications;

    [Inject]
    private void Construct(DataController data, NotificationController notifications)
    {
        this.data = data;
        this.notifications = notifications;
    }

    private const int MaxDay = 12;

    private const bool RestartAfterSkip = false; // надо ли начинать дейлики сначала если игрок пропустил день

    public bool NeedShowDailyBonus
    {
        get
        {
            DateTime previousShowTime = GetPreviousShowTime();
            TimeSpan timePassed = DateTime.Today - previousShowTime;

            int passedDays = (int)timePassed.TotalDays;
            int currentBonusIndex = GetCurrentBonusIndex();

            if (passedDays < 1) return false;
            else if (passedDays == 1 || !RestartAfterSkip)
            {
                bool indexWasUpdated = GetWasBonusIndexWasUpdatedForThisDay();

                if (!indexWasUpdated)
                {
                    currentBonusIndex++;
                    if (currentBonusIndex >= MaxDay) currentBonusIndex = 0;

                    SetCurrentBonusIndex(currentBonusIndex);
                    SetWasBonusIndexWasUpdatedForThisDay(true);
                }
                
                return true;
            }
            else
            {
                currentBonusIndex = 0;
                SetCurrentBonusIndex(currentBonusIndex);
                return true;
            }
        }
    }

    private DateTime GetPreviousShowTime()
    {
        return data.gameData.dailyBonusData.previousDailyBonusTime;
    }

    private bool GetWasBonusIndexWasUpdatedForThisDay()
    {
        return data.gameData.dailyBonusData.bonusIndexWasUpdatedForThisDay;
    }

    private void SetWasBonusIndexWasUpdatedForThisDay(bool value)
    {
        data.gameData.dailyBonusData.bonusIndexWasUpdatedForThisDay = value;
    }

    private void SetDateTime(DateTime time)
    {
        data.gameData.dailyBonusData.previousDailyBonusTime = time;
    }

    public int GetCurrentBonusIndex()
    {
        return data.gameData.dailyBonusData.currentDailyBonusIndex;
    }

    private void SetCurrentBonusIndex(int i)
    {
        data.gameData.dailyBonusData.currentDailyBonusIndex = i;
    }

    public void OnGetDailyReward()
    {
        SetDateTime(DateTime.Today);
        SetWasBonusIndexWasUpdatedForThisDay(false);
        SetNextDailyBonusNotification();
        data.Save(DataSectionType.DailyBonus);
    }

    public void SetNextDailyBonusNotification()
    {
        notifications.CancelNotification(NotificationType.DailyBonus);
        DateTime notificationTime = DateTime.Today.AddDays(1);
        notificationTime = notificationTime.AddHours(12); // о дейликах напоминаем в 12 часов
        notifications.RegisterNotification(NotificationType.DailyBonus, notificationTime);
    }

    public CurrencyType? ConvertToCurrencyType(DailyRewardType rewardType)
    {
        switch (rewardType)
        {
            case DailyRewardType.Soft: return CurrencyType.Soft;
            //case DailyRewardType.Hard: return CurrencyType.Hard;
        }

        return null;
    }
}

public enum DailyRewardType
{
    Soft,
    Hard
}
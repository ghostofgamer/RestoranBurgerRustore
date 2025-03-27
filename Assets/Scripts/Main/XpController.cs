using System;
using TheSTAR.Data;
using TheSTAR.Sound;
using TheSTAR.Utility;
using UnityEngine;
using Zenject;

public class XpController
{    
    private ConfigHelper<XpConfig> xpConfig = new();

    // todo use save load
    private int currentLevel = 0;
    private int currentXp;

    // current level, current xp, max xp
    private event Action<int, int, int> onChangeXpEvent;
    public event Action<int> OnLevelUpEvent;
    
    public event Action<int> OnProfitXpValue;

    public int CurrentLevel => currentLevel;

    private DataController data;
    private AnalyticsManager analytics;
    private SoundController sounds;

    [Inject]
    private void Consruct(DataController data, AnalyticsManager analytics, SoundController sounds)
    {
        this.data = data;
        this.analytics = analytics;
        this.sounds = sounds;
    }

    public void Load()
    {
        currentLevel = data.gameData.levelData.currentPlayerLevel;
        currentXp = data.gameData.levelData.currentPlayerXp;

        onChangeXpEvent?.Invoke(currentLevel, currentXp, GetNeededXpToLevelUp());
    }

    public void AddXp(int value)
    {
        OnProfitXpValue?.Invoke(value);
        currentXp += value;
        bool levelUp = false;

        while (currentXp >= GetNeededXpToLevelUp())
        {
            currentXp -= GetNeededXpToLevelUp();
            currentLevel++;
            levelUp = true;

            analytics.Trigger(RepeatingEventType.LevelUp);
        }

        data.gameData.levelData.currentPlayerLevel = currentLevel;
        data.gameData.levelData.currentPlayerXp = currentXp;
        data.Save(TheSTAR.Data.DataSectionType.Level);

        onChangeXpEvent?.Invoke(currentLevel, currentXp, GetNeededXpToLevelUp());
        
        if (levelUp)
        {
            sounds.Play(SoundType.level_up);
            OnLevelUpEvent?.Invoke(currentLevel);
        }
    }

    public int GetNeededXpToLevelUp()
    {
        if (currentLevel + 1 < xpConfig.Get.XpToLevelUp.Length) return xpConfig.Get.XpToLevelUp[currentLevel + 1];
        else return xpConfig.Get.XpToLevelUp[^1];
    }

    public void SubscribeOnChangeXp(Action<int, int, int> onChangeXpAction)
    {
        onChangeXpEvent += onChangeXpAction;
        onChangeXpAction?.Invoke(currentLevel, currentXp, GetNeededXpToLevelUp());
    }

    public void SubscribeOnLevelUp(Action<int> onLevelUpAction)
    {
        OnLevelUpEvent += onLevelUpAction;
        onLevelUpAction?.Invoke(currentLevel);
    }
}
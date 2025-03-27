using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TheSTAR.Utility;

namespace TheSTAR.Data
{
    [Serializable]
    public sealed class DataController
    {
        private bool lockSaves = false; // когда true, перезапись сохранений заблокирована, файлы сохранений не могут быть изменены

        public void Init(bool lockSaves)
        {
            this.lockSaves = lockSaves;
            
            if (clearData) LoadDefault();
            else LoadAll();
        }

        public void LoadGame(bool clearData = false)
        {
            if (clearData) LoadDefault();
            else LoadAll();
        }        
        
        [ContextMenu("Save")]
        private void ForceSave()
        {
            SaveAll(true);
        }

        public void SaveAll(bool force = false)
        {
            var allSections = EnumUtility.GetValues<DataSectionType>();
            foreach (var section in allSections) Save(section, force);
        }

        public void Save(DataSectionType secion, bool force = false)
        {
            if (!force && lockSaves) return;

            JsonSerializerSettings settings = new() { TypeNameHandling = TypeNameHandling.Objects };
            string jsonString = JsonConvert.SerializeObject(gameData.GetSection(secion), Formatting.Indented, settings);

            PlayerPrefs.SetString(secion.ToString(), jsonString);

            //Debug.Log($"Save {secion}");
        }

        [ContextMenu("Load")]
        private void LoadAll()
        {
            if (PlayerPrefs.HasKey(DataSectionType.Common.ToString()))
            {
                var allSections = EnumUtility.GetValues<DataSectionType>();
                foreach (var section in allSections) LoadSection(section);
            }
            else LoadDefault();

            void LoadSection(DataSectionType section)
            {
                string jsonString = PlayerPrefs.GetString(section.ToString());
                var loadedData = JsonConvert.DeserializeObject<DataSection>(jsonString, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
                gameData.SetSection(loadedData);
            }
        }

        [ContextMenu("ClearData")]
        private void LoadDefault()
        {
            gameData = new();
            SaveAll();
        }

        [Header("Data")]
        public GameData gameData = new();

        [SerializeField] private bool clearData = false;

        [Serializable]
        public class GameData
        {
            public CommonData commonData;
            public SettingsData settingsData;
            public CurrencyData currencyData;
            public LevelData levelData;
            public InappsData inappsData;
            public AnalyticsData analyticsData;
            public TutorialData tutorialData;
            public NotificationData notificationData;
            public DailyBonusData dailyBonusData;

            public GameData()
            {
                commonData = new();
                settingsData = new();
                currencyData = new();
                levelData = new();
                inappsData = new();
                analyticsData = new();
                tutorialData = new();
                notificationData = new();
                dailyBonusData = new();
            }

            public DataSection GetSection(DataSectionType sectionType)
            {
                switch (sectionType)
                {
                    case DataSectionType.Common: return commonData;
                    case DataSectionType.Settings: return settingsData;
                    case DataSectionType.Currency: return currencyData;
                    case DataSectionType.Level: return levelData;
                    case DataSectionType.InappsData: return inappsData;
                    case DataSectionType.AnalyticsData: return analyticsData;
                    case DataSectionType.Tutorial: return tutorialData;
                    case DataSectionType.Notification: return notificationData;
                    case DataSectionType.DailyBonus: return dailyBonusData;
                    default:
                        break;
                }

                return null;
            }
            public void SetSection(DataSection sectionData)
            {
                switch (sectionData.SectionType)
                {
                    case DataSectionType.Common:
                        commonData = (CommonData)sectionData;
                        break;
                    case DataSectionType.Settings:
                        settingsData = (SettingsData)sectionData;
                        break;
                    case DataSectionType.Currency:
                        currencyData = (CurrencyData)sectionData;
                        break;
                    case DataSectionType.Level:
                        levelData = (LevelData)sectionData;
                        break;
                    case DataSectionType.InappsData:
                        inappsData = (InappsData)sectionData;
                        break;
                    case DataSectionType.AnalyticsData:
                        analyticsData = (AnalyticsData)sectionData;
                        break;
                    case DataSectionType.Tutorial:
                        tutorialData = (TutorialData)sectionData;
                        break;
                    case DataSectionType.Notification:
                        notificationData = (NotificationData)sectionData;
                        break;
                    case DataSectionType.DailyBonus:
                        dailyBonusData = (DailyBonusData)sectionData;
                        break;
                }
            }
        }

        [Serializable]
        public abstract class DataSection
        {
            public abstract DataSectionType SectionType { get; }
        }

        [Serializable]
        public class CommonData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.Common;

            //public bool gdprAccepted;
            public bool gameRated;
            public List<int> rateUsLevels; // для каких уровней был показан RateUs
            public int currentLevelIndex;
            public List<int> startedLevels; // какие уровни игрок уже начинал
            public bool rateUsPlanned;
            public DateTime nextRateUsPlan;
            public bool gameStarted;

            public CommonData()
            {
                rateUsLevels = new();
                startedLevels = new();
            }
        }

        [Serializable]
        public class SettingsData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.Settings;

            public bool isMusicOn = true;
            public bool isSoundsOn = true;
            public bool isVibrationOn = true;
            public bool isNotificationsOn = true;
        }

        [Serializable]
        public class ItemCountData
        {
            public ItemType ItemType;
            public int Count;

            public ItemCountData(ItemType itemType, int count)
            {
                ItemType = itemType;
                Count = count;
            }
        }

        [Serializable]
        public class CurrencyData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.Currency;

            public Dictionary<CurrencyType, DollarValue> currencyData;

            public CurrencyData() => currencyData = new();

            public void AddCurrency(CurrencyType currencyType, DollarValue count, out DollarValue result)
            {
                if (currencyData.ContainsKey(currencyType)) currencyData[currencyType] += count;
                else currencyData.Add(currencyType, count);

                result = currencyData[currencyType];
            }

            public DollarValue GetCurrencyCount(CurrencyType currencyType)
            {
                if (currencyData.ContainsKey(currencyType)) return currencyData[currencyType];
                else return new();
            }
        }

        // Данные по прогрессу игрока в рамках уровня
        [Serializable]
        public class LevelData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.Level;

            public string fastFoodName;

            public Dictionary<BuyerPlaceType, bool[]> activeBuyerPlaces;
            public bool coffeeMachinePurchased;
            public bool deepFryerPurchased;
            public bool sodaMachinePurchased;
            public int completedOrdersCount = 0;
            public int purchasedExpandsCount = 0; // сколько раз игрок купил расширение территории магазина

            public int currentPlayerLevel;
            public int currentPlayerXp;

            public int currentDeliveryIndex;
            public Dictionary<ItemType, int> itemsInCurrentDelivery = new();
            public DateTime timeForNextDelivery;
            public bool deliveryInProcess;

            public Dictionary<ItemType, DollarValue> prices = new();

            public List<BoxInGameData> boxes = new();
            public List<ItemsContainerInGameData> itemContainers = new();
        }

        [Serializable]
        public class InappsData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.InappsData;

            public bool adsFreePurchased = false;
        }

        [Serializable]
        public class AnalyticsData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.AnalyticsData;

            public Dictionary<RepeatingEventType, int> repeatingEventsData = new();
            public Dictionary<SingleEventType, bool> singleEventsData = new();
        }

        [Serializable]
        public class TutorialData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.Tutorial;

            public List<TutorialType> completedTutorials = new();
            //public bool coffeePriceChanged = false;
            //public bool donutPriceChanged = false;
            public int completedOrdersForTutorial = 0;

            public void CompleteTutorial(TutorialType tutorialType)
            {
                if (!completedTutorials.Contains(tutorialType)) completedTutorials.Add(tutorialType);
            }

            public void UncompleteTutorial(TutorialType tutorialType)
            {
                if (completedTutorials.Contains(tutorialType)) completedTutorials.Remove(tutorialType);
            }
        }

        [Serializable]
        public class NotificationData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.Notification;

            /// <summary>
            /// Хранит id зарегестрированных нотификаций. Если id равен -1, значит нотификация неактивна (например, она была отменена)
            /// </summary>
            public Dictionary<NotificationType, int> registredNotifications;

            public void ClearNotification(NotificationType notificationType) => RegisterNotification(notificationType, -1);

            public void RegisterNotification(NotificationType notificationType, int id)
            {
                if (registredNotifications == null) registredNotifications = new Dictionary<NotificationType, int>();

                if (registredNotifications.ContainsKey(notificationType)) registredNotifications[notificationType] = id;
                else registredNotifications.Add(notificationType, id);
            }

            /// <summary>
            /// Возвращает id зарегестрированной нотификации. Если нотификация не зарегестрирована, возвращает -1
            /// </summary>
            public int GetRegistredNotificationID(NotificationType notificationType)
            {
                if (registredNotifications == null)
                {
                    registredNotifications = new Dictionary<NotificationType, int>();
                    return -1;
                }

                if (registredNotifications.ContainsKey(notificationType)) return registredNotifications[notificationType];
                else return -1;
            }
        }

        [Serializable]
        public class DailyBonusData : DataSection
        {
            public override DataSectionType SectionType => DataSectionType.DailyBonus;

            public DateTime previousDailyBonusTime;
            public int currentDailyBonusIndex = -1;
            public bool bonusIndexWasUpdatedForThisDay;
        }
    }

    [Serializable]
    public struct LevelEventInProcessData
    {
        public int levelEventIndex;
        public int additionalID; // может быть необходимым для некоторых типов событий уровня

        public LevelEventInProcessData(int levelEventIndex, int additionalID)
        {
            this.levelEventIndex = levelEventIndex;
            this.additionalID = additionalID;
        }
    }

    public enum DataSectionType
    {
        Common,
        Settings,
        Currency,
        Level,
        InappsData,
        AnalyticsData,
        Tutorial,
        Notification,
        DailyBonus
    }

    [Serializable]
    public struct SerializedVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializedVector3(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public Vector3 ToVector3() => new (x, y, z);
    }

    [Serializable]
    public class BoxInGameData
    {
        public ItemType itemType;
        public int itemsCount;
        public SerializedVector3 position;

        public BoxInGameData()
        {}

        public BoxInGameData(ItemType itemType, int itemsCount, Vector3 pos)
        {
            this.itemType = itemType;
            this.itemsCount = itemsCount;
            this.position = new(pos);
        }
    }

    [Serializable]
    public class ItemsContainerInGameData
    {
        public List<ItemInGameData> items = new();
    }

    [Serializable]
    public class ItemInGameData
    {
        public ItemType itemType;

        public ItemInGameData()
        {}

        public ItemInGameData(ItemType itemType)
        {
            this.itemType = itemType;
        }
    }

    [Serializable]
    public class EmbeddableItemInGameData : ItemInGameData
    {
        public int count;

        public EmbeddableItemInGameData()
        {}

        public EmbeddableItemInGameData(ItemType itemType, int count)
        {
            this.itemType = itemType;
            this.count = count;
        }
    }
}
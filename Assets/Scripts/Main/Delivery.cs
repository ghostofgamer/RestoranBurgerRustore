using UnityEngine;
using TheSTAR.Utility;
using Configs;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using TheSTAR.Data;
using Zenject;
using TheSTAR.GUI;
using TheSTAR.Sound;

public class Delivery : MonoBehaviour
{
    [SerializeField] private Transform deliveryPoint;
    [SerializeField] private Transform beginnerPoint;
    [SerializeField] private Transform beginnerPointTutor;
    
    private readonly ConfigHelper<GameConfig> gameConfig = new();
    private readonly ConfigHelper<ItemsConfig> itemsConfig = new();
    private readonly ConfigHelper<DeliveryConfig> deliveryConfig = new();

    public event Action<GameTimeSpan, int> OnStartDeliveryEvent;
    public event Action<int> OnDeliveryTickEvent;
    public event Action OnCompleteDeliveryEvent;

    private int currentDeliveryIndex = 0;
    private Dictionary<ItemType, int> currentDeliveryItems;
    private DateTime completeCurrentDeliveryTime;
    private bool deliveryInProcess = false;

    private List<Box> activeBoxes = new();
    private Tweener waitForDeliveryTweener;

    public int CurrentDeliveryIndex => currentDeliveryIndex;
    public bool DeliveryInProcess => deliveryInProcess;
    public Box GetAnyBox => activeBoxes.Count == 0 ? null : activeBoxes[0];
    public Box GetLastBox => activeBoxes.Count == 0 ? null : activeBoxes[^1];

    private DataController data;
    private CurrencyController currency;
    private XpController xp;
    private GuiController gui;
    private ItemsController items;
    private DiContainer diContainer;
    private AdsManager ads;
    private SoundController sounds;
    private DateTime pauseTime;
    
    public event Action OnDeleteBox;

    [Inject]
    private void Consruct(
        DataController data, 
        CurrencyController currency, 
        XpController xp,
        GuiController gui,
        ItemsController items,
        DiContainer diContainer,
        AdsManager ads,
        SoundController sounds)
    {
        this.data = data;
        this.currency = currency;
        this.xp = xp;
        this.gui = gui;
        this.items = items;
        this.diContainer = diContainer;
        this.ads = ads;
        this.sounds = sounds;
    }

    /*
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Game is paused
            pauseTime = DateTime.Now;
        }
        else
        {
            // Game is resumed
            if (deliveryInProcess)
            {
                TimeSpan pauseDuration = DateTime.Now - pauseTime;
                completeCurrentDeliveryTime = completeCurrentDeliveryTime.Subtract(pauseDuration);

                if (DateTime.Now >= completeCurrentDeliveryTime)
                {
                    OnCompleteWaitForDelivery();
                }
                else
                {
                    var timeToCompleteDelivery = new GameTimeSpan(completeCurrentDeliveryTime - DateTime.Now);
                    CallDelivery(currentDeliveryItems, timeToCompleteDelivery, true);
                }
            }
        }
    }
    */
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // Game lost focus (similar to pause)
            pauseTime = DateTime.Now;
        }
        else
        {
            // Game gained focus (similar to resume)
            if (deliveryInProcess)
            {
                TimeSpan pauseDuration = DateTime.Now - pauseTime;
                completeCurrentDeliveryTime = completeCurrentDeliveryTime.Subtract(pauseDuration);

                if (DateTime.Now >= completeCurrentDeliveryTime)
                {
                    OnCompleteWaitForDelivery();
                }
                else
                {
                    var timeToCompleteDelivery = new GameTimeSpan(completeCurrentDeliveryTime - DateTime.Now);
                    CallDelivery(currentDeliveryItems, timeToCompleteDelivery, true);
                }
            }
        }
    }
    
    public void Load()
    {
        // delivery
        currentDeliveryIndex = data.gameData.levelData.currentDeliveryIndex;
        currentDeliveryItems = data.gameData.levelData.itemsInCurrentDelivery;
        completeCurrentDeliveryTime = data.gameData.levelData.timeForNextDelivery;
        deliveryInProcess = data.gameData.levelData.deliveryInProcess;

        if (deliveryInProcess)
        {
            if (DateTime.Now >= completeCurrentDeliveryTime) 
                OnCompleteWaitForDelivery();
            else
            {
                var timeToCompleteDelivery = new GameTimeSpan(completeCurrentDeliveryTime - DateTime.Now);
                CallDelivery(currentDeliveryItems, timeToCompleteDelivery, true);
            }
        }

        // boxes

        foreach (var box in data.gameData.levelData.boxes)
        {
            SpawnDeliveryBox(box.itemType, box.itemsCount, box.position.ToVector3());
        }
    }

    public void Save()
    {
        if (quit) return;
        // wait for delivery
        data.gameData.levelData.currentDeliveryIndex = currentDeliveryIndex;
        data.gameData.levelData.itemsInCurrentDelivery = currentDeliveryItems;
        data.gameData.levelData.timeForNextDelivery = completeCurrentDeliveryTime;
        data.gameData.levelData.deliveryInProcess = deliveryInProcess;
    
        // boxes

        List<BoxInGameData> boxInGameDatas = new();

        foreach (var box in activeBoxes)
        {
            if (box == null) continue;

            boxInGameDatas.Add(new (box.ItemType, box.ItemsCount, box.transform.position));
        }

        data.gameData.levelData.boxes = boxInGameDatas;

        data.Save(DataSectionType.Level);
    }

    public void TryBuyProductsForDelivery(Dictionary<ItemType, int> shoppingCart)
    {
        DollarValue totalCost = new();

        int totalXpReward = 0;

        foreach (var element in shoppingCart)
        {
            var itemData = itemsConfig.Get.Item(element.Key);
            totalCost += itemData.CostData.BuyCost * (itemData.OtherData.BoxValue * element.Value);
            totalXpReward += itemData.XpData.BuyXpReward * element.Value;
        }
        
        currency.ReduceCurrency(CurrencyType.Soft, totalCost, () =>
        {
            sounds.Play(SoundType.Oplata_korsini);
            xp.AddXp(totalXpReward);
            CallDelivery(shoppingCart, GetCurrentDeliveryDuration(), false);
            gui.ShowMainScreen();
        }, () =>
        {
            sounds.Play(SoundType.Oshibka);
        });
    }

    private GameTimeSpan GetCurrentDeliveryDuration()
    {
        if (currentDeliveryIndex == 0) return deliveryConfig.Get.FirstDeliveryDuration;
        else if (currentDeliveryIndex <= 10) return GetRandomDuration(deliveryConfig.Get.BeginnerDeliveryDurationMin, deliveryConfig.Get.BeginnerDeliveryDurationMax);
        else return GetRandomDuration(deliveryConfig.Get.DefaultDeliveryDurationMin, deliveryConfig.Get.DefaultDeliveryDurationMax);
    }

    private GameTimeSpan GetRandomDuration(GameTimeSpan min, GameTimeSpan max)
    {
        return new (Random.Range(min.TotalSeconds, max.TotalSeconds));
    }

    private void CallDelivery(Dictionary<ItemType, int> shoppingCart, GameTimeSpan duration, bool load)
    {
        Debug.Log("Call Delivery");

        if (deliveryInProcess && !load)
        {
            currentDeliveryItems = ArrayUtility.Merge(currentDeliveryItems, shoppingCart);
            return;
        }

        currentDeliveryItems = shoppingCart;

        waitForDeliveryTweener?.Kill();

        waitForDeliveryTweener =
        DOVirtual.Int(duration.TotalSeconds, 0, duration.TotalSeconds, OnDeliveryTick).SetEase(Ease.Linear).OnComplete(() =>
        {
            OnCompleteWaitForDelivery();
        });

        OnStartDeliveryEvent?.Invoke(duration, currentDeliveryIndex);

        completeCurrentDeliveryTime = DateTime.Now.AddSeconds(duration.TotalSeconds);
        deliveryInProcess = true;
        Save();
    }

    private void OnCompleteWaitForDelivery()
    {
        sounds.Play(SoundType.Dostavka);
        
        ItemType firstItem = default;
        
        foreach (var element in currentDeliveryItems)
        {
            if (element.Value > 0)
            {
                firstItem = element.Key;
                break;
            }
        }

        var itemData = itemsConfig.Get.Item(firstItem);
        SpawnDeliveryBox(firstItem, itemData.OtherData.BoxValue, false);
        deliveryInProcess = false;
        currentDeliveryIndex++;

        OnCompleteDeliveryEvent?.Invoke();

        ArrayUtility.ReduceValue(currentDeliveryItems, firstItem, 1, true);
        if (currentDeliveryItems.Count == 0) return;

        CallDelivery(currentDeliveryItems, GetCurrentDeliveryDuration(), false);
    }

    public void SpawnDeliveryBox(Dictionary<ItemType, int> shoppingCart, bool toBeginnerPos)
    {
        Debug.Log("SpawnDeliveryBox");
        foreach (var element in shoppingCart)
        {
            for (int i = 0; i < element.Value; i++)
            {
                var itemData = itemsConfig.Get.Item(element.Key);
                int unitsInBox = itemData.OtherData.BoxValue;
                SpawnDeliveryBox(element.Key, unitsInBox, toBeginnerPos);
            }
        }

        Save();
    }

    private void SpawnDeliveryBox(ItemType itemType, int unitsInBox, bool toBeginnerPos)
    {
        SpawnDeliveryBox(itemType, unitsInBox, toBeginnerPos ? beginnerPoint.position : deliveryPoint.position);
    }
    
    public void SpawnDeliveryBox(ItemType itemType, int unitsInBox, bool toBeginnerPos,bool tutor)
    {
        SpawnDeliveryBox(itemType, unitsInBox,beginnerPointTutor.position );
    }

    private void SpawnDeliveryBox(ItemType itemType, int unitsInBox, Vector3 to)
    {
        var itemData = itemsConfig.Get.Item(itemType);
        var deliveryData = deliveryConfig.Get.DeliveryItemsData.Get(itemType);
        bool useBox = deliveryData.boxPrefab;
        
        Box box = null;
        if (useBox)
        {
            box = diContainer.InstantiatePrefabForComponent<Box>(deliveryData.boxPrefab, to, Quaternion.identity, transform);
            box.OnDestroyEvent += OnDestroyBox;
            box.OnEndDragEvent += () => Save();
            activeBoxes.Add(box);
            box.Init(itemType);
        }

        for (int i = 0; i < unitsInBox; i++)
        {
            var item = items.CreateItem(itemType, to);
            if (useBox) box.SetToBox(item);
        }
    }

    private void OnDeliveryTick(int timeSecondsLeft)
    {
        OnDeliveryTickEvent?.Invoke(timeSecondsLeft);
    }

    private void OnDestroyBox(Box box)
    {
        if (activeBoxes.Contains(box))
        {
            activeBoxes.Remove(box);
            Save();
        }

        OnDeleteBox?.Invoke();
    }

    public void TrySkipForAd()
    {
        ads.ShowRewarded("skip delivery", (success) =>
        {
            if (!success) return;
            DoSkip(true);
        });
    }

    public void SkipForFree()
    {
        DoSkip(true);
    }

    [ContextMenu("DoSkip")]
    private void DoSkip(bool skipAll)
    {
        if (!deliveryInProcess) return;

        if (skipAll)
        {
            waitForDeliveryTweener?.Kill(false);

            SpawnDeliveryBox(currentDeliveryItems, false);
            deliveryInProcess = false;
            currentDeliveryIndex++;

            OnCompleteDeliveryEvent?.Invoke();
        }
        else waitForDeliveryTweener?.Kill(true);
    }

    public Box FindBox(ItemType contentType)
    {
        foreach (var box in activeBoxes)
        {
            if (box.CurrentDraggable && box.CurrentDraggable.GetComponent<Item>().ItemType == contentType) return box;
        }

        return null;
    }

    private bool quit = false;
    private void OnApplicationQuit()
    {
        Save();
        quit = true;
    }

    public Dictionary<ItemType, int> AllItemsInBoxes()
    {
        Dictionary<ItemType, int> activeItemTypes = new();

        foreach (var box in activeBoxes)
        {
            if (!activeItemTypes.ContainsKey(box.ItemType)) activeItemTypes.Add(box.ItemType, box.ItemsCount);
            else activeItemTypes[box.ItemType] += box.ItemsCount;
        }
        
        return activeItemTypes;
    }
}

[Serializable]
public struct DeliveryData
{
    public Box boxPrefab;
}

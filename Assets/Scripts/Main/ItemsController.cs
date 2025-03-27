using System.Collections.Generic;
using Configs;
using TheSTAR.Data;
using TheSTAR.Utility;
using UnityEngine;
using Zenject;

public class ItemsController : MonoBehaviour
{
    [SerializeField] private ItemType testItem;
    [SerializeField] private Transform testWhere;

    private ConfigHelper<ItemsConfig> itemsConfig = new();
    private Dictionary<ItemType, List<Item>> activeItems = new();

    private DataController data;
    private DiContainer diContainer;

    [Inject]
    private void Construct(DataController data, DiContainer diContainer)
    {
        this.data = data;
        this.diContainer = diContainer;
        Init();
    }

    private void Init()
    {
        var allItemTypes = EnumUtility.GetValues<ItemType>();
        foreach (var itemType in allItemTypes)
        {
            activeItems.Add(itemType, new());
        }
    }

    //public Item CreateItem(ItemType itemType) => CreateItem(itemType, transform.position);
    public Item CreateItem(ItemType itemType, Vector3 pos) => CreateItem(itemType, pos, Quaternion.identity);
    public Item CreateItem(ItemType itemType, Vector3 pos, Quaternion rotation)
    {
        var prefab = itemsConfig.Get.Item(itemType).MainData.ItemPrefab;
        if (prefab == null)
        {
            Debug.LogError($"Префаб на предмет {itemType} не назначен!");
            return null;
        }

        Debug.Log("5");
        var item = diContainer.InstantiatePrefabForComponent<Item>(prefab, pos, rotation, transform);
        Debug.Log("6 " + prefab.ItemType);
        activeItems[itemType].Add(item);
        item.OnDestroyEvent += OnDestroyItem;
        return item;
    }

    private void OnDestroyItem(Item item)
    {
        if (activeItems[item.ItemType].Contains(item)) activeItems[item.ItemType].Remove(item);
    }

    private List<ItemType> GetActiveItemTypes()
    {
        List<ItemType> activeItemTypes = new();

        foreach (var group in activeItems)
        {
            if (group.Value.Count > 0) activeItemTypes.Add(group.Key);
        }

        return activeItemTypes;
    }

    public Dictionary<ItemType, int> GetAllActiveItemsAsDictionary()
    {
        Dictionary<ItemType, int> temp = new();

        foreach (var pair in activeItems)
        {
            if (pair.Value.Count > 0) temp.Add(pair.Key, pair.Value.Count);
        }

        return temp;
    }

    /*
    public Dictionary<ItemType, int> AllItemTypesInHandlers()
    {
        Dictionary<ItemType, int> activeItemTypes = new();

        foreach (var containerData in data.gameData.levelData.itemContainers)
        {
            foreach (var itemData in containerData.items)
            {
                if (itemData is EmbeddableItemInGameData embeddableItemInGameData)
                {
                    if (!activeItemTypes.ContainsKey(itemData.itemType)) activeItemTypes.Add(itemData.itemType, embeddableItemInGameData.count);
                    else activeItemTypes[itemData.itemType] += embeddableItemInGameData.count;
                }
                else
                {
                    if (!activeItemTypes.ContainsKey(itemData.itemType)) activeItemTypes.Add(itemData.itemType, 1);
                    else activeItemTypes[itemData.itemType]++;
                }
            }
        }

        return activeItemTypes;
    }
    */

    public Item FindItem(ItemType itemType)
    {
        foreach (var item in activeItems[itemType])
        {
            if (item.ItemType == itemType) return item;
        }

        return null;
    }

    public ItemData GetItemData(ItemType itemType)
    {
        return itemsConfig.Get.Item(itemType);
    }

    [ContextMenu("TestGenerateItem")]
    private void TestGenerateItem()
    {
        CreateItem(testItem, testWhere.position);
    }

    public Item Replace(Item from, ItemType to)
    {
        var pos = from.transform.position;
        var rotation = from.transform.rotation;
        var dragger = from.Draggable.CurrentDragger;
        if (dragger) dragger.EndDrag();

        Destroy(from.gameObject);

        var newItem = CreateItem(to, pos, rotation);
        if (dragger) dragger.StartDrag(newItem.Draggable);

        return newItem;
    }
}
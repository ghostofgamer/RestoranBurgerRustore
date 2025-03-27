using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

public class AssemblingBoard : MonoBehaviour, ICameraFocusable
{
    [SerializeField] private Dragger dragger;
    [SerializeField] private ItemType[] finalItems; // во что можно финализировать складываемые ингредиенты
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private Transform camPos;

    private ItemsController items;
    private GameWorldInteraction gameWorldInteraction;

    private const int MaxAssemblingDeepLimit = 10;

    private List<ItemType> currentAssembly = new();

    public event Action<List<ItemType>> OnChangeAssemblyEvent;
    public event Action OnFinalize;

    public int Deep => currentAssembly.Count;
    public Transform FocusTransform => camPos;

    [Inject]
    private void Construct(ItemsController items, GameWorldInteraction gameWorldInteraction)
    {
        this.items = items;
        this.gameWorldInteraction = gameWorldInteraction;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        touchInteractive.OnClickEvent += () =>
        {
            gameWorldInteraction.OnAssemblingBoardClick(this);
        };
    }

    public bool HavePlace()
    {
        GetLastEmptyDragger(out int deep, out _);
        return deep < MaxAssemblingDeepLimit;
    }

    public void Place(AssemblyItem burgerIngredient)
    {
        var lastDragger = GetLastEmptyDragger(out int deep, out var lastBurgerIngredient);
        if (deep >= MaxAssemblingDeepLimit) return;
        lastDragger.StartDrag(burgerIngredient.Item.Draggable);
        lastDragger.OnEndDragEvent += OnGetOutFromAssembly;
        if (lastBurgerIngredient) lastBurgerIngredient.Item.Draggable.ForceDeactivateCol();

        currentAssembly.Add(burgerIngredient.Item.ItemType);
        OnChangeAssemblyEvent?.Invoke(currentAssembly);

        burgerIngredient.OnAddToAssembly();

        TryFinalize();
    }

    public Item GetLastItem()
    {
        if (dragger.IsEmpty) return null;
        else
        {
            var item = dragger.CurrentDraggable.GetComponent<Item>();
            if (!item) return null;

            var assemblingItem = item.GetComponent<AssemblyItem>();
            if (assemblingItem) return assemblingItem.GetLastItem();
            else return item;
        }
    }

    private Dragger GetLastEmptyDragger(out int deep, out AssemblyItem lastBurgerIngredient)
    {
        if (dragger.IsEmpty)
        {
            deep = 0;
            lastBurgerIngredient = null;
            return dragger;
        }
        else return dragger.CurrentDraggable.GetComponent<AssemblyItem>().GetLastEmptyDragger(1, out deep, out lastBurgerIngredient);
    }

    private void OnGetOutFromAssembly(Dragger dragger, Draggable draggable)
    {
        currentAssembly.RemoveAt(currentAssembly.Count - 1);
        OnChangeAssemblyEvent?.Invoke(currentAssembly);
        dragger.OnEndDragEvent -= OnGetOutFromAssembly;

        GetLastEmptyDragger(out int deep, out var lastBurgerIngredient);
        if (lastBurgerIngredient)
        {
            lastBurgerIngredient.Item.Draggable.ForceActivateCol();
        }

        draggable.GetComponent<AssemblyItem>().OnTakeOutFromAssembly();
    }

    /// <summary>
    /// Пытаемся преобразовать выложенные предметы в финальный
    /// </summary>
    private void TryFinalize()
    {
        foreach (var finalItem in finalItems)
        {
            var recipe = items.GetItemData(finalItem).Recipe;
            
            if (CheckAssemblyForRecipe(recipe.RecipeItems))
            {
                Debug.Log("Finalize to " + finalItem);
                var rootDraggable = dragger.CurrentDraggable;
                dragger.EndDrag();
                Destroy(rootDraggable.gameObject);
                var item = items.CreateItem(finalItem, dragger.transform.position);
                dragger.StartDrag(item.Draggable);

                currentAssembly.Clear();
                OnChangeAssemblyEvent?.Invoke(currentAssembly);
                OnFinalize?.Invoke();
                return;
            }
        }
    }

    /// <summary>
    /// Соответствует ли сборка рецепту
    /// </summary>
    private bool CheckAssemblyForRecipe(ItemType[] recipeItems)
    {
        if (currentAssembly.Count != recipeItems.Length) return false;

        for (int i = 0; i < recipeItems.Length; i++)
        {
            if (recipeItems[i] != currentAssembly[i]) return false;
        }

        return true;
    }

    /// <summary>
    /// Возвращает финальный айтем (цельный бургер) если такой есть
    /// </summary>
    public Item TryGetFinalItem()
    {
        if (dragger.CurrentDraggable == null) return null;
        if (!dragger.CurrentDraggable.TryGetComponent<Item>(out var item)) return null;

        var section = items.GetItemData(item.ItemType).mainData.SectionType;
        if (section == ItemSectionType.FinalBurger) return item;

        return null;
    }
}
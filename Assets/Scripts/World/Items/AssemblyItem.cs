using UnityEngine;

/// <summary>
/// AssemblyItem can be used as an element on AssemblyBoard
/// </summary>
[RequireComponent(typeof(Item))]
public class AssemblyItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Dragger dragger;

    private bool inAssembly = false;

    public Item Item => item;
    public bool InAssembly => inAssembly;

    public void Place(AssemblyItem burgerIngredient)
    {
        dragger.StartDrag(burgerIngredient.item.Draggable);
    }

    public Dragger GetLastEmptyDragger(int startDeep, out int resultDeep, out AssemblyItem lastBurgerIngredient)
    {
        if (dragger.IsEmpty)
        {
            resultDeep = startDeep;
            lastBurgerIngredient = this;
            return dragger;
        }
        else
        {
            return dragger.CurrentDraggable.GetComponent<AssemblyItem>().GetLastEmptyDragger(startDeep + 1, out resultDeep, out lastBurgerIngredient);
        }
    }

    public Item GetLastItem()
    {
        if (dragger.IsEmpty) return item;
        else return dragger.CurrentDraggable.GetComponent<AssemblyItem>().GetLastItem();
    }

    public void OnAddToAssembly()
    {
        inAssembly = true;
    }

    public void OnTakeOutFromAssembly()
    {
        inAssembly = false;
    }
}
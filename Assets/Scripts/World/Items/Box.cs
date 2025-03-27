using UnityEngine;
using System;

public class Box : MonoBehaviour
{
    [SerializeField] protected DraggerSplitter splitter;
    [SerializeField] protected TrashDestroyable trashDestroyable;
    [SerializeField] protected TutorInWorldFocus tutorFocus;
    [SerializeField] protected Transform playerDragPos;

    public event Action<Box> OnDestroyEvent;
    public event Action OnEndDragEvent;

    public TutorInWorldFocus TutorFocus => tutorFocus;

    public bool IsEmpty => CurrentDraggable == null;
    public Draggable CurrentDraggable => splitter.CurrentDraggable;
    public int ItemsCount // => splitter.AllDraggables.Count; // todo учитывать вложенные
    {
        get
        {
            int result = 0;
            var splitterDraggables = splitter.AllDraggables; // корни

            foreach (var draggableRoot in splitterDraggables)
            {
                var item = draggableRoot.GetComponent<Item>();
                if (item == null) continue;

                var embeddableItem = item.GetComponent<EmbeddableItem>();
                if (embeddableItem != null) result += embeddableItem.GetEmbeddableItemsInStackCount;
                else result++;
            }

            return result;
        }
    }

    private ItemType itemType;
    public ItemType ItemType => itemType;

    public Transform PlayerDragPos => playerDragPos;

    //public virtual bool IsOpen => true;

    public void Init(ItemType itemType)
    {
        this.itemType = itemType;

        splitter.Init();
        splitter.OnEndDragEvent += (element) => OnEndDragEvent?.Invoke();
        trashDestroyable.BeforeTrashDestroyEvent += () =>
        {
            //analytics.Trigger(RepeatingEventType.DropBoxToTrash);
        };

        trashDestroyable.SetDestroyCondition(() => IsEmpty);
    }

    public void SetToBox(Item product)
    {
        if (!splitter.HavePlace(product.ItemType, out var dragger))
        {
            return;
        }
        //if (product.Draggable.CurrentDragger) product.Draggable.CurrentDragger.EndDrag();
        dragger.StartDrag(product.Draggable);
    }

    public Draggable FindFirstItem(out Dragger dragger)
    {
        Draggable product = CurrentDraggable;
        dragger = product.CurrentDragger;
        return product;
    }

    private void OnDestroy()
    {
        this.OnDestroyEvent?.Invoke(this);
    }
}
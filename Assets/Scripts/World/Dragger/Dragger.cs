using System;
using UnityEngine;
using DG.Tweening;

public class Dragger : MonoBehaviour
{
    [SerializeField] private bool autoChangeInteractableForDraggable = false;
    [SerializeField] private Draggable currentDraggable;

    public event Action<Dragger, Draggable> OnStartDragEvent;
    public event Action<Dragger, Draggable> OnEndDragEvent;
    public event Action OnChangeStackEvent;

    private DraggerGroup group;
    public DraggerGroup Group => group;

    private const float ThrowForce = 6;

    private Tweener moveDraggableTweener;
    private Tweener rotateDraggableTweener;

    public Draggable CurrentDraggable => currentDraggable;
    public bool IsEmpty
    {
        get
        {
            if (CurrentDraggable == null) return true;
            var draggableItem = currentDraggable.GetComponent<Item>();
            if (draggableItem == null) return false;

            return false;
        }
    }

    private const float MoveDuration = 0.25f;
    private readonly Ease MoveEase = Ease.OutCubic;

    public void SetCurrentItem(Draggable draggable)
    {
        currentDraggable = draggable;
    }

    
    public virtual void StartDrag(Draggable draggable, bool useAnim = true)
    {
        if (draggable.CurrentDragger) draggable.CurrentDragger.EndDrag();

        draggable.transform.parent = transform;

        if (useAnim)
        {
            moveDraggableTweener =
            draggable.transform.DOLocalMove(Vector3.zero, MoveDuration).SetEase(MoveEase);

            rotateDraggableTweener =
            draggable.transform.DOLocalRotate(Vector3.zero, MoveDuration).SetEase(MoveEase);
        }
        else
        {
            draggable.transform.localEulerAngles = Vector3.zero;
            draggable.transform.localPosition = Vector3.zero;
        }

        currentDraggable = draggable;

        draggable.OnStartDrag(this, autoChangeInteractableForDraggable);

        OnStartDrag(draggable);
    }

    public void OnStartDrag(Draggable draggable)
    {
        OnStartDragEvent?.Invoke(this, draggable);
    }

    public void EndDrag()
    {
        DoEndDrag(Vector3.zero);
    }

    public void EndDrag(Vector3 impulseDirection)
    {
        DoEndDrag(impulseDirection);
    }

    protected virtual void DoEndDrag(Vector3 impulseDirection)
    {
        if (currentDraggable == null) return;

        moveDraggableTweener?.Kill();
        rotateDraggableTweener?.Kill();
        moveDraggableTweener = null;
        rotateDraggableTweener = null;

        currentDraggable.transform.parent = null;
        currentDraggable.OnEndDrag(autoChangeInteractableForDraggable);
        if (impulseDirection != Vector3.zero) currentDraggable.Throw(impulseDirection * ThrowForce);

        var draggableToCallback = currentDraggable;
        
        currentDraggable = null;

        OnEndDrag(draggableToCallback);
    }

    public void OnEndDrag(Draggable draggable)
    {
        OnEndDragEvent?.Invoke(this, draggable);
    }

    public void SetGroup(DraggerGroup group)
    {
        this.group = group;
    }

    public void OnChangeStack()
    {
        OnChangeStackEvent?.Invoke();
    }
}
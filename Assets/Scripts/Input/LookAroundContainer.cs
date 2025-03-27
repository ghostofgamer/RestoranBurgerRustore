using System;
using TheSTAR.GUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class LookAroundContainer : GuiUniversalElement
{
    [SerializeField] private Pointer pointer;

    public event Action StartLookAroundEvent;
    public event Action<Vector2> LookAroundEvent;
    public event Action<PointerEventData> ClickEvent;
    private Vector2 startTouchPos; // for offset calculation
    private Vector2 currentTouchPos; // for offset calculation

    private const float MaxDistanceForClick = 10;

    private bool clickOnlyMove;

    public override void Init()
    {
        pointer.InitPointer(
            OnDown, 
            OnDrag,
            OnUp);
    }

    private void OnDown(PointerEventData pointerEventData)
    {
        startTouchPos = pointerEventData.position;
        currentTouchPos = pointerEventData.position;
        StartLookAroundEvent?.Invoke();
    }

    private void OnDrag(PointerEventData pointerEventData)
    {
        var offset = pointerEventData.position - currentTouchPos;
        if (!clickOnlyMove) LookAroundEvent.Invoke(offset);
        currentTouchPos = pointerEventData.position;
    }

    private void OnUp(PointerEventData pointerEventData)
    {
        var offset = pointerEventData.position - currentTouchPos;
        if (!clickOnlyMove) LookAroundEvent?.Invoke(offset);
        currentTouchPos = pointerEventData.position;

        var distance = Vector2.Distance(startTouchPos, currentTouchPos);
        if (distance < MaxDistanceForClick) ClickEvent?.Invoke(pointerEventData);
    }

    public void SetClickOnlyMove(bool clickOnlyMove)
    {
        this.clickOnlyMove = clickOnlyMove;
    }
}
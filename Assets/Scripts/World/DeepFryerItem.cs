using UnityEngine;
using System;
using DG.Tweening;

public class DeepFryerItem : MonoBehaviour
{
    [SerializeField] private bool haveToItem;
    [SerializeField] private ItemType toItem;
    [SerializeField] private Item item;

    private float griddlingValue = 0;

    private const float GriddlingDuration = 10;

    public Item Item => item;
    public ItemType To => toItem;

    private Action<DeepFryerItem> completeGriddlingProcessAction;

    private Tweener griddlingTweener;

    public void StartGriddlingProcess(Action<DeepFryerItem> completeGriddlingProcessAction)
    {
        this.completeGriddlingProcessAction = completeGriddlingProcessAction;
        
        // айтем не преобразуется ни во что
        if (!haveToItem) return;

        griddlingTweener = 
        DOVirtual.Float(griddlingValue, GriddlingDuration, GriddlingDuration - griddlingValue, (temp) =>
        {
            griddlingValue = temp;
            Debug.Log(temp);
            // todo обновлять прогресс бар
        }).OnComplete(() => this.completeGriddlingProcessAction?.Invoke(this)).SetEase(Ease.Linear);
    }

    public void BreakGriddlingProcess()
    {
        griddlingTweener.Kill();
    }   
}
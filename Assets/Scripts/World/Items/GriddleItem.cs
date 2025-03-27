using System;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// GriddleItem can be used on Griddle for cooking
/// </summary>
public class GriddleItem : MonoBehaviour
{
    [SerializeField] private bool haveToItem;
    [SerializeField] private ItemType toItem;
    [SerializeField] private Item item;
    [SerializeField] private CookingProcessUI cookingProcessUI;

    private float griddlingValue = 0;

    private const float GriddlingDuration = 10;

    public Item Item => item;
    public ItemType To => toItem;

    private Action<GriddleItem> completeGriddlingProcessAction;

    private Tweener griddlingTweener;

    public void StartGriddlingProcess(Action<GriddleItem> completeGriddlingProcessAction)
    {
        this.completeGriddlingProcessAction = completeGriddlingProcessAction;
        
        cookingProcessUI.SetTitle(GetCookingProcessTitle());
        cookingProcessUI.gameObject.SetActive(true);

        // айтем не преобразуется ни во что
        if (!haveToItem)
        {
            cookingProcessUI.SetUseProgressbar(false);
            return;
        }

        cookingProcessUI.SetUseProgressbar(true);

        griddlingTweener = 
        DOVirtual.Float(griddlingValue, GriddlingDuration, GriddlingDuration - griddlingValue, (temp) =>
        {
            griddlingValue = temp;
            
            var progress = (float)griddlingValue / (float) GriddlingDuration;
            cookingProcessUI.SetProgress(progress);

        }).OnComplete(() => this.completeGriddlingProcessAction?.Invoke(this)).SetEase(Ease.Linear);
    }

    public void BreakGriddlingProcess()
    {
        griddlingTweener.Kill();
        cookingProcessUI.gameObject.SetActive(false);
    }

    private string GetCookingProcessTitle()
    {
        switch (item.ItemType)
        {
            case ItemType.CutletRaw:
                return "PATTY, <color=#fdbe2c>RAW</color>";

            case ItemType.CutletWell:
                return "PATTY, <color=#61fd2c>WELL DONE</color>";

            case ItemType.CutletBurnt:
                return "PATTY, <color=#fc1f2a>BURNED</color>"; 

            default:
                return "Cooking";
        }
    }
}
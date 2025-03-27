using System;
using TheSTAR.Sound;
using UnityEngine;
using Zenject;

public class Griddle : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private DraggerGroup group;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    public TutorInWorldFocus TutorFocus => tutorFocus;

    private GameWorldInteraction worldInteraction;
    private ItemsController items;
    private SoundController sounds;

    public event Action OnCompleteGriddlingEvent;

    public event Action<Griddle> OnStartGriddlingEvent;
    public event Action<Griddle> OnFinishGriddlingEvent;

    [Inject]
    private void Construct(GameWorldInteraction worldInteraction, ItemsController items, SoundController sounds)
    {
        this.worldInteraction = worldInteraction;
        this.items = items;
        this.sounds = sounds;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        group.Init();
        
        touchInteractive.OnClickEvent += () =>
        {
            worldInteraction.OnGriddleClick(this);
        };

        group.OnSetItemEvent += (group, dragger, draggable) =>
        {
            var griddleItem = draggable.GetComponent<GriddleItem>();
            griddleItem.StartGriddlingProcess(OnCompleteGriddling);

            if (group.AllDraggables.Count == 1) OnStartGriddlingEvent?.Invoke(this);
        };

        group.OnEndDragItemEvent += (group, dragger, draggable) =>
        {
            var griddleItem = draggable.GetComponent<GriddleItem>();
            griddleItem.BreakGriddlingProcess();
        };

        group.OnEmptyGroup += (droup) =>
        {
            OnFinishGriddlingEvent?.Invoke(this);
        };
    }

    public void TryPlace(GriddleItem griddleItem, out bool success)
    {
        success = false;

        if (!group.HavePlace(griddleItem.Item.ItemType, out var place)) return;

        griddleItem.Item.Draggable.CurrentDragger.EndDrag();
        place.StartDrag(griddleItem.Item.Draggable);

        success = true;
    }

    private void OnCompleteGriddling(GriddleItem griddleItem)
    {
        var dragger = griddleItem.Item.Draggable.CurrentDragger;
        dragger.EndDrag();
        var toItem = griddleItem.To;

        Destroy(griddleItem.gameObject);

        var newItem = items.CreateItem(toItem, dragger.transform.position, dragger.transform.rotation);
        dragger.StartDrag(newItem.Draggable);

        if (newItem.ItemType == ItemType.CutletWell) sounds.Play(SoundType.timer_bell_m1tycbno);

        OnCompleteGriddlingEvent?.Invoke();
    }
}
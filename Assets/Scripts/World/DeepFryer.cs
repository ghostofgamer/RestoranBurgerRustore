using TheSTAR.Sound;
using UnityEngine;
using Zenject;

public class DeepFryer : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private DraggerGroup group;

    private GameWorldInteraction worldInteraction;
    private ItemsController items;
    private SoundController sounds;

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
            worldInteraction.OnDeepFryerClick(this);
        };

        group.OnSetItemEvent += (group, dragger, draggable) =>
        {
            var deepFryerItem = draggable.GetComponent<DeepFryerItem>();
            deepFryerItem.StartGriddlingProcess(OnCompleteGriddling);

            if (group.AllDraggables.Count == 1) sounds.Play(SoundType.friturnitsa);
        };

        group.OnEndDragItemEvent += (group, dragger, draggable) =>
        {
            var deepFryerItem = draggable.GetComponent<DeepFryerItem>();
            deepFryerItem.BreakGriddlingProcess();

            if (group.AllDraggables.Count == 0) sounds.Stop(SoundType.friturnitsa);
        };
    }

    public void TryPlace(DeepFryerItem deepFryerItem)
    {
        if (!group.HavePlace(deepFryerItem.Item.ItemType, out var place)) return;

        deepFryerItem.Item.Draggable.CurrentDragger.EndDrag();
        place.StartDrag( deepFryerItem.Item.Draggable);
    }

    private void OnCompleteGriddling(DeepFryerItem deepFryerItem)
    {
        var dragger = deepFryerItem.Item.Draggable.CurrentDragger;
        dragger.EndDrag();
        var toItem = deepFryerItem.To;

        Destroy(deepFryerItem.gameObject);

        var newItem = items.CreateItem(toItem, dragger.transform.position);
        dragger.StartDrag(newItem.Draggable);
    }
}
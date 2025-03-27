using UnityEngine;

public class OrderPlateDragger : Dragger
{
    [SerializeField] private GameObject miniPlateForCakes;

    public override void StartDrag(Draggable draggable, bool skipAnim)
    {
        base.StartDrag(draggable, skipAnim);

        var item = draggable.GetComponent<Item>();
        if (!item) return;

        miniPlateForCakes.SetActive(true);
    }

    protected override void DoEndDrag(Vector3 impulseDirection)
    {
        base.DoEndDrag(impulseDirection);

        miniPlateForCakes.SetActive(false);        
    }
}
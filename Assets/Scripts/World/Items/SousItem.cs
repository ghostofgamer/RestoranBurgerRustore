using UnityEngine;

public class SousItem : MonoBehaviour
{
    [SerializeField] private Draggable draggable;
    [SerializeField] private TouchInteractive touch;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        touch.OnClickEvent += () =>
        {
            if (draggable.CurrentDragger) draggable.CurrentDragger.EndDrag();
            Destroy(gameObject);
        };
    }
}
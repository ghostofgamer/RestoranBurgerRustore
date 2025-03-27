using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] protected Draggable draggable;
    [SerializeField] private ItemType itemType;
    [SerializeField] private TutorInWorldFocus tutorFocus;
    [SerializeField] private Transform playerDragPos;
    [SerializeField] private TouchInteractive touchInteractive;

    public Draggable Draggable => draggable;
    public ItemType ItemType => itemType;
    public TutorInWorldFocus TutorFocus => tutorFocus;
    public Transform PlayerDragPos => playerDragPos;
    public TouchInteractive TouchInteractive => touchInteractive;

    public event Action<Item> OnDestroyEvent;

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }
}
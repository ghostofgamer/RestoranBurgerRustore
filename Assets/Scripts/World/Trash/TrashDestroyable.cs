using System;
using UnityEngine;

/// <summary>
/// Объект уничтожается при соприкосновении с мусоркой
/// </summary>
public class TrashDestroyable : MonoBehaviour
{
    public event Action BeforeTrashDestroyEvent;
    public delegate bool DestroyCondition();

    private DestroyCondition destroyCondition;

    private void Awake()
    {
        destroyCondition ??= () => true;
    }

    public void OnEnterTrash(Trash trash)
    {
        if (!destroyCondition()) return;
        BeforeTrashDestroyEvent?.Invoke();
        Destroy(gameObject);
    }

    public void SetDestroyCondition(DestroyCondition destroyCondition)
    {
        this.destroyCondition = destroyCondition;
    }
}
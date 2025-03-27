using System;
using UnityEngine;

public class TouchInteractive : MonoBehaviour
{
    [SerializeField] private Outline[] outlines;
    [SerializeField] private bool forceFocus;
    [SerializeField] private Collider col;

    private bool interactable = true;
    private bool focused = false;
    
    public event Action OnClickEvent;

    private void Start()
    {
        UpdateVisual();
    }

    public virtual void OnClick()
    {
        if (!interactable || (!focused && !forceFocus)) return;
        OnClickEvent?.Invoke();
    }

    public virtual void OnClickForce()
    {
        OnClickEvent?.Invoke();
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (var outline in outlines)
            outline.enabled = interactable && (focused || forceFocus);
    }

    public void StartFocus()
    {
        focused = true;
        UpdateVisual();
    }

    public void EndFocus()
    {
        focused = false;
        UpdateVisual();
    }

    public void Activate()
    {
        col.enabled = true;
    }

    public void Deactivate()
    {
        col.enabled = false;
    }

    public void SetForceFocus(bool forceFocus)
    {
        this.forceFocus = forceFocus;
        UpdateVisual();
    }
}
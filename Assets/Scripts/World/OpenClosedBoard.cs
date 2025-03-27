using System;
using UnityEngine;
using Zenject;

public class OpenClosedBoard : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private GameObject[] closedObjects;
    [SerializeField] private GameObject[] openObjects;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    private TutorialController tutorial;
    private bool open;
    public bool Open => open;
    public TutorInWorldFocus TutorFocus => tutorFocus;

    public event Action<bool> OnChangeStatusEvent;

    [Inject]
    private void Construct(TutorialController tutorial)
    {
        this.tutorial = tutorial;
    }

    private void Start()
    {
        Init();
        UpdateVisual();
    }

    public void Reset()
    {
        open = false;
        UpdateVisual();
        OnChangeStatusEvent?.Invoke(open);
    }
    
    private void Init()
    {
        touchInteractive.OnClickEvent += ChangeStatus;
    }

    private void UpdateVisual()
    {
        foreach (var element in closedObjects) element.SetActive(!open);
        foreach (var element in openObjects) element.SetActive(open);
    }

    private void ChangeStatus()
    {
        if (!tutorial.IsCompleted(TutorialType.SetFastFoodName))
            return;
        
        open = !open;
        UpdateVisual();

        OnChangeStatusEvent?.Invoke(open);
    }
}
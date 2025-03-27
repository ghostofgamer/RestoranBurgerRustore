using System;
using UnityEngine;
using Zenject;

public class Garbage : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    private GameWorldInteraction worldInteraction;

    public TutorInWorldFocus TutorFocus => tutorFocus;

    public event Action<Garbage> OnCleanEvent;

    [Inject]
    private void Construct(GameWorldInteraction worldInteraction)
    {
        this.worldInteraction = worldInteraction;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        touchInteractive.OnClickEvent += () =>
        {
            worldInteraction.OnGarbageClick(this);
        };
    }

    public void Clean()
    {
        OnCleanEvent?.Invoke(this);
        Destroy(gameObject);
    }
}
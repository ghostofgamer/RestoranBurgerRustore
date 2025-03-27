using System;
using UnityEngine;
using Zenject;

[Obsolete]
public class PhoneBooth : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private TutorInWorldFocus tutorInWorldFocus;

    private GameWorldInteraction worldInteraction;

    public TutorInWorldFocus TutorInWorldFocus => tutorInWorldFocus;

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
            // worldInteraction.OnPhoneBoothClick(this);
        };
    }
}
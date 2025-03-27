using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheSTAR.GUI;
using Zenject;

public class Computer : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    public TutorInWorldFocus TutorFocus => tutorFocus;

    private GameWorldInteraction worldInteraction;

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
            worldInteraction.OnClickComputer(this);
        };
    }
}
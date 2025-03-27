using UnityEngine;
using Zenject;

public class SodaMachine : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private TutorInWorldFocus tutorFocus;
    [SerializeField] private SodaFiller[] fillers;

    private GameWorldInteraction gameWorldInteraction;

    public Filler GetFiller(ItemType from)
    {
        foreach (var pair in fillers)
        {
            if (pair.Filler.CanFill(from)) return pair.Filler;
        }

        return null;
    }

    public TutorInWorldFocus TutorFocus => tutorFocus;

    [Inject]
    private void Consruct(GameWorldInteraction gameWorldInteraction)
    {
        this.gameWorldInteraction = gameWorldInteraction;
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {                
        touchInteractive.OnClickEvent += () =>
        {
            gameWorldInteraction.OnClickSodaMachine(this);
        };
    }
}
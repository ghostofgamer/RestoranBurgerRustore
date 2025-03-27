using UnityEngine;
using Zenject;

public class CoffeeMachine : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private Filler filler;
    [SerializeField] private Transform coffeeFillTran;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    private GameWorldInteraction gameWorldInteraction;

    public Filler Filler => filler;
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
            gameWorldInteraction.OnClickCoffeeMachine(this);
        };

        filler.OnChangeFillPercentEvent += (percent) =>
        {
            coffeeFillTran.localScale = new Vector3(1, percent, 1);  
        };
    }
}
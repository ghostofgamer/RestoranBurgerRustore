using UnityEngine;
using Zenject;

public class Trash : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    public TutorInWorldFocus TutorFocus => tutorFocus;

    private GameWorldInteraction gameWorldInteraction;
    private GameController _gameController;
    private TutorialController _tutorialController;
    
    [Inject]
    private void Construct(GameWorldInteraction gameWorldInteraction, GameController gameController,
        TutorialController tutorialController)
    {
        this.gameWorldInteraction = gameWorldInteraction;
        _gameController = gameController;
        _tutorialController = tutorialController;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        // touchInteractive.OnClickEvent += () => { gameWorldInteraction.OnTrashClick(this); };
        touchInteractive.OnClickEvent += () =>
        {
            if (!_tutorialController.IsCompleted(TutorialType.ClearTrash))
            {
                _tutorialController.CompleteTutorial(TutorialType.ClearTrash);
                _gameController.TriggerTutorial();
            }
               

            gameWorldInteraction.OnTrashClick(this);
        };
    }

    private void OnCollisionEnter(Collision other)
    {
        var trashDestroyable = other.gameObject.GetComponent<TrashDestroyable>();
        if (!trashDestroyable) return;

        trashDestroyable.OnEnterTrash(this);
        
        if (!_tutorialController.IsCompleted(TutorialType.ClearTrash))
        {
            _tutorialController.CompleteTutorial(TutorialType.ClearTrash);
            _gameController.TriggerTutorial();
        }
    }
}
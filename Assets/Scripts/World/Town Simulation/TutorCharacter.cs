using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorCharacter : MonoBehaviour, ICameraFocusable
{
    [SerializeField] private TutorInWorldFocus tutorFocus;
    public TutorInWorldFocus TutorFocus => tutorFocus;
    [SerializeField] private Transform camPos;
    public Transform FocusTransform => camPos;
    /*
    [SerializeField] private Passer passer;
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private Transform tutorPos;

    public Transform TutorPos => tutorPos;
    

    private GameController game;
    private LevelController level;

    [Inject]
    private void Construct(GameController game, LevelController level)
    {
        this.game = game;
        this.level = level;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        touchInteractive.OnClickEvent += () =>
        {
            level.OnClickTutorialCharacter(this);
        };
    }

    public void OnCompleteDialog()
    {
        //gameObject.layer = 13;
        passer.InitAsMoveTo(game.World.EndPointForTutorCharacter);
        Invoke(nameof(AutoDestory), 10);
    }

    private void AutoDestory()
    {
        Destroy(gameObject);
    }
    */
}
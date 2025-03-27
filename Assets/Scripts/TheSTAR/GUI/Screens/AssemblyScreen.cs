using TheSTAR.GUI;
using UnityEngine;
using Zenject;

public class AssemblyScreen : GuiScreen
{
    [SerializeField] private PointerButton closeButton;
    [SerializeField] private AssemblyHintContainer assemblyHint;

    private GameController game;
    private GuiController gui;
    private CameraController cameras;
    private GameWorldInteraction worldInteraction;
    private TutorialController tutorial;

    [Inject]
    private void Construct(GameController game, GuiController gui, CameraController cameras, TutorialController tutorial)
    {
        this.game = game;
        this.gui = gui;
        this.cameras = cameras;
        this.tutorial = tutorial;
    }

    public override void Init()
    {
        base.Init();

        closeButton.Init(OnCloseClick);
        assemblyHint.Init();
    }

    protected override void OnShow()
    {
        base.OnShow();
        cameras.DeactivateRayVision();
        game.OnStartAssemblingFocus();
        gui.FindUniversalElement<LookAroundContainer>().SetClickOnlyMove(true);
        
        TriggerTutorial();
    }

    protected override void OnHide()
    {
        base.OnHide();

        if (tutorial.InTutorial) tutorial.BreakTutorial();
    }

    private void OnCloseClick()
    {
        cameras.TempFocus(null, true);
        cameras.ActivateRayVision();
        game.OnEndAssemblingFocus();
        gui.ShowMainScreen();
        gui.FindUniversalElement<LookAroundContainer>().SetClickOnlyMove(false);
    }

    private void TriggerTutorial()
    {
        if (tutorial.IsCompleted(TutorialType.TakeCutlet) && !tutorial.IsCompleted(TutorialType.AssemblyBurger))
        {
            tutorial.TryShowInWorld(TutorialType.AssemblyBurger, new TutorInWorldFocus[] {}, out _);
            return;
        }
    }
}
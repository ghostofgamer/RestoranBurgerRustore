using UnityEngine;
using Zenject;
using TheSTAR.GUI;

/// <summary>
/// Биндим в контексте сцены Game
/// </summary>
public class GameSceneInstaller : MonoInstaller
{
    [Header("Main")]
    [SerializeField] private GameController gameControllerPrefab;
    [SerializeField] private CameraController cameraControllerPrefab;
    [SerializeField] private ItemsController itemsControllerPrefab;
    [SerializeField] private Delivery deliveryPrefab;

    [Space]
    [SerializeField] private TutorialController tutorialControllerPrefab;

    private GuiController gui;
    [SerializeField] private GuiConfig guiConfig;

    public override void InstallBindings()
    {
        InstallGuiContainers();

        var camera = Container.InstantiatePrefabForComponent<CameraController>(cameraControllerPrefab);
        Container.Bind<CameraController>().FromInstance(camera).AsSingle();

        var itemsController = Container.InstantiatePrefabForComponent<ItemsController>(itemsControllerPrefab);
        Container.Bind<ItemsController>().FromInstance(itemsController).AsSingle();

        var delivery = Container.InstantiatePrefabForComponent<Delivery>(deliveryPrefab);
        Container.Bind<Delivery>().FromInstance(delivery).AsSingle();

        var gameController = Container.InstantiatePrefabForComponent<GameController>(gameControllerPrefab);
        Container.Bind<GameController>().FromInstance(gameController).AsSingle();
        
        // gui.InitGameController(gameController);
        // gui
        InstallGuiScreens();
    }

    private void InstallGuiContainers()
    {
        gui = Container.InstantiatePrefabForComponent<GuiController>(guiConfig.GuiControllerPrefab);
        Container.Bind<GuiController>().FromInstance(gui).AsSingle();
        Debug.Log("создание GUI Containres");
        var tutor = Container.InstantiatePrefabForComponent<TutorialController>(tutorialControllerPrefab);
        Container.Bind<TutorialController>().FromInstance(tutor).AsSingle();
    }

    private void InstallGuiScreens()
    {
        Debug.Log("создание GUI 1");
        GuiScreen[] createdScreens = new GuiScreen[guiConfig.ScreenPrefabs.Length];
        GuiScreen screen;
        for (int i = 0; i < guiConfig.ScreenPrefabs.Length; i++)
        {
            screen = Container.InstantiatePrefabForComponent<GuiScreen>(guiConfig.ScreenPrefabs[i], gui.ScreensContainer.position, Quaternion.identity, gui.ScreensContainer);
            createdScreens[i] = screen;
            
            if (screen.TryGetComponent(out DayOverScreen dayOverScreen))
            {
                Debug.Log("создание GUI DAY OVER SCREEN");
                gui.InitDayOverContainer(dayOverScreen);
            }
        }

        GuiUniversalElement[] createdUniversalElements = new GuiUniversalElement[guiConfig.UniversalElementPrefabs.Length];
        GuiUniversalElement ue;
        for (int i = 0; i < guiConfig.UniversalElementPrefabs.Length; i++)
        {
            var uePrefab = guiConfig.UniversalElementPrefabs[i];
            var placement = gui.UniversalElementsContainer(uePrefab.Placement);
            ue = Container.InstantiatePrefabForComponent<GuiUniversalElement>(uePrefab, placement.position, Quaternion.identity, placement);
            createdUniversalElements[i] = ue;
        }

        gui.Set(createdScreens, createdUniversalElements);

        foreach (var screenGui in createdUniversalElements)
        {
            if(screenGui.TryGetComponent<TopUiContainer>(out TopUiContainer topUiContainer))
                gui.InitTopUIContainer(topUiContainer);
        }
    }
}
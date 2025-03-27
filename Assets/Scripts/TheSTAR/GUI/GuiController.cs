using System;
using ReputationContent;
using UnityEngine;
using TheSTAR.Utility;
using Zenject;

namespace TheSTAR.GUI
{
    public sealed class GuiController : MonoBehaviour
    {
        #region Inspector

        private GuiScreen[] screens = new GuiScreen[0];
        private GuiUniversalElement[] universalElements = new GuiUniversalElement[0];
        private GuiScreen mainScreen;
        private DiContainer diContainer;
        
        [SerializeField] private UnityDictionary<UniversalElementPlacement, Transform> ueContainers;
        [SerializeField] private Transform screensContainer;
        [SerializeField] private bool deactivateOtherScreensByStart = true;
        [SerializeField] private bool showMainScreenByStart = true;
        
        [SerializeField]private Reputation _reputation;

        #endregion // Inspector

        private GuiScreen _currentScreen;
        public GuiScreen CurrentScreen => _currentScreen;
        
        public Reputation Reputation => _reputation;

        public Transform ScreensContainer => screensContainer;
        public Transform UniversalElementsContainer(UniversalElementPlacement placement) => ueContainers.Get(placement);

        private readonly Type mainScreenType = typeof(GameScreen);
        
        private GameController _gameController;
        private GameWorld _gameWorld;
        private TopUiContainer _topUiContainer;
        

        public DayOverScreen DayOverScreen { get; private set; }
        
        public TopUiContainer TopUiContainer => _topUiContainer;
        
        public GameWorld GameWorld => _gameWorld;

        [Inject]
        private void Construct(
            DiContainer diContainer
            )
        {
            this.diContainer = diContainer;
        }

        public void InitGameWorld(GameWorld gameworld)
        {
            _gameWorld = gameworld;
            _topUiContainer.InitReputation(gameworld.FastFood);
        }

        public void InitGameWorldInteraction(GameWorldInteraction gameWorldInteraction)
        {
            Debug.Log("InitGameWorldInteraction!!!!!!!!!!!!");
            _topUiContainer.InitGameWorldInteractive(gameWorldInteraction);
        }
        
        public void InitTopUIContainer(TopUiContainer game)
        {
            _topUiContainer = game;
            diContainer.Bind<TopUiContainer>().FromInstance(_topUiContainer).AsSingle();
        }
        
        public void InitDayOverContainer(DayOverScreen dayOverScreen)
        {
            // DayOverScreen = dayOverScreen;
            diContainer.Bind<DayOverScreen>().FromInstance(dayOverScreen).AsSingle();
        }

        public void ShowMainScreen()
        {
            Show(mainScreen, true);
        }

        public void Show<TScreen>(bool closeCurrentScreen = true, Action endAction = null) where TScreen : GuiScreen
        {
            GuiScreen screen = FindScreen<TScreen>();
            Debug.Log("SCREEN ");
            Show(screen, closeCurrentScreen, endAction);
        }
        
        public void ShowForeachScreen<TScreen>(bool closeCurrentScreen = true, Action endAction = null) where TScreen : GuiScreen
        {
            GuiScreen screen = null;
            
            foreach (var t in screens)
            {
                if (t is TScreen)
                    screen = t;
            }

            if (screen == null)
                return;
            
            Show(screen, closeCurrentScreen, endAction);
        }
        
        public GameObject GetNeedScreen<TScreen>() where TScreen : GuiScreen
        {
            GuiScreen screen = null;
            
            foreach (var t in screens)
            {
                if (t is TScreen)
                    screen = t;
            }
            
            return screen.gameObject;
        }

        public void Show<TScreen>(TScreen screen, bool closeCurrentScreen = true, Action endAction = null, bool skipShowAnim = false) where TScreen : GuiScreen
        {
            if (!screen) return;
            
            // Debug.Log(_currentScreen.);
            if (closeCurrentScreen && _currentScreen)
                _currentScreen.Hide();
            
            screen.Show(endAction, skipShowAnim);
            _currentScreen = screen;

            Time.timeScale = screen.Pause ? 0 : 1;
            
            for (int i = 0; i < universalElements.Length; i++)
            {
                var ue = universalElements[i];

                if (ue is LookAroundContainer) UpdateUniversalPanel(ue, screen.UseLookAround);
                else if (ue is MainMenuFon) UpdateUniversalPanel(ue, screen.UseMainMenuFon);
                else if (ue is TopUiContainer) UpdateUniversalPanel(ue, screen.UseTopUiContainer);
            }

            bool UpdateUniversalPanel(GuiUniversalElement ue, bool needShow)
            {
                if (needShow) ue.Show();
                else ue.Hide();

                return needShow;
            }
        }

        public void Show(Type screenType)
        {
            GuiScreen screen = FindScreen(screenType);
            Show(screen);
        }

        public GuiScreen FindScreen(Type screenType)
        {
            foreach (var screen in screens)
            {
                if (screen.GetType() == screenType) return screen;
            }

            return null;
        }

        public T FindScreen<T>() where T : GuiScreen
        {
            Debug.Log("FindScreen " + typeof(T));
            
            int index = ArrayUtility.FastFindElement<GuiScreen, T>(screens);

            if (index == -1)
            {
                Debug.LogError($"Not found screen {typeof(T)}");
                return null;
            }
            else return (T)(screens[index]);
        }

        public GuiUniversalElement FindUniversalElement(Type universalElementType)
        {
            foreach (var universalElement in universalElements)
            {
                if (universalElement.GetType() == universalElementType) return universalElement;
            }

            return null;
        }

        public T FindUniversalElement<T>() where T : GuiUniversalElement
        {
            int index = ArrayUtility.FastFindElement<GuiUniversalElement, T>(universalElements);

            if (index == -1)
            {
                Debug.LogError($"Not found universal element {typeof(T)}");
                return null;
            }
            else return (T)(universalElements[index]);
        }

        [ContextMenu("GetScreens")]
        private void GetScreens()
        {
            screens = GetComponentsInChildren<GuiScreen>(true);
            universalElements = GetComponentsInChildren<GuiUniversalElement>(true);
        }

        [ContextMenu("SortScreens")]
        private void SortScreens()
        {
            System.Array.Sort(screens);
            System.Array.Sort(universalElements);
        }

        public void Set(GuiScreen[] newScreens, GuiUniversalElement[] newUniversalElements)
        {
            this.screens = newScreens;
            this.universalElements = newUniversalElements;

            if (showMainScreenByStart && mainScreen != null) Show(mainScreen, false);

            foreach (var screen in screens)
            {
                if (screen == null) continue;
                if (deactivateOtherScreensByStart && screen.gameObject.activeSelf) screen.gameObject.SetActive(false);

                screen.Init();

                if (screen.GetType() == mainScreenType) mainScreen = screen;
            }

            foreach (var ue in universalElements)
            {
                if (ue == null) continue;
                if (deactivateOtherScreensByStart && ue.gameObject.activeSelf) ue.gameObject.SetActive(false);

                ue.Init();
            }
        }
    }
}
using UnityEngine;
using Zenject;

namespace TheSTAR.GUI
{
    public class MenuScreen : GuiScreen
    {
        [SerializeField] private PointerButton playButton;
        [SerializeField] private PointerButton settingsButton;
        
        private GuiController gui;
        private GameController game;

        [Inject]
        private void Construct(GuiController gui, GameController game)
        {
            this.gui = gui;
            this.game = game;
        }

        public override void Init()
        {
            base.Init();

            settingsButton.Init(() =>
            {
                var settingsScreen = gui.FindScreen<SettingsScreen>();
                settingsScreen.Init(this, true);
                gui.Show(settingsScreen);
            });

            playButton.Init(() =>
            {
                game.StartGame();
            });
        }
    }
}
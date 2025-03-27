using UnityEngine;
using Zenject;
using TheSTAR.Data;
using TheSTAR.GUI;
using TheSTAR.Sound;
using TheSTAR.Utility;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    private DataController data;
    private GuiController gui;
    private SoundController sound;

    private readonly ResourceHelper<GameConfig> gameConfig = new("Configs/GameConfig");
    public GameConfig GameConfig => gameConfig.Get;

    [Inject]
    private void Construct(DataController data, SoundController sound)
    {
        this.data = data;
        this.sound = sound;

        data.Init(gameConfig.Get.LockData);
    }

    private void Start()
    {
        sound.Init(() => data.gameData.settingsData.isSoundsOn, () => data.gameData.settingsData.isMusicOn);

        LoadGameScene();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }
}

public enum GameAbVersionType
{
    VersionA,
    VersionB
}
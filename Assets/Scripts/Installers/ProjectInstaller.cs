using UnityEngine;
using Zenject;
using TheSTAR.Data;
using TheSTAR.Sound;

/// <summary>
/// Здесь биндим в контексте всего проекта
/// </summary>
public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameLoader gameControllerPrefab;
    [SerializeField] private SoundController soundControllerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<DataController>().AsSingle();
        Container.Bind<DailyBonusService>().AsSingle();
        Container.Bind<NotificationController>().AsSingle();
        Container.Bind<CurrencyController>().AsSingle();
        Container.Bind<XpController>().AsSingle();
        Container.Bind<AnalyticsManager>().AsSingle();
        Container.Bind<AdsManager>().AsSingle();
        Container.Bind<VibrationController>().AsSingle();
        Container.Bind<AllPrices>().AsSingle();
        Container.Bind<OrdersManager>().AsSingle();

        var soundController = Container.InstantiatePrefabForComponent<SoundController>(soundControllerPrefab, soundControllerPrefab.transform.position, Quaternion.identity, null);
        Container.Bind<SoundController>().FromInstance(soundController).AsSingle();

        var gameLoader = Container.InstantiatePrefabForComponent<GameLoader>(gameControllerPrefab, gameControllerPrefab.transform.position, Quaternion.identity, null);
        Container.Bind<GameLoader>().FromInstance(gameLoader).AsSingle();        
    }
}
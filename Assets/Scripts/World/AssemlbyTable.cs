using UnityEngine;
using Zenject;

public class AssemlbyTable : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;

    private GameWorldInteraction worldInteraction;

    [Inject]
    private void Construct(GameWorldInteraction worldInteraction)
    {
        this.worldInteraction = worldInteraction;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        touchInteractive.OnClickEvent += () => worldInteraction.OnAssemblyTableClick(this);
    }
}
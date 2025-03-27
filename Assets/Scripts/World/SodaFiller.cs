using UnityEngine;
using Zenject;

public class SodaFiller : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private Filler filler;
    [SerializeField] private ItemType to;

    public Filler Filler => filler;
    public ItemType To => to;

    private GameWorldInteraction worldInteraction;

    [Inject]
    private void Construct(GameWorldInteraction worldInteraction)
    {
        this.worldInteraction = worldInteraction;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        touchInteractive.OnClickEvent += () => worldInteraction.OnSodaFillerClick(this);
    }
}
using UnityEngine;
using Zenject;

public class SousContainer : MonoBehaviour
{
    [SerializeField] private TouchInteractive touch;
    [SerializeField] private ItemType sousType;
    [SerializeField] private Transform createItemPos;

    private GameWorldInteraction worldInteraction;

    public ItemType SousType => sousType;
    public Transform CreateItemPos => createItemPos;
    public TouchInteractive Touch => touch;

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
        touch.OnClickEvent += () => worldInteraction.OnSousContainerClick(this);
    }
}
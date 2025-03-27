using UnityEngine;
using World;
using Zenject;

public class BuyerCash : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    private BuyerPlace place;
    private DollarValue dollarValue;
    private int xpValue;

    public BuyerPlace Place => place;
    public DollarValue DollarValue => dollarValue;
    public int XpValue => xpValue;
    public TutorInWorldFocus TutorFocus => tutorFocus;

    private GameWorldInteraction worldInteraction;

    [Inject]
    private void Construct(GameWorldInteraction worldInteraction)
    {
        this.worldInteraction = worldInteraction;
    }

    public void Init(BuyerPlace place)
    {
        this.place = place;
        
        touchInteractive.OnClickEvent += () =>
        {
            worldInteraction.OnCashClick(this);
        };
    }

    public void SetValue(DollarValue dollarValue, int xpValue)
    {
        this.dollarValue = dollarValue;
        this.xpValue = xpValue;
    }
}
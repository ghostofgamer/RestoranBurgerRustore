using TheSTAR.Utility;
using TMPro;
using UnityEngine;
using Zenject;

public class CostPanel : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    private GameWorldInteraction worldInteraction;

    private ItemType itemType;
    public ItemType ItemType => itemType;
    public TutorInWorldFocus TutorFocus => tutorFocus;

    public void Init(ItemType itemType)
    {
        this.itemType = itemType;
    }

    [Inject]
    private void Construct(GameWorldInteraction worldInteraction)
    {
        this.worldInteraction = worldInteraction;

        touchInteractive.OnClickEvent += () =>
        {
            this.worldInteraction.OnCostPanelClick(this);
        };
    }

    public void DisplayCost(DollarValue dollarValue)
    {
        costText.text = TextUtility.FormatPrice(dollarValue, true);
    }
}
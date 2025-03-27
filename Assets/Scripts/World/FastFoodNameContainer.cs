using TheSTAR.Data;
using TMPro;
using UnityEngine;
using Zenject;

public class FastFoodNameContainer : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private TextMeshProUGUI[] nameTitles;
    [SerializeField] private TutorInWorldFocus tutorFocus;

    private const string DefaultName = "FAST FOOD";

    public TutorInWorldFocus TutorFocus => tutorFocus;

    private DataController data;
    private GameWorldInteraction worldInteraction;
    
    [Inject]
    private void Construct(DataController data, GameWorldInteraction worldInteraction)
    {
        this.data = data;
        this.worldInteraction = worldInteraction;
    }

    public void Init()
    {
        touchInteractive.OnClickEvent += () =>
        {
            worldInteraction.OnDinerNameContainerClick(this);
        };

        var storeName = data.gameData.levelData.fastFoodName;
        if (string.IsNullOrEmpty(storeName)) storeName = DefaultName;

        DisplayName(storeName);
    }

    public void DisplayName(string newName)
    {
        foreach (var nameText in nameTitles)
        {
            nameText.text = newName;   
        }
    }
}
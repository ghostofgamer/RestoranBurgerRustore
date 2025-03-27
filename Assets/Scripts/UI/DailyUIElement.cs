using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyUIElement : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image bgImg;
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI dayTitle;
    [SerializeField] private TextMeshProUGUI rewardValueTitle;

    public void Init(int index, bool visible, Sprite iconSprite, int rewardValue)
    {
        dayTitle.text = $"DAY {index + 1}";
        rewardValueTitle.text = $"{rewardValue}";
        SetVisible(visible);

        iconImg.sprite = iconSprite;
    }

    private void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1 : 0.7f;
    }
}
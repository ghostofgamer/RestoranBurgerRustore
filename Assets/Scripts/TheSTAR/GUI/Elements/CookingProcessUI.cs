using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingProcessUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Image fillImg;
    [SerializeField] private GameObject progressbarObject;

    public void SetTitle(string title)
    {
        this.title.text = title;
    }

    public void SetProgress(float progress)
    {
        fillImg.fillAmount = progress;
    }

    public void SetUseProgressbar(bool use)
    {
        progressbarObject.SetActive(use);
    }
}
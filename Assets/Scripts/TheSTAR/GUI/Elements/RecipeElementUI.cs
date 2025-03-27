using UnityEngine;
using UnityEngine.UI;

public class RecipeElementUI : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private GameObject checkObject;

    public void SetIcon(Sprite icon)
    {
        iconImg.sprite = icon;
    }

    public void SetCompleted(bool completed)
    {
        checkObject.SetActive(completed);
    }
}
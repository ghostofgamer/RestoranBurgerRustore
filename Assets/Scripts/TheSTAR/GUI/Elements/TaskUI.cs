using TheSTAR.GUI;
using TMPro;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    [SerializeField] private PointerButton miniButton;
    [SerializeField] private PointerButton panelButton;
    [SerializeField] private TextMeshProUGUI text;

    public void Init()
    {
        miniButton.Init(() =>
        {
            miniButton.gameObject.SetActive(false);
            gameObject.SetActive(true);
        });

        panelButton.Init(() =>
        {
            miniButton.gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
    }

    public void SetTask(string text)
    {
        this.text.text = text;
        gameObject.SetActive(true);
    }

    public void ClearTask()
    {
        gameObject.SetActive(false);
    }
}
using UnityEngine;

/// <summary>
/// Коробка с возможностью открыть/закрыть
/// </summary>
public class BoxOpenClose : Box
{
    [SerializeField] private Transform[] openRotators;
    
    private const float CloseAngle = 0;
    private const float OpenAngle = 200;

    private bool isOpen = false;
    public bool IsOpen => isOpen;
    
    [ContextMenu("Open")]
    public void Open()
    {
        if (isOpen) return;

        isOpen = true;

        foreach (var element in openRotators)
        {
            element.transform.localEulerAngles = new Vector3(element.transform.localEulerAngles.x, element.transform.localEulerAngles.y, OpenAngle);
        }

        splitter.gameObject.SetActive(true);

        //analytics.Trigger(RepeatingEventType.OpenBox);
    }

    [ContextMenu("Close")]
    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        foreach (var element in openRotators)
        {
            element.transform.localEulerAngles = new Vector3(element.transform.localEulerAngles.x, element.transform.localEulerAngles.y, CloseAngle);
        }

        splitter.gameObject.SetActive(false);

        //analytics.Trigger(RepeatingEventType.CloseBox);
    }
}
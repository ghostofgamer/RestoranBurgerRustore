using UnityEngine;

public class DisableOnAnimationEnd : MonoBehaviour
{
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
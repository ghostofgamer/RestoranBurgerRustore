using UnityEngine;

public class TutorPoint : MonoBehaviour
{
    private bool currentlyInFOV = true;
    public bool CurrentlyInFOV => currentlyInFOV;   
    
    public void SetInFOV(bool inFOV)
    {
        // Debug.Log("выкл или вкл " + inFOV);
        this.currentlyInFOV = inFOV;
        gameObject.SetActive(inFOV);
    }
}
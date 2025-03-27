using System;
using UnityEngine;

public class RayVision : MonoBehaviour
{
    [SerializeField] private float visionDistance = 5;
    
    private TouchInteractive currentFocus;
    private Collider currentFocusCollider;

    public TouchInteractive CurrentFocus => currentFocus;

    public event Action<TouchInteractive> OnChangeCurrentFocusEvent;

    private int currentLayerMask;
    
    void Update()
    {   
        SendRay();
    }

    private void SendRay()
    {
        // Raycast from the position of this GameObject in its forward direction
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Send the ray
        if (Physics.Raycast(ray, out hit, visionDistance, currentLayerMask))
        {
            if (hit.collider == currentFocusCollider) return;

            //currentHitObject = hit.collider.gameObject;

            var touchInteractive = hit.collider.GetComponent<TouchInteractive>();

            if (touchInteractive)
            {
                if (currentFocus != null) currentFocus.EndFocus();

                touchInteractive.StartFocus();
                currentFocus = touchInteractive;
                currentFocusCollider = hit.collider;
                OnChangeCurrentFocusEvent?.Invoke(currentFocus);
            }
            else if (currentFocus != null)
            {
                currentFocus.EndFocus();
                currentFocus = null;
                currentFocusCollider = null;
                OnChangeCurrentFocusEvent?.Invoke(currentFocus);
            }
        }
        else if (currentFocus != null)
        {
            //currentHitObject = null;
            currentFocus.EndFocus();
            currentFocus = null;
            currentFocusCollider = null;
            OnChangeCurrentFocusEvent?.Invoke(currentFocus);
        }
    }

    public void SetCurrentLayerMask(int currentLayerMask)
    {
        this.currentLayerMask = currentLayerMask;
    }

    public void Deactivate()
    {
        enabled = false;
        if (currentFocus)
        {
            currentFocus.EndFocus();
            currentFocus = null;
            currentFocusCollider = null;
            OnChangeCurrentFocusEvent?.Invoke(currentFocus);
        }
    }

    public void Activate()
    {
        enabled = true;
    }
}

public enum PlayerInteractionScenario
{
    EmptyHands,
    ClosedBoxInHands,
    OpenBoxInHands,
    EmptyBoxInHands,
    DefaultItemInHands,
    EmptySodaCupDefaultItemInHands,
    OrderTrayInHands,
    ContainerWithAvailablePlacesInHands // группа котлет с доступными местами в руках
}
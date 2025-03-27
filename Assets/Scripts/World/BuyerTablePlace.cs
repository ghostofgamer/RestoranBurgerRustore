using System;
using System.Collections;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class BuyerTablePlace : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private DraggerGroup group;
    [SerializeField] private TutorInWorldFocus tutorInWorldFocus;
    // [SerializeField] private Reputation _reputation;
    
    private GameWorldInteraction worldInteraction;
    
    public TutorInWorldFocus TutorInWorldFocus => tutorInWorldFocus;
    public List<Draggable> OrderItems => group.AllDraggables;
    public DraggerGroup Group => group;

    private int index;
    public int Index => index;

    [Inject]
    private void Construct(GameWorldInteraction worldInteraction)
    {
        this.worldInteraction = worldInteraction;
    }

    /*private void Start()
    {
        StartCoroutine(FindReputationWithDelay(3.0f));
    }*/
    
    /*private IEnumerator FindReputationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _reputation = FindObjectOfType<Reputation>();
        
        if (_reputation != null)
        {
            Debug.Log("Reputation object found!");
        }
        else
        {
            Debug.Log("Reputation object not found.");
        }
    }*/

    public void Init(int index)
    {
        this.index = index;
        group.Init();
        touchInteractive.OnClickEvent += () => worldInteraction.OnBuyerTablePlaceClick(this);
    }

    public void DecreasePer()
    {
        // _reputation.DecreaseReputation();
    }
    
    /*
    public void Activate()
    {   
        touchInteractive.Activate();
    }

    public void Deactivate()
    {
        touchInteractive.Deactivate();
    }
    */
}
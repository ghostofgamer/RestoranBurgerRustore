using System;
using TheSTAR.Data;
using UnityEngine;
using Zenject;

public class LoaderCompletedBurger : MonoBehaviour
{
    [SerializeField] private Item _finallyBurger;
    [SerializeField] private Dragger _dragger;
    
    private DiContainer diContainer;
    
    [Inject]
    private void Construct(DataController data, DiContainer diContainer)
    {
        this.diContainer = diContainer;
    }

    private void Start()
    {
        var item= diContainer.InstantiatePrefabForComponent<Item>(_finallyBurger, transform);
        item.GetComponent<DraggableByPlayer>().SetDragger(_dragger);
        var dragger = item.GetComponent<DraggableByPlayer>();
        dragger.Rigidbody.isKinematic = true;
        dragger.Col.enabled = false;
        _dragger.SetCurrentItem(item.GetComponent<Draggable>());
    }
}
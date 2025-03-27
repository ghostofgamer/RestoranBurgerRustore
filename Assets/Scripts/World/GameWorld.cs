using DecorationContent;
using ReputationContent;
using UnityEngine;
using Unity.AI.Navigation;
using Zenject;
using TheSTAR.Data;
using TheSTAR.GUI;

public class GameWorld : MonoBehaviour
{
    [SerializeField] private FastFood fastFood;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private BuyerPlaceFurnitureUnit[] _placesFurnitureUnitTutor;
    [SerializeField] private DecorationSystem _decorationSystem;

    public DecorationSystem DecorationSystem => _decorationSystem;
    public Transform PlayerSpawnPoint => playerSpawnPoint;
    public FastFood FastFood => fastFood; 
    public BuyerPlaceFurnitureUnit[] BuyerPlacesFurnitureUnitTutor => _placesFurnitureUnitTutor;
    
    [ContextMenu("BakeNavigationSurface")]
    public void BakeNavigationSurface()
    {
        navMeshSurface.BuildNavMesh();
    }

    public void Init(Reputation reputation,BuyerPlaceFurnitureUnit[] placesFurnitureUnitTutor)
    {
        _placesFurnitureUnitTutor = placesFurnitureUnitTutor;
        fastFood.Init(reputation);
    }

    public void Load()
    {
        fastFood.LoadItems();
    }

    public void ResetOrderTray(int index)
    {
        Debug.Log("ResetOrderTray");
        fastFood.ResetOrderTray(index);
    }
}
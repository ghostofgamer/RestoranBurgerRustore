using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

/// <summary>
/// Коллизионное зрение
/// </summary>
public class CollisionVision : MonoBehaviour
{
    [SerializeField] private CircleCollider2D snapshopCol;

    private string[] searchTags;

    private const float Duration = 0.1f;

    private Tweener shotTweener;
    private List<GameObject> objectsInViewField = new();
    private Action<GameObject> returnObjectInView;
    private Action<GameObject> onObjectFound;
    private Action<GameObject> onObjectLost;
    private SearchMode currentSearchMode;
    private SearchObjectCondition searchCondition;

    private bool searchForSingleObject = false;

    private void PrepareForSearch(int distance, string[] searchTags)
    {
        shotTweener?.Kill();

        this.searchTags = searchTags;
        snapshopCol.enabled = false;
        snapshopCol.radius = distance - 1; // нужно, чтобы избежать проблем расчёта дистанций
    }

    #region OneSearch

    public void OneSearchForObject(int distance, string searchTag, SearchObjectCondition searchCondition, Action<GameObject> returnObjectInView)
    {
        OneSearchForObject(distance, new string[] {searchTag}, searchCondition, returnObjectInView);
    }
    
    public void OneSearchForObject(int distance, string[] searchTags, SearchObjectCondition searchCondition, Action<GameObject> returnObjectInView)
    {
        currentSearchMode = SearchMode.OneSearch;
        searchForSingleObject = true;
        this.searchCondition = searchCondition;
        this.returnObjectInView = returnObjectInView;
        PrepareForSearch(distance, searchTags);
        snapshopCol.enabled = true;

        shotTweener =
        DOVirtual.Float(0, 1, Duration, (value) => { }).OnComplete(() =>
        {
            snapshopCol.enabled = false;
            returnObjectInView?.Invoke(null);
        });
    }

    public void OneSearchForObjects(int distance, string searchTag, SearchObjectCondition searchCondition, Action<List<GameObject>> returnObjectsInView)
    {
        OneSearchForObjects(distance, new string[] {searchTag}, searchCondition, returnObjectsInView);
    }
    
    public void OneSearchForObjects(int distance, string[] searchTags, SearchObjectCondition searchCondition, Action<List<GameObject>> returnObjectsInView)
    {
        this.searchCondition = searchCondition;

        currentSearchMode = SearchMode.OneSearch;
        searchForSingleObject = false;
        PrepareForSearch(distance, searchTags);

        objectsInViewField.Clear();

        snapshopCol.enabled = true;

        shotTweener =
        DOVirtual.Float(0, 1, Duration, (value) => { }).OnComplete(() =>
        {
            snapshopCol.enabled = false;
            returnObjectsInView?.Invoke(objectsInViewField);
        });
    }

    private void SingleObjectFound(GameObject _object)
    {
        shotTweener?.Kill();
        snapshopCol.enabled = false;
        returnObjectInView?.Invoke(_object);
        searchForSingleObject = false;
    }

    #endregion

    #region LongSearchForOne

    public void StartLongSearchForOne(int distance, string searchTag, SearchObjectCondition searchCondition, Action<GameObject> returnObjectInView)
    {
        StartLongSearchForOne(distance, new string[] {searchTag}, searchCondition, returnObjectInView);
    }

    public void StartLongSearchForOne(int distance, string[] searchTags, SearchObjectCondition searchCondition, Action<GameObject> returnObjectInView)
    {
        currentSearchMode = SearchMode.LongSearchForOne;
        this.searchCondition = searchCondition;
        this.returnObjectInView = returnObjectInView;
        PrepareForSearch(distance, searchTags);
        snapshopCol.enabled = true;
    }

    #endregion

    #region LongSearchConstant

    public void StartLongSearchConstant(int distance, string searchTag, SearchObjectCondition searchCondition, Action<GameObject> onObjectFound, Action<GameObject> onObjectLost)
    {
        StartLongSearchConstant(distance, new string[] {searchTag}, searchCondition, onObjectFound, onObjectLost);
    }

    public void StartLongSearchConstant(int distance, string[] searchTags, SearchObjectCondition searchCondition, Action<GameObject> onObjectFound, Action<GameObject> onObjectLost)
    {
        //Debug.Log("StartLongSearchConstant");

        currentSearchMode = SearchMode.LongSearchConstant;
        this.searchCondition = searchCondition;
        this.onObjectFound = onObjectFound;
        this.onObjectLost = onObjectLost;
        PrepareForSearch(distance, searchTags);
        snapshopCol.enabled = true;
    }

    #endregion
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("Trigger enter " + col.tag);
        if (!searchTags.Contains(col.tag)) return;
        if (searchCondition != null && !searchCondition(col.gameObject)) return;

        switch (currentSearchMode)
        {
            case SearchMode.OneSearch:
                if (searchForSingleObject) SingleObjectFound(col.gameObject);
                else objectsInViewField.Add(col.gameObject);
                break;

            case SearchMode.LongSearchForOne:
                returnObjectInView?.Invoke(col.gameObject);
                BreakSearch();
                break;

            case SearchMode.LongSearchConstant:
                onObjectFound?.Invoke(col.gameObject);
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (currentSearchMode != SearchMode.LongSearchConstant) return;
        if (searchCondition != null && !searchCondition(col.gameObject)) return;

        onObjectLost?.Invoke(col.gameObject);
    }

    public void BreakSearch()
    {
        // test only
        //if (currentSearchMode == SearchMode.LongSearchConstant) Debug.Log("BreakLongSearchConstant");

        shotTweener?.Kill();
        objectsInViewField.Clear();
        returnObjectInView = null;
        onObjectFound = null;
        onObjectLost = null;
        snapshopCol.enabled = false;
    }
}

public delegate bool SearchObjectCondition(GameObject obj);

public enum SearchMode
{
    /// <summary>
    /// Одиночный снимок с поиском объекта
    /// </summary>
    OneSearch,

    /// <summary>
    /// Долговременная активация зрения до тех пор, пока не будет найден объект, автоматическое прерывание после находки
    /// </summary>
    LongSearchForOne,

    /// <summary>
    /// Постоянный поиск, улавливается находка и потеря обхектов, автоматического прерывания не происходит
    /// </summary>
    LongSearchConstant
}
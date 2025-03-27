using System.Collections.Generic;
using UnityEngine;
using TheSTAR.Utility;
using TheSTAR.Data;
using Zenject;
using System;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private TutorCursor cursor;
    [SerializeField] private TutorPoint tutorPointPrefab;
    [SerializeField] private Transform pointsContainer;

    [Space] [SerializeField] private RectTransform upPos;
    [SerializeField] private RectTransform bottomPos;
    [SerializeField] private RectTransform leftPos;
    [SerializeField] private RectTransform rightPos;

    private bool showDebugs = false;

    [Header("Test")] [SerializeField] private CursorViewType testCursorViewType;

    private List<TutorPoint> createdPoints = new();

    private DataController data;
    private AnalyticsManager analytics;

    private bool _inTutorial = false;
    private Transform[] _currentFocusTran;
    private TutorInWorldFocus[] currentWorldFocus;
    private TutorialBasingType? _currentTutorialBasingType;
    private bool _autoUpdatePos = false;
    private TutorialType? _currentTutorial = null;
    private bool currentlyInFOV = false;

    private CheckInFovDelegate checkInFovDelegate;

    public event Action<TutorialType, TutorialData> OnStartTutorialEvent;
    public event Action OnBreakTutorialEvent;
    public event Action OnCompleteTutorialEvent;

    public readonly Dictionary<TutorialType, TutorialData> tutorialDatas = new()
    {
        { TutorialType.LookAround, new() },
        { TutorialType.Move, new() },
        { TutorialType.LiftingBox, new("Pic Up Box Burgers") },
        { TutorialType.ClearTrash, new("Throw in the trash BOX WITH BURGER BUN ") },
        { TutorialType.FirstDelivery, new("Order products: {0}") },
        { TutorialType.GetFirstDelivery, new("PICK UP A BOX OF BURGER BUN.") },
        { TutorialType.CutBun, new("Place the burger bun in the tray") },
        { TutorialType.PlacePackingBoxToShelf, new("Opeun Box and Place the Burger Boxes on the shelf") },
        { TutorialType.PlaceCutletToTray, new("Place the burger patty in the tray") },
        { TutorialType.GetFourCutletsInHands, new("You can hold up to 4 cutlets on the tray!") },
        { TutorialType.PlaceCutletToGrill, new("Place the burger patty on the grill") },
        { TutorialType.FryCutlet, new("Fry the burger patty") },
        { TutorialType.TakeCutlet, new("Take the burger patty") },
        { TutorialType.TakeToAssemblyTable, new("Take cutlets To Assembly Table") },
        { TutorialType.AssemblyBurger, new("Make the first s burger") },
        { TutorialType.SetPrice, new("Great! You've learned how to make burgers! Let's set the price!") },
        { TutorialType.SetFastFoodName, new("Let's name the restaurant!") },
        { TutorialType.OpenFastFood, new("It's time to open a fast food restaurant!") },
        { TutorialType.ExpectBuyers, new("Expect customers after the restaurant opens!") },
        { TutorialType.AcceptOrder, new("Accept Order!") },
        { TutorialType.PutBurgerOnTray, new("Put the burger on the tray") },
        { TutorialType.PickUpTray, new("Pick up the tray") },
        { TutorialType.ServeTheQuests, new("Serve the guests: {0}") },
        { TutorialType.ClearTables, new("Clear table") },
        { TutorialType.BuyChair, new("Buy a chair") },
        { TutorialType.BuySection, new("Buy a selection 1") },

        // todo выполнять это когда назначили цены
        { TutorialType.UpdateMenu_Cheeseburger, new("Your menu has been updated, change the price tag!") },
        { TutorialType.UpdateMenu_burgerM, new("Your menu has been updated, change the price tag!") },
        { TutorialType.UpdateMenu_frenchFries, new("Your menu has been updated, change the price tag!") },
        { TutorialType.UpdateMenu_starburger, new("Your menu has been updated, change the price tag!") },
        { TutorialType.UpdateMenu_soda, new("Your menu has been updated, change the price tag!") },
        { TutorialType.UpdateMenu_mega, new("Your menu has been updated, change the price tag!") },
    };

    public bool UseTutorials => true; // data.gameData.tutorialData.useTutorials;

    public TutorialType? CurrentTutorial => _currentTutorial;
    public bool InTutorial => _inTutorial;

    public bool IsCompleted(TutorialType tutorialType) => data.gameData.tutorialData.completedTutorials != null &&
                                                          data.gameData.tutorialData.completedTutorials.Contains(
                                                              tutorialType);

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        checkInFovDelegate ??= (c) => true;
    }

    private void Start()
    {
        // Debug.Log("CURRENT TUTORIAL " + _currentTutorial);
    }

    private void FixedUpdate()
    {
        if (CurrentTutorial.HasValue)
        {
            Debug.Log("Current Tutorial: " + CurrentTutorial.Value);
        }
        else
        {
            Debug.Log("No current tutorial set.");
        }
    }

    [Inject]
    private void Constuct(DataController data, AnalyticsManager analytics)
    {
        this.data = data;
        this.analytics = analytics;
    }

    public void Init(CheckInFovDelegate checkInFovDelegate)
    {
        this.checkInFovDelegate = checkInFovDelegate;
    }

    #region Show

    public void TryShowInUI(
        TutorialType tutorialType,
        Transform focusTran,
        bool autoUpdatePos = false)
    {
        TryShowInUI(
            tutorialType,
            focusTran,
            out _, autoUpdatePos);
    }

    public void TryShowInUI(
        TutorialType tutorialType,
        Transform focusTran,
        out bool successful,
        bool autoUpdatePos = false)
    {
        Show(
            tutorialType,
            focusTran,
            autoUpdatePos,
            TutorialBasingType.UI,
            out successful);
    }

    public void TryShowInWorld(
        TutorialType tutorialType,
        TutorInWorldFocus focus)
    {
        TryShowInWorld(tutorialType, focus, out _);
    }

    public void TryShowInWorld(
        TutorialType tutorialType,
        TutorInWorldFocus focus,
        out bool successful)
    {
        TryShowInWorld(
            tutorialType,
            new TutorInWorldFocus[] { focus },
            out successful);
    }

    public void TryShowInWorld(
        TutorialType tutorialType,
        TutorInWorldFocus[] focus,
        out bool successful)
    {
        currentWorldFocus = focus;
        Transform[] focusTran = new Transform[focus.Length];
        for (int i = 0; i < focus.Length; i++)
            focusTran[i] = focus[i].FocusTran;
        Show(tutorialType, focusTran, true, TutorialBasingType.World, out successful);
    }

    private void Show(
        TutorialType tutorialType,
        Transform focusTran,
        bool autoUpdatePos,
        TutorialBasingType basingType,
        out bool successful,
        bool force = false)
    {
        Show(tutorialType, new Transform[] { focusTran }, autoUpdatePos, basingType, out successful, force);
    }

    private void Show(
        TutorialType tutorialType,
        Transform[] focusTran,
        bool autoUpdatePos,
        TutorialBasingType basingType,
        out bool successful,
        bool force = false)
    {
        successful = false;

        if (!force && IsCompleted(tutorialType)) return;
        if (_inTutorial)
        {
            if (CurrentTutorial == tutorialType && focusTran == _currentFocusTran) return;
            else BreakTutorial();
        }

        if (showDebugs) Debug.Log("[tutor] Show Tutor " + tutorialType);

        _inTutorial = true;
        _currentTutorial = tutorialType;
        gameObject.SetActive(true);

        _currentTutorialBasingType = basingType;
        _autoUpdatePos = autoUpdatePos;
        _currentFocusTran = focusTran;
        currentlyInFOV = transform;

        if (basingType == TutorialBasingType.UI)
        {
            cursor.gameObject.SetActive(true);
        }
        else
        {
            while (createdPoints.Count < focusTran.Length)
            {
                Debug.Log("[tutor] Create Point");
                TutorPoint newPoint = Instantiate(tutorPointPrefab, pointsContainer);
                createdPoints.Add(newPoint);
                newPoint.SetInFOV(true);

                /*Debug.Log("[tutor] Create Point");
                createdPoints.Add(Instantiate(tutorPointPrefab, pointsContainer));*/
            }

            for (int i = 0; i < _currentFocusTran.Length; i++)
            {
                // bool inFOV = checkInFovDelegate(currentWorldFocus[i].Col);
                // createdPoints[i].SetInFOV(inFOV);
            }
        }

        UpdateCursorPosition();

        successful = true;

        Debug.Log("ТУТОР ТАЙПЕ " + tutorialType);
        OnStartTutorialEvent?.Invoke(tutorialType, tutorialDatas[tutorialType]);
    }

    #endregion Show

    /*
    private void SetCursorVisual(bool toRight, bool toUp)
    {
        CursorViewType cursorViewType;

        if (toUp)
        {
            if (toRight) cursorViewType = CursorViewType.ToUpToRight;
            else cursorViewType = CursorViewType.ToUpToLeft;
        }
        else
        {
            if (toRight) cursorViewType = CursorViewType.ToBottomToRight;
            else cursorViewType = CursorViewType.ToBottomToLeft;
        }

        cursor.SetTransformData(cursorViewType);
    }
    */

    /// <summary>
    /// Туториал выполнен, он не будет больше показываться
    /// </summary>
    public void CompleteCurrentTutorial()
    {
        if (!InTutorial) return;
        CompleteTutorial((TutorialType)_currentTutorial);
    }

    public void CompleteTutorial(TutorialType tutorialType)
    {
        if (!IsCompleted(TutorialType.SetFastFoodName))
        {
            Debug.Log("IF TUTORIAL COMPELTE " + tutorialType);
            data.gameData.tutorialData.CompleteTutorial(tutorialType);
            data.Save(DataSectionType.Tutorial);
        }
        /*else if(IsCompleted(TutorialType.SetFastFoodName)&&!IsCompleted(TutorialType.ClearTables))
        {
            Debug.Log("ELSE IF TUTORIAL COMPELTE " + tutorialType);
            data.gameData.tutorialData.CompleteTutorial(tutorialType);
        }*/
        else if(IsCompleted(TutorialType.SetFastFoodName)&&!IsCompleted(TutorialType.ServeTheQuests))
        {
            Debug.Log("ELSE IF TUTORIAL COMPELTE " + tutorialType);
            data.gameData.tutorialData.CompleteTutorial(tutorialType);
        }
        else if(IsCompleted(TutorialType.ServeTheQuests)&&!IsCompleted(TutorialType.ClearTables))
        {
            data.gameData.tutorialData.CompleteTutorial(tutorialType);
            data.Save(DataSectionType.Tutorial);
        }
        else if(IsCompleted(TutorialType.ExpectBuyers)&&IsCompleted(TutorialType.ClearTables))
        {
            Debug.Log("ELSE IF CLEAR TABLE TUTORIAL COMPELTE " + tutorialType);
            data.gameData.tutorialData.CompleteTutorial(tutorialType);
            data.Save(DataSectionType.Tutorial);
        }
        
        /*data.gameData.tutorialData.CompleteTutorial(tutorialType);
        data.Save(DataSectionType.Tutorial);*/
        HideTutor();

        analytics.LogForTutorial(tutorialType);

        OnCompleteTutorialEvent?.Invoke();
    }

    /// <summary>
    /// Туториал скрывается, но не считается завершённым. Он может быть показан позже
    /// </summary>
    public void BreakTutorial()
    {
        if (!InTutorial) return;

        HideTutor();

        OnBreakTutorialEvent?.Invoke();
    }

    private void HideTutor()
    {
        if (showDebugs) Debug.Log("[tutor] Hide Tutor");

        _inTutorial = false;
        _currentTutorial = null;
        _autoUpdatePos = false;
        gameObject.SetActive(false);
        
        
        
        cursor.gameObject.SetActive(false);

        Debug.Log("Hide Tutor");
        foreach (var point in createdPoints)
            point.gameObject.SetActive(false);

        _currentTutorialBasingType = null;
        _currentFocusTran = null;

        //SaveData();
    }

    /*public void UpdateCursorPosition()
    {
        if (!_inTutorial) return;

        Vector3 focusPos;
        Vector3 focusScreenPos;

        switch (_currentTutorialBasingType)
        {
            case TutorialBasingType.UI:
                focusPos = _currentFocusTran[0].position;
                focusScreenPos = focusPos;
                cursor.transform.position = focusScreenPos;
                break;

            case TutorialBasingType.World:
                for (int i = 0; i < _currentFocusTran.Length; i++)
                {
                    focusPos = _currentFocusTran[i].position;

                    focusScreenPos = Camera.main.WorldToScreenPoint(focusPos);
                    focusScreenPos = new Vector3(
                        MathUtility.Limit(focusScreenPos.x, leftPos.position.x, rightPos.position.x),
                        MathUtility.Limit(focusScreenPos.y, bottomPos.position.y, upPos.position.y),
                        createdPoints[i].transform.position.z);

                    createdPoints[i].transform.position = focusScreenPos;
                }
                break;
        }

        //bool toRight = cursor.transform.localPosition.x > -1;
        //bool toUp = cursor.transform.localPosition.y > -1;
        //SetCursorVisual(toRight, toUp);
    }*/

    /*public void UpdateCursorPosition()
    {
        if (!_inTutorial) return;

        Vector3 focusPos;
        Vector3 focusScreenPos;

        switch (_currentTutorialBasingType)
        {
            case TutorialBasingType.UI:
                focusPos = _currentFocusTran[0].position;
                focusScreenPos = focusPos;
                cursor.transform.position = focusScreenPos;
                break;

            case TutorialBasingType.World:
                for (int i = 0; i < _currentFocusTran.Length; i++)
                {
                    focusPos = _currentFocusTran[i].position;

                    focusScreenPos = Camera.main.WorldToScreenPoint(focusPos);
                    focusScreenPos = new Vector3(
                        MathUtility.Limit(focusScreenPos.x, leftPos.position.x, rightPos.position.x),
                        MathUtility.Limit(focusScreenPos.y, bottomPos.position.y, upPos.position.y),
                        createdPoints[i].transform.position.z);

                    // Ensure the cursor is fully visible on the screen
                    focusScreenPos.x = Mathf.Clamp(focusScreenPos.x, cursor.GetComponent<RectTransform>().rect.width / 2, Screen.width - cursor.GetComponent<RectTransform>().rect.width / 2);
                    focusScreenPos.y = Mathf.Clamp(focusScreenPos.y, cursor.GetComponent<RectTransform>().rect.height / 2, Screen.height - cursor.GetComponent<RectTransform>().rect.height / 2);

                    createdPoints[i].transform.position = focusScreenPos;
                }
                break;
        }

        //bool toRight = cursor.transform.localPosition.x > -1;
        //bool toUp = cursor.transform.localPosition.y > -1;
        //SetCursorVisual(toRight, toUp);
    }*/

    public void UpdateCursorPosition()
    {
        if (!_inTutorial) return;

        Vector3 focusPos;
        Vector3 focusScreenPos;
        Camera mainCamera = Camera.main;

        switch (_currentTutorialBasingType)
        {
            case TutorialBasingType.UI:
                // Debug.Log("[tutor] Update cursor position");
                focusPos = _currentFocusTran[0].position;
                focusScreenPos = mainCamera.WorldToScreenPoint(focusPos);
                cursor.transform.position = focusScreenPos;
                break;

            case TutorialBasingType.World:


                if (_currentFocusTran.Length > 0)
                {
                    foreach (var point in createdPoints)
                        point.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("НУЛ");
                }

                for (int i = 0; i < _currentFocusTran.Length; i++)
                {
                    focusPos = _currentFocusTran[i].position;
                    Vector3 directionToTarget = focusPos - mainCamera.transform.position;
                    float angleToTarget = Vector3.Angle(mainCamera.transform.forward, directionToTarget);
                    float thresholdAngle = 90f;

                    if (angleToTarget < thresholdAngle)
                    {
                        focusScreenPos = mainCamera.WorldToScreenPoint(focusPos);
                        /*focusScreenPos.x = Mathf.Clamp(focusScreenPos.x,
                            cursor.GetComponent<RectTransform>().rect.width / 2,
                            Screen.width - cursor.GetComponent<RectTransform>().rect.width / 2);
                        focusScreenPos.y = Mathf.Clamp(focusScreenPos.y,
                            cursor.GetComponent<RectTransform>().rect.height / 2,
                            Screen.height - cursor.GetComponent<RectTransform>().rect.height / 2);
                        focusScreenPos.z = 0;*/ // Ensure the Z position is 0 for UI elements

                        
                        focusScreenPos.x = Mathf.Clamp(focusScreenPos.x,
                            createdPoints[i].GetComponent<RectTransform>().rect.width / 2,
                            Screen.width - createdPoints[i].GetComponent<RectTransform>().rect.width / 2);
                        focusScreenPos.y = Mathf.Clamp(focusScreenPos.y,
                            createdPoints[i].GetComponent<RectTransform>().rect.height / 2,
                            Screen.height - createdPoints[i].GetComponent<RectTransform>().rect.height / 2);
                        focusScreenPos.z = 0;
                        
                        
                        
                        
                        
                        
                        createdPoints[i].transform.position = focusScreenPos;
                        // Debug.Log("IF");
                        // cursor.gameObject.SetActive(true);
                    }
                    else
                    {
                        Vector3 screenDirection = mainCamera.WorldToScreenPoint(focusPos) -
                                                  new Vector3(Screen.width / 2, Screen.height / 2, 0);
                        screenDirection.Normalize();
                        float edgeX = screenDirection.x > 0 ? 0 : Screen.width;
                        float edgeY = screenDirection.y > 0 ? 0 : Screen.height;

                        /*focusScreenPos.x = Mathf.Clamp(edgeX,
                            cursor.GetComponent<RectTransform>().rect.width / 2,
                            Screen.width - cursor.GetComponent<RectTransform>().rect.width / 2);
                        focusScreenPos.y = Mathf.Clamp(edgeY,
                            cursor.GetComponent<RectTransform>().rect.height / 2,
                            Screen.height - cursor.GetComponent<RectTransform>().rect.height / 2);
                        focusScreenPos.z = 0;*/
                        
                        
                        focusScreenPos.x = Mathf.Clamp(edgeX,
                            createdPoints[i].GetComponent<RectTransform>().rect.width / 2,
                            Screen.width - createdPoints[i].GetComponent<RectTransform>().rect.width / 2);
                        focusScreenPos.y = Mathf.Clamp(edgeY,
                            createdPoints[i].GetComponent<RectTransform>().rect.height / 2,
                            Screen.height - createdPoints[i].GetComponent<RectTransform>().rect.height / 2);
                        focusScreenPos.z = 0;
                        
                        
                        
                        
                        createdPoints[i].transform.position = focusScreenPos;
                        /*Debug.Log("ELSE");
                        cursor.gameObject.SetActive(false);*/
                    }
                }

                break;
        }

        //bool toRight = cursor.transform.localPosition.x > -1;
        //bool toUp = cursor.transform.localPosition.y > -1;
        //SetCursorVisual(toRight, toUp);
    }

    private void Update()
    {
        if (!_inTutorial) return;
        if (!_autoUpdatePos) return;

        UpdateCursorPosition();

        if (_currentTutorialBasingType == TutorialBasingType.World)
        {
            for (int i = 0; i < _currentFocusTran.Length; i++)
            {
                bool inFOV = checkInFovDelegate(currentWorldFocus[i].Col);
                if (inFOV != createdPoints[i].CurrentlyInFOV)
                {
                    createdPoints[i].SetInFOV(true);

                    // createdPoints[i].SetInFOV(inFOV);
                }
            }
        }
    }

    public enum TutorialBasingType
    {
        UI,
        World
    }

    [ContextMenu("TestUseView")]
    private void TestUseView()
    {
        cursor.SetTransformData(testCursorViewType);
    }
}

public struct CursorTransformData
{
    private Vector3 rotation;
    private Vector3 scale;

    public CursorTransformData(Vector3 rotation, Vector3 scale)
    {
        this.rotation = rotation;
        this.scale = scale;
    }

    public Vector3 Rotation => rotation;
    public Vector3 Scale => scale;
}

public interface ITutorialStarter
{
    void TriggerTutorial();
}

public enum CursorViewType
{
    Default,

    UpEdge,
    BottomEnge,
    LeftEdge,
    RightEdge,

    ToUpToRight,
    ToUpToLeft,
    ToBottomToRight,
    ToBottomToLeft,
}

public delegate bool CheckInFovDelegate(Collider col);

public enum TutorialType
{
    Congratulations,
    LookAround,
    Move,
    LiftingBox,
    FirstDelivery,
    GetFirstDelivery,
    CutBun,
    PlacePackingBoxToShelf,
    PlaceCutletToTray,
    GetFourCutletsInHands,
    PlaceCutletToGrill,
    FryCutlet,
    TakeCutlet,
    TakeToAssemblyTable,
    AssemblyBurger,
    SetPrice,
    SetFastFoodName,
    OpenFastFood,
    ExpectBuyers,
    AcceptOrder,
    PutBurgerOnTray,
    PickUpTray,
    ServeTheQuests,
    ClearTables,
    BuyChair,
    BuySection,

    UpdateMenu_Cheeseburger,
    UpdateMenu_burgerM,
    UpdateMenu_frenchFries,
    UpdateMenu_starburger,
    UpdateMenu_soda,
    UpdateMenu_mega,

    ClearTrash, // не успеваю
}

[Serializable]
public struct TutorialData
{
    private bool useTaskPanel;
    private string taskText;

    public bool UseTaskPanel => useTaskPanel;
    public string TaskText => taskText;

    public TutorialData(string taskText)
    {
        this.useTaskPanel = true;
        this.taskText = taskText;
    }
}
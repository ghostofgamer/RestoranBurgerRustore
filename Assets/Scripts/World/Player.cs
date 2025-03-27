using TheSTAR.Input;
using UnityEngine;
using DG.Tweening;
using TheSTAR.Utility;
using System;

public class Player : MonoBehaviour, IJoystickControlled, ICameraFocusable
{
    [SerializeField] private Transform messagePos;
    [SerializeField] private Transform rotator;
    [SerializeField] private DraggerSplitter itemsSplitter;
    [SerializeField] private Dragger defaultDragger;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private Transform draggerItemOffset; // смещение перетаскиваемого предмета в зависимости от типа предмета
    [SerializeField] private PlayerSteps steps;

    private const float DefaultSpeed = 3.5f;
    public const int MaxValueInPlayerStorage = 1;

    public Transform MessagePos => messagePos;
    public bool CanUseVipQueue => true;
    public Transform FocusTransform => cameraPos;

    private float speed => DefaultSpeed;

    public Draggable CurrentDraggable
    {
        get
        {
            if (defaultDragger.CurrentDraggable) return defaultDragger.CurrentDraggable;
            else return itemsSplitter.CurrentDraggable;
        }
    }

    public int ItemsInHandsCount
    {
        get
        {
            if (defaultDragger.CurrentDraggable) return 1;
            else return itemsSplitter.AllDraggables.Count;
        }
    }

    public event Action<Dragger, Draggable> OnPlayerStartDragEvent;
    public event Action<Draggable> OnPlayerEndDragEvent;

    public bool HavePlace(Draggable draggable, out Dragger place)
    {
        var item = draggable.GetComponent<Item>();
        if (item) return itemsSplitter.HavePlace(item.ItemType, out place);

        if (defaultDragger.CurrentDraggable)
        {
            place = null;
            return false;
        }

        place = defaultDragger;
        return true;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        itemsSplitter.Init();

        defaultDragger.OnStartDragEvent += OnStartDrag;
        itemsSplitter.OnSetItemEvent += OnStartDrag;

        defaultDragger.OnEndDragEvent += (dragger, draggable) => OnEndDrag(draggable);
        itemsSplitter.OnEndDragEvent += OnEndDrag;
    }

    private void OnStartDrag(Dragger dragger, Draggable draggable)
    {
        //Debug.Log("Player: OnStartDrag");

        var item = draggable.GetComponent<Item>();
        if (item) draggerItemOffset.transform.localPosition = new Vector3(0, -item.PlayerDragPos.localPosition.y, 0);
        else
        {
            var box = draggable.GetComponent<Box>();
            if (box) draggerItemOffset.transform.localPosition = new Vector3(0, -box.PlayerDragPos.localPosition.y, 0);
        }
        
        OnPlayerStartDragEvent?.Invoke(dragger, draggable);
    }

    private void OnEndDrag(Draggable draggable)
    {
        OnPlayerEndDragEvent?.Invoke(draggable);
    }

    #region Move

    public void JoystickInput(Vector2 input)
    {
        var velocity = new Vector3(input.x * speed, rb.linearVelocity.y, input.y * speed);
        rb.linearVelocity = velocity;
        steps.AddValue(input.magnitude * 0.08f);
    }

    #endregion

    private ItemType? currentItemType;
    public ItemType? CurrentItemType => currentItemType;

    #region Reacts

    public void OnIncomeTransactionReact(ItemType itemType, int finalValue)
    {
        //SetItem(itemType);
    }

    public void OnReduceTransactionReact(ItemType itemType, int finalValue)
    {
        currentItemType = null;
        //ClearCurrentVisualItem();
    }

    public void OnAnyTransactionReact(ItemType itemType, int finalValue)
    {}

    #endregion

    private Tweener speedBonusTweener;

    public void OnCompleteDrop() { }

    public void PrepareToDespawn()
    {
        if (speedBonusTweener != null)
        {
            speedBonusTweener.Kill();
            speedBonusTweener = null;
        }
    }

    private const float CameraRotationSpeed = 0.3f;

    public void RotateCam(Vector2 offset)
    {
        rotator.transform.localEulerAngles += new Vector3(-offset.y * CameraRotationSpeed, offset.x * CameraRotationSpeed, 0);
        //dragger.transform.localEulerAngles = new Vector3(-rotator.transform.localEulerAngles.x, 0, 0);

        float verticalAngle = rotator.transform.localEulerAngles.x;
        //Debug.Log("verticalAngle: " + verticalAngle);

        // todo добавить лимит поворота головы

        //bool toBottom = verticalAngle > 0 && verticalAngle < 180;
        bool inBoundsBottom = MathUtility.InBounds(verticalAngle, 0, 80);

        if (!inBoundsBottom)
        {
            bool inBoundsTop = MathUtility.InBounds(verticalAngle, 290, 360) || MathUtility.InBounds(verticalAngle, -70, 0);
            if (!inBoundsTop)
            {
                if (verticalAngle > 0 && verticalAngle < 180)
                {
                    rotator.transform.localEulerAngles = new (80, rotator.transform.localEulerAngles.y, 0);
                }
                else
                {
                    rotator.transform.localEulerAngles = new (290, rotator.transform.localEulerAngles.y, 0);
                }
            }
        }
        //else draggerRotationOffset.transform.localPosition = new Vector3(0, MathUtility.ProgressToValue((float)verticalAngle / 90, 0, DraggerBottomLookAtOffset), 0);
    }

    private const float DraggerBottomLookAtOffset = 0.3f; // насколько драггер смещается вперёд если игрок смотрит чётко вниз (когда смотрит чётко вперёд смещение равно нулю)
}
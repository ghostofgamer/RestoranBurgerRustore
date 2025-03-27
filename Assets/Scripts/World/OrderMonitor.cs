using UnityEngine;
using Configs;
using TheSTAR.Utility;
using TMPro;
using Zenject;
using World;

public class OrderMonitor : MonoBehaviour, ICameraFocusable
{
    [SerializeField] private TutorInWorldFocus _tutorFocus;
    [SerializeField]private Canvas _acceptOrderCanvas;
    [SerializeField]private Canvas _cancelOrderCanvas;
    
    [SerializeField] private GameObject emptyOrderTitle;
    [SerializeField] private OrderMonitorElement slot;
    [SerializeField] private GameObject orderScreen;
    [SerializeField] private GameObject cardPaymentScreen;
    [SerializeField] private GameObject cashPaymentScreen;
    [SerializeField] private TextMeshProUGUI[] totalCostText;
    [SerializeField] private TextMeshProUGUI receivedText;
    [SerializeField] private TextMeshProUGUI changeText;
    [SerializeField] private TextMeshProUGUI givingText;
    [SerializeField] private Transform cameraFocusPos;

    [Space]
    [SerializeField] private Color defaultTextColor;
    [SerializeField] private Color greenTextColor;
    [SerializeField] private Color redTextColor;

    [Space]
    [SerializeField] private TouchInteractive touchInteractive;

    private ConfigHelper<ItemsConfig> itemsConfig = new();

    private DollarValue totalCost;
    private DollarValue received;
    private DollarValue change;
    private DollarValue currentGiving;

    public TutorInWorldFocus TutorFocus =>_tutorFocus;
    private bool receivedCash => received.dollars > 0 || received.cents > 0;
    public Transform FocusTransform => cameraFocusPos;

    private GameWorldInteraction worldInteraction;
    
    private AllPrices _allPrices;

    [Inject]
    private void Construct(GameWorldInteraction worldInteraction,AllPrices allPrices)
    {
        this.worldInteraction = worldInteraction;
        _allPrices = allPrices;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        touchInteractive.OnClickEvent += () =>
        {
            worldInteraction.OnMonitorClick(this);
            Debug.Log("CLICK");
        };
    }

    public void SetActiveCanvas(bool value)
    {
        if(value)
        {
         _acceptOrderCanvas.gameObject.SetActive(true);
         _cancelOrderCanvas.gameObject.SetActive(false);
        }
        else
        {
            _acceptOrderCanvas.gameObject.SetActive(false);
            _cancelOrderCanvas.gameObject.SetActive(true);
        }
    }
    
    public void SetStatus(MonitorStatus status)
    {
        orderScreen.SetActive(false);
        cardPaymentScreen.SetActive(false);
        cashPaymentScreen.SetActive(false);

        if (status == MonitorStatus.Order) orderScreen.SetActive(true);
        else if (status == MonitorStatus.CardPayment) cardPaymentScreen.SetActive(true);
        else if (status == MonitorStatus.CashPayment) cashPaymentScreen.SetActive(true);
    }

    public void ClearOrder()
    {
        SetStatus(MonitorStatus.Order);
        emptyOrderTitle.SetActive(true);
        slot.gameObject.SetActive(false);

        currentBuyer = null;
        currentOrderData = null;
    }

    public Buyer currentBuyer { get; private set; }
    public OrderData? currentOrderData { get; private set; }

    public void SetOrder(Buyer buyer, OrderData orderData)
    {
        currentBuyer = buyer;
        currentOrderData = orderData;

        emptyOrderTitle.SetActive(false);
        slot.gameObject.SetActive(true);
        var itemData = itemsConfig.Get.Item(orderData.Items[0].ItemType);
        
        Debug.Log("SetOrder " + orderData.Items[0].ItemType);
        
        slot.SetVisual(itemData.MainData.IconSprite, itemData.mainData.Name);

        SetReceived(new());
        SetGiving(new());
    }

    public void SetOrderCost(DollarValue cost)
    {
        totalCost = cost;
        Debug.Log("SetOrderCost " + totalCost);
        
        string costText = TextUtility.FormatPrice(cost, true);
        foreach (var t in totalCostText) t.text = costText;
    }

    public void SetReceived(DollarValue received)
    {
        Debug.Log("Received " + received);
        Debug.Log("totalCost " + totalCost);
        
        // получено
        this.received = received;

        if (receivedCash) 
            receivedText.text = TextUtility.FormatPrice(received, false);
        else 
            receivedText.text = "-";

        // разница

        if (received.dollars < totalCost.dollars) 
            change = new();
        else 
            change = received - totalCost;

        if (receivedCash)
            changeText.text = TextUtility.FormatPrice(change, false);
        else 
            changeText.text = "-";
        UpdateGivingUI();
    }

    public void SetGiving(DollarValue currentGiving)
    {
        Debug.Log("SetGiving");
        this.currentGiving = currentGiving;

        UpdateGivingUI();
    }

    private void UpdateGivingUI()
    {
        Debug.Log("UpdateGivingUI");
        if (receivedCash)
        {
            givingText.text = TextUtility.FormatPrice(currentGiving, false);

            if (received.dollars <= 0 && received.cents <= 0) givingText.color = defaultTextColor;
            else if (currentGiving.dollars != change.dollars || currentGiving.cents != change.cents) givingText.color = redTextColor;
            else givingText.color = greenTextColor;
        }
        else
        {
            givingText.text = "-";
            givingText.color = defaultTextColor;
        }
    }
}

public enum MonitorStatus
{
    Order,
    CardPayment,
    CashPayment
}
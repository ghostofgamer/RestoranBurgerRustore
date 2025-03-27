using System;
using Configs;
using TheSTAR.Data;
using TheSTAR.Input;
using TheSTAR.Utility;
using TMPro;
using UnityEngine;
using Zenject;

namespace TheSTAR.GUI
{
    public class GameScreen : TutorialScreen
    {
        [SerializeField] private JoystickContainer joystickContainer;

        [Header("Interaction")] [SerializeField]
        private PointerButton mainInteractionButton;

        [SerializeField] private GameObject mainInteractionOutline;
        [SerializeField] private PointerButton placeButton;
        [SerializeField] private PointerButton openButton;
        [SerializeField] private PointerButton closeButton;
        [SerializeField] private PointerButton throwButton;

        [Space] [SerializeField] private DeliveryInfoUi deliveryInfoUi;
        [SerializeField] private GameObject hideWhileDeliveryObject;
        [SerializeField] private TaskUI taskUI;
        //[SerializeField] private GameObject firstAssemblyHint;

        [Space] [SerializeField] private GameObject interactionUiContainer;
        [SerializeField] private GameObject lookAroundTutorObject;
        [SerializeField] private GameObject moveTutorObject;

        [Space] [SerializeField] private PointerButton openCloseOrdersButton;
        [SerializeField] private TextMeshProUGUI journalTitle;

        [Space] [SerializeField] private AssemblyHintContainer assemblyHintContainer;
        [SerializeField] private Animator orderAlertAnim;
        //[SerializeField] private OrdersNote ordersNote;

        [Header("Income X2")] [SerializeField] private PointerButton increaseIncomeBtn;
        [SerializeField] private TextMeshProUGUI increaseIncomeEffectTimer;
        [SerializeField] private Animator incomeX2_animator;
        [SerializeField] private GameObject adTitleObject;

        public JoystickContainer JoystickContainer => joystickContainer;

        public event Action MainInteractionClickEvent;
        public event Action OnPlaceClickEvent;
        public event Action OnOpenClickEvent;
        public event Action OnCloseClickEvent;
        public event Action OnFillClickEvent;
        public event Action OnUseClickEvent;
        public event Action OnThrowClickEvent;

        private ConfigHelper<ItemsConfig> itemsConfig = new();

        private GameController game;
        private DataController data;
        private GuiController gui;
        private TutorialController tutorial;
        private Delivery delivery;
        private AdsManager ads;
        private CurrencyController currency;

        public PointerButton OpenButton => openButton;
        public PointerButton ThrowButton => throwButton;

        private bool ordersOpenned = false;

        [Inject]
        private void Construct(
            GameController game,
            DataController data,
            GuiController gui,
            TutorialController tutorial,
            Delivery delivery,
            AdsManager ads,
            CurrencyController currency)
        {
            this.game = game;
            this.data = data;
            this.gui = gui;
            this.tutorial = tutorial;
            this.delivery = delivery;
            this.ads = ads;
            this.currency = currency;
        }

        // Init for game
        public override void Init()
        {
            base.Init();

            joystickContainer.Init();
            joystickContainer.OnStartJoystickInteractEvent += OnStartMove;

            mainInteractionButton.Init(() => { MainInteractionClickEvent?.Invoke(); });
            placeButton.Init(() => OnPlaceClickEvent?.Invoke());
            openButton.Init(() =>
            {
                OnOpenClickEvent?.Invoke();
                /*
                if (tutorial.InTutorial &&
                    (tutorial.CurrentTutorialID == TutorialController.TutorID_PlaceCoffeeBeans || tutorial.CurrentTutorialID == TutorialController.TutorID_PlaceDessert))
                {
                    TriggerTutorial();
                }
                */
            });
            closeButton.Init(() =>
            {
                OnCloseClickEvent?.Invoke();
                /*
                if (tutorial.InTutorial &&
                    (tutorial.CurrentTutorialID == TutorialController.TutorID_PlaceCoffeeBeans || tutorial.CurrentTutorialID == TutorialController.TutorID_PlaceDessert))
                {
                    TriggerTutorial();
                }
                */
            });
            throwButton.Init(() => OnThrowClickEvent?.Invoke());

            deliveryInfoUi.Init(() => { delivery.TrySkipForAd(); }, () => { delivery.SkipForFree(); });

            taskUI.Init();

            openCloseOrdersButton.Init(() =>
            {
                ordersOpenned = !ordersOpenned;
                journalTitle.text = ordersOpenned ? "CLOSE" : "OPEN";

                //ordersNote.gameObject.SetActive(ordersOpenned);
            });

            //ordersNote.Init();

            delivery.OnStartDeliveryEvent += (duration, deliveryIndex) =>
            {
                deliveryInfoUi.gameObject.SetActive(true);
                hideWhileDeliveryObject.SetActive(false);
                deliveryInfoUi.SetForFree(deliveryIndex < 1);
            };

            delivery.OnDeliveryTickEvent += (timeSecondsLeft) =>
            {
                deliveryInfoUi.SetInfo(new GameTimeSpan(timeSecondsLeft));
            };

            delivery.OnCompleteDeliveryEvent += () =>
            {
                deliveryInfoUi.gameObject.SetActive(false);
                hideWhileDeliveryObject.SetActive(true);
                TriggerTutorial();
            };

            //ordersManager.OnOrderAcceptedEvent += ForceUpdateOrdersNote;
            //ordersManager.OnOrderChangeEvent += SoftUpdateOrdersNote;
            /*
            ordersManager.OnOrderCompletedEvent += (b) =>
            {
                //if (ordersNote.CurrentOrderData == null) return;

                if (ordersManager.ActiveOrders.Count == 0) ForceClearOrdersNote();
                else ordersNote.Set(ordersManager.ActiveOrders[0], true);
            };
            */

            tutorial.OnStartTutorialEvent += OnStartTutorial;
            tutorial.OnBreakTutorialEvent += () =>
            {
                taskUI.ClearTask();
                //firstAssemblyHint.SetActive(false);
            };
            tutorial.OnCompleteTutorialEvent += () =>
            {
                taskUI.ClearTask();
                //firstAssemblyHint.SetActive(false);
            };

            assemblyHintContainer.Init();

            increaseIncomeBtn.Init(() => { currency.TryGiveIncomeBonusForAds(); });

            currency.ShowIncomeX2OfferEvent += () =>
            {
                if (currency.IncomeX2) return;

                increaseIncomeBtn.SetInteractable(true);
                increaseIncomeBtn.gameObject.SetActive(true);
                adTitleObject.SetActive(true);
                incomeX2_animator.SetBool("Jump", true);
            };

            currency.HideIncomeX2OfferEvent += () =>
            {
                if (currency.IncomeX2) return;

                increaseIncomeBtn.gameObject.SetActive(false);
            };

            currency.StartIncomeEffectEvent += () =>
            {
                increaseIncomeBtn.gameObject.SetActive(true);
                increaseIncomeEffectTimer.gameObject.SetActive(true);
                increaseIncomeBtn.SetInteractable(false);
                adTitleObject.SetActive(false);
                incomeX2_animator.SetBool("Jump", false);
            };

            currency.IncomeX2Tick += (int timeLeftSeconds) =>
            {
                increaseIncomeEffectTimer.text = TextUtility.TimeToText(new GameTimeSpan(timeLeftSeconds));
            };

            currency.CompleteIncomeEffectEvent += () =>
            {
                increaseIncomeBtn.gameObject.SetActive(false);
                increaseIncomeEffectTimer.gameObject.SetActive(false);
            };
        }

        private void OnStartTutorial(TutorialType tutorialType, TutorialData tutorData)
        {
            if (tutorData.UseTaskPanel)
            {
                var defaultText = tutorData.TaskText;
                /*
                if (tutorialType == TutorialType.RemoveTrash)
                {
                    int current = game.World.InitialGarbageCount - game.World.StartGarbage.Count;
                    int max = game.World.InitialGarbageCount;
                    taskUI.SetTask(defaultText.Replace("{0}", $"{current}/{max}"));
                }
                else if (tutorialType == TutorialType.SetPrice)
                {
                    int current = 0;
                    if (data.gameData.tutorialData.coffeePriceChanged) current++;
                    if (data.gameData.tutorialData.donutPriceChanged) current++;
                    int max = 2;
                    taskUI.SetTask(defaultText.Replace("{0}", $"{current}/{max}"));
                }
                else if (tutorialType == TutorialType.CompleteOrders)
                {
                    int current = data.gameData.tutorialData.completedOrdersForTutorial;
                    int max = 2;
                    taskUI.SetTask(defaultText.Replace("{0}", $"{current}/{max}"));
                }
                */
                if (tutorialType == TutorialType.FirstDelivery)
                {
                    int current = 0;
                    int max = 1;
                    taskUI.SetTask(defaultText.Replace("{0}", $"{current}/{max}"));
                }
                else if (tutorialType == TutorialType.ServeTheQuests)
                {
                    int current = data.gameData.levelData.completedOrdersCount;
                    int max = 2;
                    taskUI.SetTask(defaultText.Replace("{0}", $"{current}/{max}"));
                }
                else if (tutorialType == TutorialType.AssemblyBurger)
                {
                    taskUI.SetTask(defaultText);
                    //firstAssemblyHint.SetActive(true);
                }
                else taskUI.SetTask(defaultText);
            }
            else taskUI.ClearTask();
        }

        /*
        private void ForceUpdateOrdersNote(ActiveOrderData activeOrderData)
        {
            ordersNote.Set(activeOrderData, true);

            ordersOpenned = true;
            journalTitle.text = ordersOpenned ? "CLOSE" : "OPEN";
            ordersNote.gameObject.SetActive(ordersOpenned);
        }

        private void ForceClearOrdersNote()
        {
            ordersNote.Set(null, true);

            ordersOpenned = false;
            journalTitle.text = ordersOpenned ? "CLOSE" : "OPEN";
            ordersNote.gameObject.SetActive(ordersOpenned);
        }

        private void SoftUpdateOrdersNote(ActiveOrderData activeOrderData)
        {
            ordersNote.Set(activeOrderData, false);
        }
        */

        protected override void OnShow()
        {
            base.OnShow();

            if (!data.gameData.commonData.gameRated && data.gameData.commonData.rateUsPlanned &&
                DateTime.Now >= data.gameData.commonData.nextRateUsPlan)
            {
                data.gameData.commonData.rateUsPlanned = false;
                gui.Show<RateUsScreen>();
            }

            UpdateInteractionButtons();

            TriggerTutorial();

            ads.TriggerInterstitial("game screen");
        }

        protected override void OnHide()
        {
            base.OnHide();
            joystickContainer.BreakInput();
            if (tutorial.InTutorial) tutorial.BreakTutorial();
        }

        public void OnStartDrag(Draggable draggable)
        {
            //Debug.Log("GameScreen: OnStartDrag");
            currentDraggable = draggable;
            UpdateInteractionButtons();

            //if (tutorial.InTutorial && tutorial.CurrentTutorialID == TutorialController.TutorID_MakeFirstCoffee_Final) TriggerTutorial();
        }

        public void OnTotalEndDrag()
        {
            currentDraggable = null;
            UpdateInteractionButtons();

            //if (tutorial.InTutorial && tutorial.CurrentTutorialID == TutorialController.TutorID_MakeFirstCoffee_Final) TriggerTutorial();
        }

        private Draggable currentDraggable; // предмет в руках игрока
        private TouchInteractive currentFocus; // предмет на который смотрит игрок

        public void SetPlayerFocus(TouchInteractive focus)
        {
            if (focus != null)
                currentFocus = focus;
            
            UpdateInteractionButtons();
        }

        public void UpdateInteractionButtons()
        {
            mainInteractionOutline.SetActive(currentFocus);
            DoUpdateInteractionUI();

            void DoUpdateInteractionUI()
            {
                openButton.gameObject.SetActive(false);
                closeButton.gameObject.SetActive(false);
                placeButton.gameObject.SetActive(false);
                throwButton.gameObject.SetActive(false);

                if (currentDraggable)
                {
                    throwButton.gameObject.SetActive(true);

                    var box = currentDraggable.GetComponent<Box>();
                    if (box)
                    {
                        if (box is BoxOpenClose boxOpenClose)
                        {
                            openButton.gameObject.SetActive(!boxOpenClose.IsOpen);
                            closeButton.gameObject.SetActive(boxOpenClose.IsOpen);

                            if (!currentFocus || !boxOpenClose.IsOpen)
                            {
                                placeButton.gameObject.SetActive(false);
                                return;
                            }
                        }
                        else if (!currentFocus)
                        {
                            placeButton.gameObject.SetActive(false);
                            return;
                        }

                        var boxDraggable = box.CurrentDraggable;
                        if (!boxDraggable) return;

                        var boxProduct = boxDraggable.GetComponent<Item>();
                        if (!boxProduct) return;

                        var boxProductSection = itemsConfig.Get.Item(boxProduct.ItemType).MainData.SectionType;

                        var draggableItem = currentDraggable.GetComponent<Item>();

                        var itemsHandler = currentFocus.GetComponent<ItemsHandler>();
                        if (draggableItem && itemsHandler && itemsHandler.HavePlace(draggableItem.ItemType) &&
                            itemsHandler.CanPlaceBySectionType(boxProductSection))
                        {
                            placeButton.gameObject.SetActive(true);
                        }

                        return;
                    }

                    if (currentFocus)
                    {
                        var coffeeMachine = currentFocus.GetComponent<OldCoffeeMachine>();

                        var product = currentDraggable.GetComponent<Item>();
                        if (product)
                        {
                            var productSection = itemsConfig.Get.Item(product.ItemType).MainData.SectionType;
                            var itemsHandler = currentFocus.GetComponent<ItemsHandler>();
                            if (itemsHandler && itemsHandler.HavePlace(product.ItemType) &&
                                itemsHandler.CanPlaceBySectionType(productSection))
                            {
                                placeButton.gameObject.SetActive(true);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private bool inWaitForLookAround = false;
        private bool inWaitForMove = false;

        public override void TriggerTutorial()
        {
            if (!IsShow) return;

            if (!tutorial.IsCompleted(TutorialType.LookAround))
            {
                tutorial.CompleteTutorial(TutorialType.LookAround);
                joystickContainer.gameObject.SetActive(false);
                SetUseTopUI(false);
                lookAroundTutorObject.SetActive(true);
                interactionUiContainer.SetActive(false);
                inWaitForLookAround = true;
                return;
            }

            if (!tutorial.IsCompleted(TutorialType.Move))
            {
                lookAroundTutorObject.SetActive(false);
                tutorial.CompleteTutorial(TutorialType.Move);
                joystickContainer.gameObject.SetActive(true);
                SetUseTopUI(false);
                moveTutorObject.SetActive(true);
                interactionUiContainer.SetActive(false);
                inWaitForMove = true;
                return;
            }

            game.TriggerTutorial();
        }

        public void OnStartLookAround()
        {
            if (inWaitForLookAround)
            {
                inWaitForLookAround = false;
                TriggerTutorial();
            }
        }

        public void OnStartMove()
        {
            if (inWaitForMove)
            {
                inWaitForMove = false;
                moveTutorObject.SetActive(false);
                SetUseTopUI(true);
                gui.FindUniversalElement<TopUiContainer>().gameObject.SetActive(true);
                interactionUiContainer.SetActive(true);
                TriggerTutorial();
            }
        }

        public void OnWrongOrderOnTray()
        {
            orderAlertAnim.SetTrigger("Show");
        }
    }
}

public enum UiMode
{
    Full,
    Minimum
}
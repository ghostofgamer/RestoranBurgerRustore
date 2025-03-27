using TheSTAR.Utility;
using UnityEngine;
using Zenject;
using TheSTAR.Data;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TheSTAR.GUI
{
    public class ComputerPayShopScreen : GuiScreen
    {
        [SerializeField] private PointerButton closeButton;
        [SerializeField] private PointerButton storeButton;
        [SerializeField] private PointerButton manageBtn;
        
        [Space] [SerializeField] private PointerButton placesButton_Active;
        [SerializeField] private PointerButton placesButton_Inactive;
        [SerializeField] private PointerButton zonesButton_Active;
        [SerializeField] private PointerButton zonesButton_Inactive;
        [SerializeField] private PointerButton equipmentButton_Active;
        [SerializeField] private PointerButton equipmentButton_Inactive;

        [Header("BuyerPlace")] [SerializeField]
        private BuyerPlaceSlot buyerPlaceSlotPrefab;

        [SerializeField] private ScrollRect buyerPlaceScrollRect;
        [SerializeField] private Transform buyerPlaceSlotsParent;

        [Header("Apparat")] [SerializeField] private ZoneSlot zoneSlotPrefab;
        [SerializeField] private ScrollRect zoneScrollRect;
        [SerializeField] private Transform zoneSlotsParent;

        [Header("Apparat")] [SerializeField] private ApparatSlot apparatSlotPrefab;
        [SerializeField] private ScrollRect apparatScrollRect;
        [SerializeField] private Transform apparatSlotsParent;

        [Space] [SerializeField] private GameObject comingSoonObject;

        private Dictionary<BuyerPlaceType, BuyerPlaceSlot[]> slotsByGroups;
        private List<ZoneSlot> zoneSlots;
        private List<ApparatSlot> apparatSlots;

        private ConfigHelper<GameConfig> gameConfig = new();

        private GameController game;
        private DataController data;
        private GuiController gui;
        private XpController xp;
        private TutorialController _tutorialController;
        private AdsManager ads;
        private CurrencyController currency;
        
        [Inject]
        private void Construct(GameController game, DataController data, GuiController gui, XpController xp,
            TutorialController tutorialController,AdsManager ads,CurrencyController currency)
        {
            this.game = game;
            this.data = data;
            this.gui = gui;
            this.xp = xp;
            this.ads = ads;
            this.currency = currency;
            _tutorialController = tutorialController;
        }

        public override void Init()
        {
            base.Init();

            closeButton.Init(() => { gui.ShowMainScreen(); });
            storeButton.Init(() => { gui.Show<ComputerStoreScreen>(); });
            manageBtn.Init(() => { gui.Show<ComputerManageScreen>(); });
        }

        protected override void OnShow()
        {
            base.OnShow();
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            Debug.Log("UpdatePayScreen");
        }

        private void OnBuySlotClick(BuyerPlaceType buyerPlaceType, int index)
        {
            game.TryBuyPlace(buyerPlaceType, index);
            UpdateVisual();
        }

        private void OnBuyExpandZoneClick(int index)
        {
            game.TryBuyExpandZone(index);
            UpdateVisual();
        }

        private void OnBuyApparatClick(int index)
        {
            if (index == 0) game.TryBuyCoffeeMachine();
            else if (index == 1) game.TryBuyDeepFlyer();
            else if (index == 2) game.TryBuySodaMachine();

            UpdateVisual();
        }

        public void AddMoneyFromADS()
        {
            ads.ShowRewarded("skip delivery", (success) =>
            {
                if (!success) return;
                currency.AddCurrencyFastMoney(CurrencyType.Soft, new(10, 0), true);
            });
        }
    }
}
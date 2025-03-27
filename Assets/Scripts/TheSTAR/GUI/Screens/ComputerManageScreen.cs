using TheSTAR.Utility;
using UnityEngine;
using Zenject;
using TheSTAR.Data;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TheSTAR.GUI
{
    public class ComputerManageScreen : GuiScreen
    {
        [SerializeField] private PointerButton closeButton;
        [SerializeField] private PointerButton storeButton;
        [SerializeField] private PointerButton shopBtn;
        
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

        private readonly BuyerPlaceType[] buyerPlaceTypesQueue = new BuyerPlaceType[]
        {
            BuyerPlaceType.SingleChair,
            BuyerPlaceType.DobbleChair,
            BuyerPlaceType.Sofa,
            BuyerPlaceType.SofaChair,
        };

        [Inject]
        private void Construct(GameController game, DataController data, GuiController gui, XpController xp,
            TutorialController tutorialController)
        {
            this.game = game;
            this.data = data;
            this.gui = gui;
            this.xp = xp;
            _tutorialController = tutorialController;
        }

        public override void Init()
        {
            base.Init();

            closeButton.Init(() => { gui.ShowMainScreen(); });
            shopBtn.Init(() => { gui.ShowForeachScreen<ComputerPayShopScreen>(); });
            storeButton.Init(() => { gui.Show<ComputerStoreScreen>(); });

            placesButton_Inactive.Init(() =>
            {
                Debug.Log("Places");
                placesButton_Inactive.gameObject.SetActive(false);
                placesButton_Active.gameObject.SetActive(true);
                zonesButton_Inactive.gameObject.SetActive(true);
                zonesButton_Active.gameObject.SetActive(false);
                equipmentButton_Active.gameObject.SetActive(false);
                equipmentButton_Inactive.gameObject.SetActive(true);

                buyerPlaceScrollRect.gameObject.SetActive(true);
                zoneScrollRect.gameObject.SetActive(false);
                apparatScrollRect.gameObject.SetActive(false);

                comingSoonObject.SetActive(false);
            });

            zonesButton_Inactive.Init(() =>
            {
                Debug.Log("Zones");
                placesButton_Inactive.gameObject.SetActive(true);
                placesButton_Active.gameObject.SetActive(false);
                zonesButton_Inactive.gameObject.SetActive(false);
                zonesButton_Active.gameObject.SetActive(true);
                equipmentButton_Active.gameObject.SetActive(false);
                equipmentButton_Inactive.gameObject.SetActive(true);

                buyerPlaceScrollRect.gameObject.SetActive(false);
                zoneScrollRect.gameObject.SetActive(true);
                apparatScrollRect.gameObject.SetActive(false);

                comingSoonObject.SetActive(false);
            });

            equipmentButton_Inactive.Init(() =>
            {
                Debug.Log("Equipment");
                placesButton_Inactive.gameObject.SetActive(true);
                placesButton_Active.gameObject.SetActive(false);
                zonesButton_Inactive.gameObject.SetActive(true);
                zonesButton_Active.gameObject.SetActive(false);
                equipmentButton_Active.gameObject.SetActive(true);
                equipmentButton_Inactive.gameObject.SetActive(false);

                buyerPlaceScrollRect.gameObject.SetActive(false);
                zoneScrollRect.gameObject.SetActive(false);
                apparatScrollRect.gameObject.SetActive(true);

                comingSoonObject.SetActive(false);
            });

            // buyer place slots
            slotsByGroups = new();
            foreach (var placeType in buyerPlaceTypesQueue)
            {
                var placeData = gameConfig.Get.BuyerPlaceCostData.Get(placeType);
                var length = placeData.Length;
                slotsByGroups.Add(placeType, new BuyerPlaceSlot[length]);

                for (int i = 0; i < length; i++)
                {
                    var slot = Instantiate(buyerPlaceSlotPrefab, buyerPlaceSlotsParent);
                    slot.SetActiveHandValue(false);
                    slot.Init(i, placeType, placeData[i].DisplayName, gameConfig.Get.BuyerPlaceIcons.Get(placeType),
                        placeData[i].Cost, OnBuySlotClick, buyerPlaceScrollRect, placeData[i].PlacesCount);
                    slotsByGroups[placeType][i] = slot;
                }
            }

            // zones
            zoneSlots = new();
            for (int i = 0; i < gameConfig.Get.ExpandZonesData.Length; i++)
            {
                var expandData = gameConfig.Get.ExpandZonesData[i];
                var slot = Instantiate(zoneSlotPrefab, zoneSlotsParent);
                slot.Init(i, expandData.DisplayName, expandData.Cost, OnBuyExpandZoneClick, zoneScrollRect);
                zoneSlots.Add(slot);
            }

            // apparat slots
            apparatSlots = new();

            // coffee
            var coffeeSlot = Instantiate(apparatSlotPrefab, apparatSlotsParent);
            coffeeSlot.Init(
                0,
                gameConfig.Get.CoffeeMachineData.DisplayName,
                gameConfig.Get.CoffeeMachineData.Icon,
                gameConfig.Get.CoffeeMachineData.Cost,
                OnBuyApparatClick,
                buyerPlaceScrollRect);
            apparatSlots.Add(coffeeSlot);

            // deep fryer
            var deepFryerSlot = Instantiate(apparatSlotPrefab, apparatSlotsParent);
            deepFryerSlot.SetValueSoonPanel(true);

            deepFryerSlot.Init(
                1,
                gameConfig.Get.DeepFryerMachineData.DisplayName,
                gameConfig.Get.DeepFryerMachineData.Icon,
                gameConfig.Get.DeepFryerMachineData.Cost,
                OnBuyApparatClick,
                buyerPlaceScrollRect);
            apparatSlots.Add(deepFryerSlot);

            // soda
            var sodaSlot = Instantiate(apparatSlotPrefab, apparatSlotsParent);
            sodaSlot.Init(
                2,
                gameConfig.Get.SodaMachineData.DisplayName,
                gameConfig.Get.SodaMachineData.Icon,
                gameConfig.Get.SodaMachineData.Cost,
                OnBuyApparatClick,
                buyerPlaceScrollRect);
            apparatSlots.Add(sodaSlot);
        }

        protected override void OnShow()
        {
            base.OnShow();
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            Debug.Log("UpdateManageScreen");
            bool owned;
            int neededLevel;
            bool availableByLevel;
            int neededExpandCount;
            bool availableByExpand;

            // buyer places
            foreach (var placeType in buyerPlaceTypesQueue)
            {
                var slotGroup = slotsByGroups[placeType];

                Debug.Log("колличество слотов " + slotGroup.Length);
                Debug.Log("placeType " + placeType);


                for (int i = 0; i < slotGroup.Length; i++)
                {
                    owned = data.gameData.levelData.activeBuyerPlaces[placeType][i];
                    slotGroup[i].SetVisual(owned);

                    if (i == 2 && _tutorialController.IsCompleted(TutorialType.ClearTables) &&
                        !_tutorialController.IsCompleted(TutorialType.BuyChair) &&
                        placeType == BuyerPlaceType.SingleChair)
                        slotGroup[i].SetActiveHandValue(true);
                    else
                        slotGroup[i].SetActiveHandValue(false);

                    neededLevel = gameConfig.Get.BuyerPlaceCostData.Get(placeType)[i].NeededLevel;
                    availableByLevel = xp.CurrentLevel >= neededLevel;
                    if (availableByLevel) slotGroup[i].SetUnlockedByLevel();
                    else slotGroup[i].SetLockedByLevel(neededLevel);

                    neededExpandCount = gameConfig.Get.BuyerPlaceCostData.Get(placeType)[i].NeededExpandsCount;
                    availableByExpand = data.gameData.levelData.purchasedExpandsCount >= neededExpandCount;
                    if (availableByExpand) slotGroup[i].SetUnlockedByExpand();
                    else slotGroup[i].SetLockedByExpand(neededExpandCount);
                }
            }

            // zones
            for (int i = 0; i < zoneSlots.Count; i++)
            {
                owned = data.gameData.levelData.purchasedExpandsCount > i;
                zoneSlots[i].SetVisual(owned);
                neededLevel = gameConfig.Get.ExpandZonesData[i].NeededLevel;
                availableByLevel = xp.CurrentLevel >= neededLevel;
                if (availableByLevel) zoneSlots[i].SetUnlockedByLevel();
                else zoneSlots[i].SetLockedByLevel(neededLevel);

                neededExpandCount = gameConfig.Get.ExpandZonesData[i].NeededExpandCount;
                availableByExpand = data.gameData.levelData.purchasedExpandsCount >= neededExpandCount;
                if (availableByExpand) zoneSlots[i].SetUnlockedByExpand();
                else zoneSlots[i].SetLockedByExpand(neededExpandCount);
            }

            // apparats
            var slot = apparatSlots[0];
            owned = data.gameData.levelData.coffeeMachinePurchased;
            slot.SetVisual(owned);
            neededLevel = gameConfig.Get.CoffeeMachineData.NeededLevel;
            availableByLevel = xp.CurrentLevel >= neededLevel;
            if (availableByLevel) slot.SetUnlocked();
            else slot.SetLocked(neededLevel);

            slot = apparatSlots[1];
            owned = data.gameData.levelData.deepFryerPurchased;
            slot.SetVisual(owned);
            neededLevel = gameConfig.Get.DeepFryerMachineData.NeededLevel;
            availableByLevel = xp.CurrentLevel >= neededLevel;
            if (availableByLevel) slot.SetUnlocked();
            else slot.SetLocked(neededLevel);

            slot = apparatSlots[2];
            owned = data.gameData.levelData.sodaMachinePurchased;
            slot.SetVisual(owned);
            neededLevel = gameConfig.Get.SodaMachineData.NeededLevel;
            availableByLevel = xp.CurrentLevel >= neededLevel;
            if (availableByLevel) slot.SetUnlocked();
            else slot.SetLocked(neededLevel);
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
    }
}
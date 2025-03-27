using System;
using DailyRewardContent;
using DecorationContent;
using UnityEngine;
using TheSTAR.Data;
using Zenject;
using TheSTAR.Sound;
using UnityEngine.UI;

namespace TheSTAR.GUI
{
    public class DailyRewardScreen : GuiScreen
    {
        private const string LastClaimDateKey = "LastClaimDate";
        private const string CurrentDayIndexKey = "CurrentDayIndex";
        
        [SerializeField] private DailyReward _dailyReward;
        [SerializeField] private PointerButton closeButton;
        [SerializeField] private Button[] _dailyButtons;
        [SerializeField] private GameObject[] _checkMarks;
        [SerializeField] private Sprite _rewardClaimedSprite;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _currentDaySprite;
        [SerializeField] private Sprite _lastDayDefaultSprite;
        [SerializeField] private GameObject _superPrizeDecoration;
        [SerializeField] private GameObject _superPrizeMoney;

        public Button[] dayButtons;
        public Button claimButton;
        private int currentDayIndex;
        private DecorationSystem _decorationSystem;
        private bool rewardClaimedToday;
        private AdsManager ads;
        private GameController game;
        private GuiController gui;
        private DataController data;
        private CurrencyController currency;
        private XpController xp;
        private SoundController sounds;
        private GuiScreen from;
        private string filePath;
        private GameWorld _gameWorld;
        public bool isTesting;
        public int testDaysOffset;

        [Inject]
        private void Construct(GameController game, DataController data, GuiController gui, CurrencyController currency,
            XpController xp, SoundController sounds, AdsManager ads)
        {
            this.game = game;
            this.data = data;
            this.gui = gui;
            this.currency = currency;
            this.xp = xp;
            this.sounds = sounds;
            this.ads = ads;
            /*_gameWorld = world;*/
        }

        private void Start()
        {
            _decorationSystem = gui.GameWorld.DecorationSystem;
            InitializeDailyRewards();
        }

        public void Init(GuiScreen from, bool useMainMenuFon)
        {
            this.from = from;
            SetUseMainMenuFon(useMainMenuFon);
        }

        public override void Init()
        {
            base.Init();
            closeButton.Init(() => { gameObject.SetActive(false); });
        }

        protected override void OnShow()
        {
            base.OnShow();
        }

        private void InitializeDailyRewards()
        {
            DateTime lastClaimDate = GetLastClaimDate();
            // DateTime currentDate = DateTime.Now;
            DateTime currentDate = isTesting ? DateTime.Now.AddDays(testDaysOffset) : DateTime.Now;

            if (lastClaimDate == DateTime.MinValue )
            {
                currentDayIndex = 0;
                rewardClaimedToday = false;
            }
            else if ((currentDate - lastClaimDate).TotalDays >= 2)
            {
                currentDayIndex = 0;
                rewardClaimedToday = false;
                PlayerPrefs.SetInt(CurrentDayIndexKey, -1);
            }
            else if ((currentDate - lastClaimDate).TotalDays >= 1)
            {
                currentDayIndex = PlayerPrefs.GetInt(CurrentDayIndexKey, 0);

                if (currentDayIndex >= dayButtons.Length - 1)
                    currentDayIndex = 0;
                else
                    currentDayIndex++;

                rewardClaimedToday = false;
            }
            else
            {
                currentDayIndex = PlayerPrefs.GetInt(CurrentDayIndexKey, 0);
                rewardClaimedToday = true;
            }

            UpdateUI();
        }

        private DateTime GetLastClaimDate()
        {
            string lastClaimDateString = PlayerPrefs.GetString(LastClaimDateKey, string.Empty);

            if (string.IsNullOrEmpty(lastClaimDateString))
            {
                return DateTime.MinValue;
            }

            return DateTime.Parse(lastClaimDateString);
        }

        private void UpdateUI()
        {
            var currentIndex = PlayerPrefs.GetInt(CurrentDayIndexKey, -1);

            SetValueSuperPrizeIcon(
                _decorationSystem.GetActivationValueDecoration(_decorationSystem.CurrentDailyRewardDecoration));
            
            for (int i = 0; i < dayButtons.Length; i++)
            {
                if (i < currentDayIndex)
                {
                    dayButtons[i].image.sprite = _rewardClaimedSprite;
                    _checkMarks[i].SetActive(true);
                }
                else if (i == currentDayIndex)
                {
                    dayButtons[i].image.sprite = currentIndex == i ? _rewardClaimedSprite : _currentDaySprite;
                    _checkMarks[i].SetActive(currentIndex == i);
                }
                else
                {
                    dayButtons[i].image.sprite = i == dayButtons.Length - 1 ? _lastDayDefaultSprite : _defaultSprite;
                }

                // dayButtons[i].interactable = (i == currentDayIndex && !rewardClaimedToday);
            }

            claimButton.interactable = (currentDayIndex < dayButtons.Length && !rewardClaimedToday);
        }

        private void SetValueSuperPrizeIcon(bool value)
        {
            _superPrizeDecoration.SetActive(value);
            _superPrizeMoney.SetActive(!value);
        }

        public void ClaimReward()
        {
            DateTime lastClaimDate = GetLastClaimDate();
            // DateTime currentDate = DateTime.Now;
            DateTime currentDate = isTesting ? DateTime.Now.AddDays(testDaysOffset) : DateTime.Now;
            sounds.Play(SoundType.ButtonClickWet);
            sounds.Play(SoundType.DailyReward);
            
            if ((currentDate - lastClaimDate).TotalDays >= 1)
            {
                if (currentDayIndex < dayButtons.Length)
                {
                    Debug.Log("Награда за день " + (currentDayIndex + 1) + " получена!");
                    PlayerPrefs.SetString(LastClaimDateKey, DateTime.Now.ToString());
                    PlayerPrefs.SetInt(CurrentDayIndexKey, currentDayIndex);
                    rewardClaimedToday = true;
                    UpdateUI();
                    _dailyReward.Claim(currentDayIndex);
                }
            }
            else
            {
                Debug.Log("Вы уже получили награду за сегодня. Попробуйте завтра.");
            }
        }

        protected override void OnHide()
        {
            /*base.OnHide();
            data.Save(DataSectionType.Settings);*/
        }
    }
}
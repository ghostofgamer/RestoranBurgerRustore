using System.IO;
using CoppraGames;
using DailyTimerContent;
using FortuneContent;
using UnityEngine;
using TheSTAR.Data;
using Zenject;
using TMPro;
using SoundController = TheSTAR.Sound.SoundController;

namespace TheSTAR.GUI
{
    public class FortuneScreen : GuiScreen
    {
        [SerializeField] private PointerButton closeButton;
        [SerializeField] private TMP_Text _spinValueText;
        [SerializeField] private SpinWheelController _spinWheelController;
        [SerializeField] private DailyTimerFortune _dailyTimerFortune;
        [SerializeField] private DailyTimerFortune _dailyTimerADSFortune;
        [SerializeField] private GameObject[] _spinButtons;
        [SerializeField] private GameObject _spinFreeButton;
        [SerializeField] private GameObject _spinValueButton;
        [SerializeField] private GameObject _spinADSButton;
        [SerializeField] private GameObject _backWinText;
        [SerializeField] private TMP_Text _prizeText;
        

        private int _currentValueSpin = 1;
        private AdsManager ads;
        private GameController game;
        private GuiController gui;
        private DataController data;
        private CurrencyController currency;
        private XpController xp;
        private SoundController sounds;
        private GuiScreen from;
        private string filePath;

        private Prize[] prizeMap = new Prize[]
        {
            new Prize { Type = PrizesFortune.Money, Value = 10 },
            new Prize { Type = PrizesFortune.Spin, Value = 10 },
            new Prize { Type = PrizesFortune.Money, Value = 50 },
            new Prize { Type = PrizesFortune.XP, Value = 20 },
            new Prize { Type = PrizesFortune.Spin, Value = 3 },
            new Prize { Type = PrizesFortune.XP, Value = 600 },
            new Prize { Type = PrizesFortune.Money, Value = 1000 },
            new Prize { Type = PrizesFortune.XP, Value = 75 },
            new Prize { Type = PrizesFortune.Spin, Value = 2 },
            new Prize { Type = PrizesFortune.XP, Value = 1500 },
        };

        [Inject]
        private void Construct(GameController game, DataController data, GuiController gui, CurrencyController currency,
            XpController xp, SoundController sounds,AdsManager ads)
        {
            this.game = game;
            this.data = data;
            this.gui = gui;
            this.currency = currency;
            this.xp = xp;
            this.sounds = sounds;
            this.ads = ads;
        }

        private void OnEnable()
        {
            _spinWheelController.PrizeCompleted += SetPrize;
            _dailyTimerFortune.TimeOverCompleted += ActivateFreeSpinButton;
            _dailyTimerFortune.TimeNotOverCompleted += ActiveOtherSpinButton;
            _dailyTimerADSFortune.TimeOverCompleted += ActivateADSSpinButton;
            _dailyTimerADSFortune.TimeNotOverCompleted += DeactivateAdsSpinButton;
        }

        private void OnDisable()
        {
            _spinWheelController.PrizeCompleted -= SetPrize;
            _dailyTimerFortune.TimeOverCompleted -= ActivateFreeSpinButton;
            _dailyTimerFortune.TimeNotOverCompleted -= ActiveOtherSpinButton;
            _dailyTimerADSFortune.TimeOverCompleted -= ActivateADSSpinButton;
            _dailyTimerADSFortune.TimeNotOverCompleted -=DeactivateAdsSpinButton;
        }

        private void Start()
        {
            filePath = Path.Combine(Application.persistentDataPath, "spinData.json");
            LoadSpinData();
            _spinValueText.text = _currentValueSpin.ToString();
        }

        public void Init(GuiScreen from, bool useMainMenuFon)
        {
            this.from = from;
            SetUseMainMenuFon(useMainMenuFon);
        }

        public override void Init()
        {
            base.Init();
            // closeButton.Init(() => { gui.ShowMainScreen(); });
            closeButton.Init(() => { gameObject.SetActive(false); });
        }

        protected override void OnShow()
        {
            base.OnShow();
            _spinValueText.text = _currentValueSpin.ToString();
        }

        public void SpinWheel()
        {
            if (_currentValueSpin <= 0)
                return;

            if (_spinWheelController.IsStarted)
                return;

            _currentValueSpin--;
            SaveSpinData();
            Spin();
            _spinValueText.text = _currentValueSpin.ToString();
        }

        public void SpinFree()
        {
            if (_spinWheelController.IsStarted)
                return;
            
            _dailyTimerFortune.StartButtonClick();
            Spin();
        }

        private void Spin()
        {
            _backWinText.SetActive(false);
            // _wellFortune.Rotate();
            _spinWheelController.TurnWheel();
            _dailyTimerFortune.UpdateInfo();
        }

        public void SpinADS()
        {
            if (_spinWheelController.IsStarted)
                return;
            
            _dailyTimerADSFortune.StartButtonClick();
            
            ads.ShowRewarded("skip delivery", (success) =>
            {
                if (!success) return;
                Spin();
            });
        }

        private void ActivateFreeSpinButton()
        {
            Debug.Log("ActivateFreeSpinButton");
            DeactivationButtons();
            _spinFreeButton.SetActive(true);
        }

        private void ActiveOtherSpinButton()
        {
            Debug.Log("ActiveOtherSpinButton");
            DeactivationButtons();

            if (_currentValueSpin <= 0)
                _dailyTimerADSFortune.UpdateInfo();
            else
                _spinValueButton.SetActive(true);
        }

        private void ActivateADSSpinButton()
        {
            Debug.Log("ActivateADSSpinButton");
            if (_spinFreeButton.activeSelf)
                return;
        
            if (_currentValueSpin > 0)
                return;
          
            _spinADSButton.SetActive(true);
        }

        private void DeactivateAdsSpinButton()
        {
            _spinADSButton.SetActive(false);
        }

        private void DeactivationButtons()
        {
            foreach (var button in _spinButtons)
                button.SetActive(false);
        }

        private void SetPrize(int index)
        {
            Debug.Log(index);
            
            Prize prize = prizeMap[index];
            _backWinText.SetActive(true);
            _prizeText.text = $"You Win: + {prize.Value}  {prize.Type.ToString()}";
            
            switch (prize.Type)
            {
                case PrizesFortune.Money:
                    currency.AddCurrencyFastMoney(CurrencyType.Soft, new(prize.Value, 0), true);
                    break;

                case PrizesFortune.XP:
                    xp.AddXp(prize.Value);
                    break;

                case PrizesFortune.Spin:
                    _currentValueSpin+=prize.Value;
                    _dailyTimerFortune.UpdateInfo();
                    _spinValueText.text = _currentValueSpin.ToString();
                    // SaveSpinData();
                    break;
            }
        }

        protected override void OnHide()
        {
            /*base.OnHide();
            data.Save(DataSectionType.Settings);*/
        }

        private void SaveSpinData()
        {
            SpinData data = new SpinData { currentValueSpin = _currentValueSpin };
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);
        }

        private void LoadSpinData()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SpinData data = JsonUtility.FromJson<SpinData>(json);
                _currentValueSpin = data.currentValueSpin;
            }
        }
    }

    [System.Serializable]
    public class SpinData
    {
        public int currentValueSpin;
    }

    public struct Prize
    {
        public PrizesFortune Type;
        public int Value;
    }
}
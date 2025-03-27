using System;
using System.IO;
using TheSTAR.GUI;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ReputationContent
{
    public class Reputation : MonoBehaviour
    {
        private const string SaveFileName = "reputation.json";

        [SerializeField] private TMP_Text _reputationText;
        [SerializeField] private int _maxReputation = 100;
        [SerializeField] private Slider _reputationSlider;
        [SerializeField] private Image[] _stars;
        [SerializeField] private FlyValue _rateFly;

        private int _currentReputation;
        private FastFood _fastFood;
        private bool isSubscribed = false;
        private GameWorld _gameWorld;
        private GuiController _guiController;
        private GameWorldInteraction _gameWorldInteraction;
        private int _starsRestaurant;
        private int _minReputationChangeValue = 1;

        private bool _isDecreasing = false;

        public event Action<int> NewReputationValueReceived;
        
        public int CurrentReputation => _currentReputation;

        public int StarsRestaurant => _starsRestaurant;
        

        private void OnEnable()
        {
            if (_isDecreasing)
            {
                DecreaseReputation();
                _isDecreasing = false;
            }
        }

        private void Start()
        {
            LoadReputation();
        }

        [Inject]
        private void Construct(
            GuiController gui)
        {
            _guiController = gui;
        }

        public void Init(FastFood fastFood)
        {
            _fastFood = fastFood;
            _fastFood.OrderCompleted += IncreaseReputation;
            _fastFood.OverpaymentMaded += EnableDecrease;
        }

        public void InitGameWorldInteraction(GameWorldInteraction gameWorldInteraction)
        {
            _gameWorldInteraction = gameWorldInteraction;
            _gameWorldInteraction.NotTable += DecreaseReputation;
        }

        public void IncreaseReputation()
        {
            Debug.Log("увеличить репутацию");
            _currentReputation = Mathf.Clamp(_currentReputation + _minReputationChangeValue, 0, _maxReputation);
            _rateFly.Show(_minReputationChangeValue);
            UpdateReputationUI();
            SaveReputation();
            NewReputationValueReceived?.Invoke(_minReputationChangeValue);
        }

        public void DecreaseReputation()
        {
            _currentReputation = Mathf.Clamp(_currentReputation - _minReputationChangeValue, 0, _maxReputation);
            _rateFly.Show(-_minReputationChangeValue);
            UpdateReputationUI();
            Debug.Log("уменьшить репутацию");
            SaveReputation();
            NewReputationValueReceived?.Invoke(-_minReputationChangeValue);
        }

        private void EnableDecrease()
        {
            _isDecreasing = true;
        }

        public void ClientView(int waitingRate, int pollutionRate)
        {
            int result = waitingRate + pollutionRate;
            _currentReputation = Mathf.Clamp(_currentReputation + result, 0, _maxReputation);
            _rateFly.Show(result);
            UpdateReputationUI();
            Debug.Log($"уровень ожидания клиента {waitingRate}\nа грязь стола  {pollutionRate}");
            Debug.Log($"НОВЫЕ ДАННЫЕ РЕПУТАЦИИ {result}");
            SaveReputation();
            NewReputationValueReceived?.Invoke(result);
        }

        private void UpdateReputationUI()
        {
            _reputationSlider.value = (float)_currentReputation / _maxReputation;
            Debug.Log($"урвоень нашей репутации {_currentReputation}");
            // _starsRestaurant = Mathf.FloorToInt((float)_currentReputation / _maxReputation * _stars.Length);

            _starsRestaurant = _currentReputation switch
            {
                >= 1 and <= 5 => 1,
                >= 6 and <= 15 => 2,
                >= 16 and <= 50 => 3,
                >= 51 and <= 80 => 4,
                >= 81 and <= 100 => 5,
                _ => 0
            };

            for (int i = 0; i < _stars.Length; i++)
                _stars[i].color = i < _starsRestaurant ? Color.white : Color.black;
        }

        public void SaveReputation()
        {
            ReputationData data = new ReputationData
            {
                ReputationValue = _currentReputation,
                // StarsCount = StarsCount
            };

            string json = JsonUtility.ToJson(data);
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            File.WriteAllText(path, json);
        
            Debug.Log("Сохранили " + _currentReputation);
        }

        public void LoadReputation()
        {
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Debug.Log("Loaded JSON: " + json); // Вывод содержимого файла для проверки

                try
                {
                    ReputationData data = JsonUtility.FromJson<ReputationData>(json);
                    _currentReputation = data.ReputationValue;
                    // _starsCount = data.StarsCount;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to deserialize JSON: " + e.Message);
                    _currentReputation = 0;
                    // _starsCount = 0;
                }
            }
            else
            {
                _currentReputation = 0;
                // _starsCount = 0;
            }

            Debug.Log("Загрузили " + _currentReputation);
            UpdateReputationUI();
        }
    }
}
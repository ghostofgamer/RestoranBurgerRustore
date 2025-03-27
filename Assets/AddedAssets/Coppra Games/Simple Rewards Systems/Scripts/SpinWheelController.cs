using System;
using System.Collections;
using TheSTAR.Sound;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace CoppraGames
{
    public class SpinWheelController : MonoBehaviour
    {
        [System.Serializable]
        public class RewardItem
        {
            public Sprite icon;
            public int count;
        }

        [SerializeField] private GameObject _closeButton;
        
        public float[] rewardProbabilities;
        public RewardItem[] rewards;
        public RewardItemComponent[] rewardItemComponents;
        public Transform Wheel;
        public AnimationCurve Curve;
        public Collider2D SpinWheelArrowCollider;

        private bool _isStarted;
        private float _startAngle;
        private float _endAngle;
        private int _randomRewardIndex = 0;
        private float _currentRotationTime;
        private float _maxRotationTime;
        private SoundController sounds;
        
        public event Action<int> PrizeCompleted;

        public bool IsStarted => _isStarted;

        [Inject]
        private void Consruct(SoundController sounds)
        {
            this.sounds = sounds;
        }
        
        public void Init()
        {
            ApplyValues();
        }

        public void TurnWheel()
        {
            if (_isStarted)
                return;
            
            _closeButton.SetActive(false);
            sounds.Play(SoundType.ButtonClickWet);
            SpinWheelArrowCollider.gameObject.SetActive(true);
            SpinWheelArrowCollider.enabled = true;
            _isStarted = true;
            _startAngle = Wheel.localEulerAngles.z;
      
            int totalSlots = rewards.Length;
            // _randomRewardIndex = Random.Range(0, totalSlots);
            
            _randomRewardIndex = GetRandomRewardIndex();
            int rotationCount = Random.Range(10, 15);
            float angle = _randomRewardIndex * 36;
            
            // _endAngle = -(rotationCount * 360 + _randomRewardIndex * 360 / totalSlots);
            _endAngle = (rotationCount * 360 + _randomRewardIndex * (360f / totalSlots));
           
            _currentRotationTime = 0.0f;
            _maxRotationTime = Random.Range(5.0f, 9.0f);

        }

        void Update()
        {
            if (_isStarted)
            {
                float t = _currentRotationTime / _maxRotationTime;
                t = Curve.Evaluate(t);

                //t = t * t * t * (t * (a * t - b) + c);

                float angle = Mathf.Lerp(_startAngle, _endAngle, t);
        
                Wheel.eulerAngles = new Vector3(0, 0, angle);
                
                if (angle >= _endAngle)
                {
                    _isStarted = false;
                    Debug.Log("RandomRewardIndex " + _randomRewardIndex);
                    PrizeCompleted?.Invoke(_randomRewardIndex);
                    SettleWheel();
                    _closeButton.SetActive(true);
                }

                _currentRotationTime += Time.deltaTime;
            }
        }

        public int GetRandomRewardIndex()
        {
            // Проверяем, что сумма вероятностей равна 1
            float totalProbability = 0f;
            
            foreach (float probability in rewardProbabilities)
            {
                totalProbability += probability;
            }
            
            if (Mathf.Abs(totalProbability - 1f) > 0.0001f)
            {
                Debug.LogError("Сумма вероятностей должна быть равна 1.");
                return -1;
            }

            // Генерируем случайное число от 0 до 1
            float randomValue = Random.value;

            // Выбираем индекс на основе вероятностей
            float cumulativeProbability = 0f;
            
            for (int i = 0; i < rewardProbabilities.Length; i++)
            {
                cumulativeProbability += rewardProbabilities[i];
                if (randomValue < cumulativeProbability)
                {
                    return i;
                }
            }

            // Если что-то пошло не так, возвращаем -1
            return -1;
        }
        
        void SettleWheel()
        {
            SpinWheelArrowCollider.enabled = false;
            // ShowResult(_randomRewardIndex);
        }

        public void ApplyValues()
        {
            int index = 0;
            
            foreach (var r in rewards)
            {
                if (rewardItemComponents.Length > index)
                    rewardItemComponents[index].SetData(r);

                index++;
            }
        }
        
        public void OnTriggerNeedle()
        {
            if (_isStarted)
                sounds.Play(SoundType.WheelTick);
                // SoundController.instance.PlaySoundEffect("spin_tuk", false, 1);
        }

        public void Close()
        {
            Main.instance.ShowSpinWheelWindow(false);
        }
    }
}

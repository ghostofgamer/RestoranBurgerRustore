using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FortuneContent
{
    public class WellFortune : MonoBehaviour
    {
        [SerializeField] private float _minRotatePower;
        [SerializeField] private float _maxRotatePower;
        [SerializeField] private float _minStopPower;
        [SerializeField] private float _maxStopPower;
        [SerializeField] private GameObject _closeBtn;

        private Rigidbody2D _rbody;
        private int _inRotate;
        private float _t;
        private float _currentRotatePower;
        private float _currentStopPower;
        private bool _isWork;

        public event Action<int> PrizeCompleted;
         
        public int InRotate => _inRotate;

        private void Start()
        {
            _rbody = GetComponent<Rigidbody2D>();
        }

        private PrizesFortune[] prizeMap = new PrizesFortune[]
        {
            PrizesFortune.Money,
            PrizesFortune.Money,
            PrizesFortune.Money,
            PrizesFortune.XP,
            PrizesFortune.XP,
            PrizesFortune.Spin,
            PrizesFortune.Spin,
            PrizesFortune.Spin
        };
        
        private void Update()
        {
            if (!_isWork)
                return;
            
            if (_rbody.angularVelocity > 0)
            {
                _rbody.angularVelocity -= _currentStopPower * Time.deltaTime;
                _rbody.angularVelocity = Mathf.Clamp(_rbody.angularVelocity, 0, 1440);
            }

            if (_rbody.angularVelocity == 0 && _inRotate == 1)
            {
                _t += 1 * Time.deltaTime;

                if (_t >= 0.5f)
                {
                    GetReward();
                    _inRotate = 0;
                    _t = 0;
                    _closeBtn.SetActive(true);
                    _isWork = false;
                }
            }
        }

        public void Rotate()
        {
            if (_inRotate == 0)
            {
                _isWork = true;
                _closeBtn.SetActive(false);
                _currentRotatePower = Random.Range(_minRotatePower, _maxRotatePower);
                _currentStopPower = Random.Range(_minStopPower, _maxStopPower);
                _rbody.AddTorque(_currentRotatePower);
                _inRotate = 1;
            }
        }

        private void GetReward()
        {
            float rot = transform.eulerAngles.z;

            switch (rot)
            {
                case > 0 - 23 and <= 45 - 22:
                    Win(1);
                    break;
                case > 45 - 22 and <= 90 - 22:
                    Win(2);
                    break;
                case > 90 - 22 and <= 135 - 22:
                    Win(3);
                    break;
                case > 135 - 22 and <= 180 - 22:
                    Win(4);
                    break;
                case > 180 - 22 and <= 225 - 22:
                    Win(5);
                    break;
                case > 225 - 22 and <= 270 - 22:
                    Win(6);
                    break;
                case > 270 - 22 and <= 315 - 22:
                    Win(7);
                    break;
                case > 315 - 22 and <= 360 - 22:
                    Win(8);
                    break;
            }
        }

        private void Win(int index)
        {
            Debug.Log("приз " + index);
            PrizeCompleted?.Invoke(index);
        }
    }
}
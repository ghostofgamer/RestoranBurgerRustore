using System;
using DayChangeContent.Counters;
using TMPro;
using UnityEngine;

namespace DayChangeContent
{
    public class DayNightCycle : MonoBehaviour
    {
        private const string IS_DAY_KEY = "IsDay";
        private const string StatisticDay = "StatisticDay";
        private const string IS_NIGHT_KEY = "IsNight";
        private const string TIME_OF_DAY_KEY = "TimeOfDay";
        private const float START_HOUR = 9f;
        private const float END_HOUR = 22f;

        [SerializeField] private Material skyboxMaterial;
        [SerializeField] private float dayDuration;
        [SerializeField] private float _nightDuration;
        [SerializeField] private Color dayColor;
        [SerializeField] private Color nightColor;
        [SerializeField] private Color _dayAmbientColor;
        [SerializeField] private Color _nightAmbientColor;
        [SerializeField] private Color _dayEquatorColor;
        [SerializeField] private Color _nightEquatorColor;
        [SerializeField] private Color _dayLightColor;
        [SerializeField] private Color _nightLightColor;
        [SerializeField] private BuyersCounter _buyersCounter;

        private TMP_Text _timeText;
        public float minExposure = 0.5f;
        public float maxExposure = 1.65f;
        private float timeOfDay;
        private bool _isDay;
        private bool _isNight;
        private Light sceneLight;
        private bool _isStatisticDayOpened;
        private bool _isOpen;

        public event Action DayOverCompleted;

        public event Action NewDayStarted;

        public event Action<bool> SetNightLighting;

        private void Start()
        {
            _isDay = PlayerPrefs.GetInt(IS_DAY_KEY, 1) == 1;
            Debug.Log("_isDay" + _isDay);
            // _isNight = PlayerPrefs.GetInt(IS_NIGHT_KEY, 0) == 1;
            _isNight = !_isDay;

            timeOfDay = PlayerPrefs.GetFloat(TIME_OF_DAY_KEY, 0f);

            if (timeOfDay == 0f)
                timeOfDay = (START_HOUR - 9f) / (END_HOUR - 9f);

            RenderSettings.skybox = skyboxMaterial;
            GameObject lightObject = GameObject.Find("Light");

            if (lightObject != null)
                sceneLight = lightObject.GetComponent<Light>();
            else
                Debug.LogError("SceneLight not found in the scene.");


            if (!_isDay && !_isStatisticDayOpened)
            {
                DayOverCompleted?.Invoke();
                SetNightTime();
                Debug.Log("НОЧЬ");
                UpdateTimeText(START_HOUR, END_HOUR, timeOfDay);
            }
            else
            {
                UpdateTimeText(START_HOUR, END_HOUR, timeOfDay);
                UpdateSkyboxColor(dayColor, nightColor, timeOfDay, maxExposure, minExposure, _dayAmbientColor,
                    _nightAmbientColor, _dayEquatorColor, _nightEquatorColor, _dayLightColor, _nightLightColor);
            }
        }

        private void Update()
        {
            if (!_isOpen)
                return;

            if (!_isDay && !_isStatisticDayOpened)
            {
                return;
            }

            if (!_isDay && !_isNight)
                return;

            if (_isDay)
            {
                UpdateDayCycle(dayDuration, false, true);

                UpdateSkyboxColor(dayColor, nightColor, timeOfDay, maxExposure, minExposure, _dayAmbientColor,
                    _nightAmbientColor, _dayEquatorColor, _nightEquatorColor, _dayLightColor, _nightLightColor);

                UpdateTimeText(START_HOUR, END_HOUR, timeOfDay);
            }

            if (_isNight)
            {
                UpdateDayCycle(_nightDuration, true, false);

                UpdateSkyboxColor(nightColor, dayColor, timeOfDay, minExposure, maxExposure, _nightAmbientColor,
                    _dayAmbientColor, _nightEquatorColor, _dayEquatorColor, _nightLightColor, _dayLightColor);

                UpdateTimeText(END_HOUR, START_HOUR, timeOfDay);
            }
        }

        public void ResetDay()
        {
            _isDay = true;
            _isNight = false;
            SetDayTime();
        }

        public void SetOpenValue(bool value)
        {
            _isOpen = value;
        }

        public void InitTimeText(TMP_Text timeText)
        {
            _timeText = timeText;
        }

        private void UpdateSkyboxColor(Color currentTintColor, Color targetTintColor, float duration,
            float currentExposure, float targetExposure, Color startAmbientColor, Color endAmbientColor,
            Color startEquatorColor, Color endEquatorColor, Color startLightColor, Color endLightColor)
        {
            Color currentColor = Color.Lerp(currentTintColor, targetTintColor, duration);
            skyboxMaterial.SetColor("_Tint", currentColor);
            float exposure = Mathf.Lerp(currentExposure, targetExposure, duration);
            skyboxMaterial.SetFloat("_Exposure", exposure);
            Color currentAmbientColor = Color.Lerp(startAmbientColor, endAmbientColor, duration);
            RenderSettings.ambientSkyColor = currentAmbientColor;
            Color currentEquatorColor = Color.Lerp(startEquatorColor, endEquatorColor, duration);
            RenderSettings.ambientEquatorColor = currentEquatorColor;
            Color currentLightColor = Color.Lerp(startLightColor, endLightColor, duration);
            sceneLight.color = currentLightColor;
        }

        private void SetNightTime()
        {
            skyboxMaterial.SetColor("_Tint", nightColor);
            skyboxMaterial.SetFloat("_Exposure", minExposure);
            RenderSettings.ambientSkyColor = _nightAmbientColor;
            RenderSettings.ambientEquatorColor = _nightEquatorColor;
            sceneLight.color = _nightLightColor;
        }

        public void SetDayTime()
        {
            skyboxMaterial.SetColor("_Tint", dayColor);
            skyboxMaterial.SetFloat("_Exposure", maxExposure);
            RenderSettings.ambientSkyColor = _dayAmbientColor;
            RenderSettings.ambientEquatorColor = _dayEquatorColor;
            sceneLight.color = _dayLightColor;
            UpdateTimeText(START_HOUR, END_HOUR, 0);
        }

        private void UpdateTimeText(float startHour, float endHour, float duration)
        {
            if (_timeText != null)
            {
                float currentHour = Mathf.Lerp(startHour, endHour, duration);

                int hour = Mathf.FloorToInt(currentHour);
                int minute = Mathf.FloorToInt((currentHour - hour) * 60f);
                _timeText.text = string.Format("{0:00}:{1:00}", hour, minute);

                if (hour >= 21 && minute >= 0)
                    _buyersCounter.SetAllowedSpawnBuyers(false);
                else
                    _buyersCounter.SetAllowedSpawnBuyers(true);

                if (hour >= 18 && minute >= 0)
                    SetNightLighting?.Invoke(true);
                else
                    SetNightLighting?.Invoke(false);
            }
        }

        private void UpdateDayCycle(float duration, bool isDay, bool isNight)
        {
            timeOfDay += Time.deltaTime / duration;

            if (timeOfDay >= 1f)
            {
                if (isNight)
                {
                    DayOverCompleted?.Invoke();
                    _isDay = isDay;
                }
                else
                {
                    timeOfDay = 0;
                    _isDay = isDay;
                    _isNight = isNight;
                }
            }
        }

        public void StartNewDay()
        {
            timeOfDay = 0f;
            _isDay = false;
            _isNight = true;
            _isStatisticDayOpened = true;
            NewDayStarted?.Invoke();
        }

        private void OnApplicationQuit()
        {
            Debug.Log("_isDay OnApplicationQuit " + _isDay);
            PlayerPrefs.SetInt(IS_DAY_KEY, _isDay ? 1 : 0);
            // PlayerPrefs.SetInt(IS_NIGHT_KEY, _isNight ? 1 : 0);
            PlayerPrefs.SetFloat(TIME_OF_DAY_KEY, timeOfDay);
            PlayerPrefs.Save();
        }
    }
}
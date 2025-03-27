using System.Collections;
using TMPro;
using UnityEngine;

namespace ReputationContent
{
    public class RateFly : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _flyDuration = 1.0f;
        [SerializeField] private float _flyHeight = 3.0f;
        [SerializeField] private Transform startPosition;
    
        private Vector3 _startPosition;
        private Coroutine _flyCoroutine;
        private Color _color;

        private void Awake()
        {
            _startPosition = startPosition.position;
        }

        void Start()
        {
            _color = _text.color;
            _color.a = 1;
            _text.color = _color;
        }
    
        public void Show(int rate)
        {
            if (rate == 0) return;
            
            gameObject.SetActive(true);
            _color = rate < 0 ? Color.red : Color.green;
            _text.color = _color;
        
            if (rate > 0)
                _text.text = "+" + rate.ToString();
            else
                _text.text = rate.ToString();
        
            if (_flyCoroutine != null)
                StopCoroutine(_flyCoroutine);
        
            _flyCoroutine= StartCoroutine(FlyUp());
        }

        public void Show(DollarValue dollarValue, string text = " ")
        {
            gameObject.SetActive(true);
            /*_color = rate < 0 ? Color.red : Color.green;
            _text.color = _color;
        
            if (rate > 0)
                _text.text = "+" + rate.ToString();
            else
                _text.text = rate.ToString();*/
            _text.color = Color.green;
            _text.text = "+" + dollarValue.ToString() +" "+ text;
            
            Debug.Log("GameObject activeInHierarchy: " + gameObject.activeInHierarchy);
            Debug.Log("Component enabled: " + this.enabled);
            
            if (gameObject.activeInHierarchy && this.enabled)
            {
                if (transform.parent == null || transform.parent.gameObject.activeInHierarchy)
                {
                    StartCoroutine(FlyUp());
                }
                else
                {
                    Debug.LogError("Parent GameObject is not active in the hierarchy.");
                }
            }
            else
            {
                if (!gameObject.activeInHierarchy)
                {
                    Debug.LogError("GameObject is not active in the hierarchy.");
                }
                if (!this.enabled)
                {
                    Debug.LogError("Component is not enabled.");
                }
            }
            
            
            
            
            
            /*if (_flyCoroutine != null)
                StopCoroutine(_flyCoroutine);
        
            _flyCoroutine= StartCoroutine(FlyUp());*/
        }
    
        private IEnumerator FlyUp()
        {
            Debug.Log("StartFlyUp");
            _color.a = 1;
            float elapsedTime = 0f;
            Vector3 endPosition = _startPosition + Vector3.up * _flyHeight;

            while (elapsedTime < _flyDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / _flyDuration;
                transform.position = Vector3.Lerp(_startPosition, endPosition, t);

                Color color = _text.color;
                color.a = 1 - t;
                _text.color = color;

                yield return null;
            }
        
            Color finalColor = _text.color;
            finalColor.a = 0;
            _text.color = finalColor;
            Debug.Log("EndFlyUp");
            gameObject.SetActive(false);
        }
    }
}
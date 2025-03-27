using TMPro;
using UnityEngine;

namespace UI
{
    public class FlyValue : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        private Color _color;
        
        public void Show(int value)
        {
            if (value == 0) return;
            
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            _color = value < 0 ? Color.red : Color.green;
            _text.color = _color;
            
            if (value > 0)
                _text.text = "+" + value.ToString();
            else
                _text.text = value.ToString();
            
        }
        
        public void Show(DollarValue dollarValue, string text = " ")
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            _text.color = Color.green;
            _text.text = "+" + dollarValue.ToString() +" "+ text;
        }
    }
}
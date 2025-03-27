using TMPro;
using UnityEngine;

namespace DayChangeContent
{
    public class TimeView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeValueText;

        public TMP_Text TimeValueText => _timeValueText;
    }
}
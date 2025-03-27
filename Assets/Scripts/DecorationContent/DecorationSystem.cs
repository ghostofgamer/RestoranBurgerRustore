using UnityEngine;
using UnityEngine.Serialization;

namespace DecorationContent
{
    public enum Decoration
    {
        RedFlower
    }

    public class DecorationSystem : MonoBehaviour
    {
        [SerializeField] private Decoration _currentDailyRewardDecoration;
        
        [FormerlySerializedAs("cubeDecoration")] [SerializeField] private GameObject _redFlowerDecoration;

        public Decoration CurrentDailyRewardDecoration=>_currentDailyRewardDecoration;
        
        private void Start()
        {
            SetValueDecorations();
        }

        private void SetValueDecorations()
        {
            _redFlowerDecoration.SetActive(PlayerPrefs.GetInt(Decoration.RedFlower.ToString(), 0) == 1);
        }

        public void ActivateDecoration(Decoration decoration)
        {
            switch (decoration)
            {
                case Decoration.RedFlower:
                    _redFlowerDecoration.SetActive(true);
                    PlayerPrefs.SetInt(Decoration.RedFlower.ToString(), 1);
                    break;
            }
        }

        public bool GetActivationValueDecoration(Decoration decoration )
        {
            int value = PlayerPrefs.GetInt(decoration.ToString(), 0);
            return value == 0;
        }
    }
}
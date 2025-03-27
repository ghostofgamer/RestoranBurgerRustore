using System;
using TheSTAR.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.EventSystems;

namespace TheSTAR.GUI
{
    public class PointerButton : Pointer
    {
        [SerializeField] private bool scrollable;
        [SerializeField] private ScrollRect scrollRect;
        [Space]
        [SerializeField] protected PointerButtonInfo _info = new PointerButtonInfo();

        protected virtual PointerButtonInfo currentInfo => _info;

        public Image Img => currentInfo._img;

        // scroll
        private const float MinScrollOffset = 50; // если смещение меньше, считается что игрок сделал клик. Если больше - считается что скролл
        private Vector2 startClickPos;
        private Vector2 offset;
        private Vector2 currentClickPos;

        [Inject] private SoundController _soundController;
        private Action _clickAction;
        private Action _enterAction;
        private Action _exitAction;
        private bool _isEnter = false;
        private bool _isDown = false;
        private bool _isInteractalbe = true;

        private static bool globalIsDown = false;
        private static DateTime previousClickTime;
        public static bool GlobalIsDown => globalIsDown || (DateTime.Now - previousClickTime).TotalSeconds < MinClickDuration;

        private const float MinClickDuration = 0.1f;

        private void Start() 
        {
            InitPointer(
                OnButtonDown,
                OnButtonDrag,
                OnButtonUp,
                OnButtonEnter,
                OnButtonExit);
        }

        public void Init(SoundController sounds)
        {
            this._soundController = sounds;
        }

        public void Init(Action clickAction, Action enterAction = null, Action exitAction = null)
        {
            _clickAction = clickAction;
            _enterAction = enterAction;
            _exitAction = exitAction;
        }

        private void OnButtonEnter(PointerEventData eventData)
        {
            if (!_isInteractalbe) return;
            if (currentInfo._useEnterSound && _soundController) _soundController.Play(currentInfo._enterSoundType);
            
            _isEnter = true;

            UpdateVisual();

            _enterAction?.Invoke();
        }

        private void OnButtonExit(PointerEventData eventData)
        {            
            if (!_isInteractalbe) return;
            
            _isEnter = false;

            UpdateVisual();

            _exitAction?.Invoke();
        }

        private void OnButtonDrag(PointerEventData eventData)
        {
            if (scrollable)
            {
                scrollRect.OnDrag(eventData);
                currentClickPos = eventData.position;
                offset = currentClickPos - startClickPos;
            }
        }

        private void OnButtonDown(PointerEventData eventData)
        {
            if (scrollable)
            {
                scrollRect.OnBeginDrag(eventData);
                startClickPos = eventData.position;
                currentClickPos = eventData.position;
                offset = Vector2.zero;
            }

            if (!_isInteractalbe) return;
            if (currentInfo._useClickSound && _soundController) _soundController.Play(currentInfo._clickSoundType);
            
            _isDown = true;
            globalIsDown = true;
            //Debug.Log("GLOBAL DOWN");
            previousClickTime = DateTime.Now;

            UpdateVisual();
        }

        private void OnButtonUp(PointerEventData eventData)
        {
            bool lockButtonEventForScroll = false;
            if (scrollable)
            {
                scrollRect.OnEndDrag(eventData);
                currentClickPos = eventData.position;
                offset = currentClickPos - startClickPos;
                float result = Math.Abs(offset.x) + Math.Abs(offset.y);

                lockButtonEventForScroll = result > MinScrollOffset;
            }

            if (!_isInteractalbe) return;
            if (_isEnter)
            {
                if (!lockButtonEventForScroll)
                {
                    previousClickTime = DateTime.Now;
                    _clickAction?.Invoke();
                }
            }

            _isDown = false;
            globalIsDown = false;
            //Debug.Log("GLOBAL UP");
            UpdateVisual();
        }

        private void OnDisable()
        {
            _isEnter = false;
            _isDown = false;
            UpdateVisual();
        }

        public void SetInteractable(bool value)
        {
            _isInteractalbe = value;
            UpdateVisual();
        }

        protected void UpdateVisual()
        {
            if (!_isInteractalbe)
            {
                if (currentInfo._useChangeColor) currentInfo._img.color = currentInfo._disableColor;
                if (currentInfo._useChangeSprite) currentInfo._img.sprite = currentInfo._disableSprite;
            }
            else if (_isEnter)
            {
                if (currentInfo._useChangeColor) currentInfo._img.color = _isDown ? currentInfo._pressColor : currentInfo._idleColor;
                if (currentInfo._useChangeSprite) currentInfo._img.sprite = _isDown ? currentInfo._pressSprite : currentInfo._idleSprite;
            }
            else
            {
                if (currentInfo._useChangeColor) currentInfo._img.color = currentInfo._idleColor;
                if (currentInfo._useChangeSprite) currentInfo._img.sprite = currentInfo._idleSprite;
            }
        }

        [Serializable]
        public class PointerButtonInfo
        {
            [HideInInspector] public bool _useEnterSound => false;
            [HideInInspector] public SoundType _enterSoundType;

            public bool _useClickSound;
            public SoundType _clickSoundType;
            
            [Space]
            public bool _useChangeSprite;
            public Sprite _idleSprite;
            //public Sprite _selectSprite;
            public Sprite _pressSprite;
            public Sprite _disableSprite;

            [Space]
            public bool _useChangeColor;
            public Color _idleColor = Color.white;
            //public Color _selectColor = Color.white;
            public Color _pressColor = new (0.8f, 0.8f, 0.8f, 1);
            public Color _disableColor = Color.white;

            //[ShowIf("@_useChangeSprite || _useChangeColor")] 
            [Space]
            public Image _img;
        }
    
        public void SetScrollable(ScrollRect scrollRect)
        {
            if (!scrollRect) return;
            this.scrollRect = scrollRect;
            scrollable = true;
        }
    }
}
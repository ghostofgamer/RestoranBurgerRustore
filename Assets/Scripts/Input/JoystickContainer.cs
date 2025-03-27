using System;
using TheSTAR.Utility;
using TheSTAR.GUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheSTAR.Input
{
    public class JoystickContainer : MonoBehaviour
    {
        [SerializeField] private Pointer pointer;
        [SerializeField] private CanvasGroup joystickCanvasGroup;
        [SerializeField] private GameObject stickObject;
        
        private const float LimitDistance = 170;
        private const float ShowTime = 0.1f;

        private bool _isDown = false;
        private int _showHideLTID = -1;

        public event Action OnStartJoystickInteractEvent;
        public event Action<Vector2> JoystickInputEvent;
        public event Action OnEndJoystickInteractEvent;

        private const float InactiveJoystickAlpha = 1f;
        private const float ActiveJoystickAlpha = 1f;

        public void Init()
        {
            pointer.InitPointer(
                (eventData) => OnJoystickDown(eventData), 
                (eventData) => OnJoystickDrag(eventData),
                (eventData) => OnJoystickUp());
        }

        private void Start()
        {
            joystickCanvasGroup.alpha = InactiveJoystickAlpha;
        }

        private void FixedUpdate()
        {
            JoystickInput();
        }

        private void OnJoystickDown(PointerEventData eventData)
        {
            _isDown = true;
            ShowJoystick(eventData);
            UpdateStickPosByMouse(eventData);

            OnStartJoystickInteractEvent?.Invoke();
        }
    
        private void OnJoystickDrag(PointerEventData eventData)
        {
            _isDown = true;
            UpdateStickPosByMouse(eventData);
        }
    
        private void OnJoystickUp()
        {
            _isDown = false;
            HideJoystick();

            OnEndJoystickInteractEvent?.Invoke();
        }

        private void UpdateStickPosByMouse(PointerEventData eventData)
        {
            stickObject.transform.position = eventData.position; // UnityEngine.Input.mousePosition;
            stickObject.transform.localPosition = MathUtility.LimitForCircle(stickObject.transform.localPosition, LimitDistance);
        }

        private void JoystickInput()
        {
            if (_isDown) JoystickInputEvent?.Invoke(stickObject.transform.localPosition / LimitDistance);
            else JoystickInputEvent?.Invoke(Vector2.zero);
        }

        #region Show/Hide

        private void ShowJoystick(PointerEventData eventData)
        {
            if (_showHideLTID != -1) LeanTween.cancel(_showHideLTID);
            
            joystickCanvasGroup.transform.position = eventData.position; //UnityEngine.Input.mousePosition;
            //joystickCanvasGroup.alpha = 0;
            
            _showHideLTID =
            LeanTween.alphaCanvas(joystickCanvasGroup, ActiveJoystickAlpha, ShowTime).setOnComplete(() =>
            {
                _showHideLTID = -1;
            }).id;
        }
        
        private void HideJoystick()
        {
            if (_showHideLTID != -1)
            {
                LeanTween.cancel(_showHideLTID);
                _showHideLTID = -1;
            }

            stickObject.transform.localPosition = Vector3.zero;
            joystickCanvasGroup.transform.localPosition = Vector3.zero;

            joystickCanvasGroup.alpha = InactiveJoystickAlpha;
        }

        #endregion

        public void BreakInput()
        {
            if (_isDown)
            {
                OnJoystickUp();
                JoystickInput();
            }
        }
    }

    public interface IJoystickControlled
    {
        void JoystickInput(Vector2 input);
    }
}
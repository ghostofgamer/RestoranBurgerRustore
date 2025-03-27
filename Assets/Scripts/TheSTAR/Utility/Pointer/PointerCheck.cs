using System;
using UnityEngine;
using UnityEngine.UI;

namespace TheSTAR.GUI
{
    public class PointerCheck : MonoBehaviour
    {
        [SerializeField] private Pointer pointer;
        [SerializeField] private Image fonImg;
        [SerializeField] private GameObject checkObject;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool value;
        private bool interactable = true;

        public Image FonImg => fonImg;

        private Action<bool> onChangeValueAction;

        public void Init(Action<bool> onChangeValueAction, bool value, bool interactable)
        {
            this.onChangeValueAction = onChangeValueAction;
            pointer.InitPointer((eventData) =>
            {
                if (!interactable) return;
                SetValue(!this.value);
            });

            SetValueWithoutNotify(value);
            SetInteractable(interactable);
        }

        public void SetValueWithoutNotify(bool value)
        {
            this.value = value;
        }

        private void SetValue(bool value)
        {
            this.value = value;
            UpdateVisual();

            onChangeValueAction?.Invoke(value);
        }

        private void UpdateVisual()
        {
            checkObject.SetActive(value);
            canvasGroup.alpha = interactable ? 1 : 0.3f;
        }

        public void SetInteractable(bool interactable)
        {
            this.interactable = interactable;
            UpdateVisual();
        }
    }
}
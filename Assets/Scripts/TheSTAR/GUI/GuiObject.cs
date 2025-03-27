using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TheSTAR.GUI
{
    public class GuiObject : MonoBehaviour
    {
        private bool isShow = false;
        public bool IsShow => isShow;

        public virtual void Init() { }

        public async void Show(Action endAction = null, bool skipShowAnim = false)
        {
            //if (IsShow) return;
            
            isShow = true;
            gameObject.SetActive(true);

            OnShow();

            if (!skipShowAnim)
            {
                AnimateShow(out int hideTime);
                await Task.Delay(hideTime);
            }

            endAction?.Invoke();
        }

        protected virtual void OnShow()
        {}

        public async void Hide(Action endAction = null)
        {
            //if (!IsShow) return;
            
            AnimateHide(out int hideTime);

            await Task.Delay(hideTime);

            gameObject.SetActive(false);
            OnHide();
            isShow = false;

            endAction?.Invoke();
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void AnimateShow(out int showTime)
        {
            showTime = 0;
        }

        protected virtual void AnimateHide(out int hideTime)
        {
            hideTime = 0;
        }

        public virtual void Reset()
        {
        }

        public override string ToString()
        {
            return GetType().ToString();
        }
    }
}
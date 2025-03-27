using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TheSTAR.GUI
{
    public class LoadScreen : GuiScreen
    {
        [SerializeField] private Image loadBarFillImg;

        private Action loadingAction;
        private Action exitFromLoadingAction;

        private const float ProgressForLoading = 0.8f;

        public void Init(Action loadingAction, Action exitFromLoadingAction)
        {
            this.loadingAction = loadingAction;
            this.exitFromLoadingAction = exitFromLoadingAction;
        }

        protected override void OnShow()
        {
            base.OnShow();

            DOVirtual.Float(0f, ProgressForLoading, 1f, (value) =>
            {
                loadBarFillImg.fillAmount = value;
            }).OnComplete(DoDelayAction);
        }

        private void DoDelayAction()
        {
            loadingAction?.Invoke();

            DOVirtual.Float(ProgressForLoading, 1f, 1f, (value) =>
            {
                loadBarFillImg.fillAmount = value;
            }).OnComplete(() =>
            {
                exitFromLoadingAction?.Invoke();
            });
        }
    }
}
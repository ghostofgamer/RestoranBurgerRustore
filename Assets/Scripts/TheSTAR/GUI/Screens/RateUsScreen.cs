using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TheSTAR.Data;
using Zenject;

namespace TheSTAR.GUI
{
    public class RateUsScreen : GuiScreen
    {
        [SerializeField] private PointerButton closeButton;
        [SerializeField] private PointerButton noButton;
        [SerializeField] private PointerButton yesButton;

        public static string GetRateUsURL => GetGooglePlayURL;
        private static string GetGooglePlayURL => $"https://play.google.com/store/apps/details?id={Application.identifier}";

        private Action exitAction;

        private DataController data;

        [Inject]
        private void Construct(DataController data)
        {
            this.data = data;
        }

        public override void Init()
        {
            base.Init();

            closeButton.Init(OnNoClick);
            noButton.Init(OnNoClick);
            yesButton.Init(OnYesClick);
        }

        public void Init(Action exitAction, bool useMainMenuFon)
        {
            this.exitAction = exitAction;
            SetUseMainMenuFon(useMainMenuFon);
        }

        private void OnNoClick()
        {
            PlanNextRateUs();
            exitAction?.Invoke();
        }

        private void OnYesClick()
        {
            Application.OpenURL(GetRateUsURL);
            //GameController.Instance.IAR.ShowInAppReview();
            exitAction?.Invoke();
            data.gameData.commonData.gameRated = true;
        }

        private void PlanNextRateUs()
        {
            var planDateTime = System.DateTime.Now.AddDays(3);
            data.gameData.commonData.nextRateUsPlan = planDateTime;
            data.gameData.commonData.rateUsPlanned = true;
            //Debug.Log("Plan for next rate us: " + planDateTime);
        }
    }
}
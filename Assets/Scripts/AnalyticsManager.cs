using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using TheSTAR.Data;

public class AnalyticsManager
{
    private bool showDebugs = false;
    
    private DataController data;

    private readonly int[] repeatingEventsValue = new int[] 
    {
        1,
        2,
        3,
        4,
        5,
        10,
        20,
        30,
        40,
        50,
        100,
        250,
        500,
        1000,
        2000,
        3000,
        4000,
        5000,
        10000,
        100000,
        1000000
    };

    [Inject]
    private void Construct(DataController data)
    {
        this.data = data;
    }

    public void Trigger(RepeatingEventType repeatingEventType)
    {
        var RED = data.gameData.analyticsData.repeatingEventsData;
        if (!RED.ContainsKey(repeatingEventType)) RED.Add(repeatingEventType, 1);
        else RED[repeatingEventType]++;

        if (repeatingEventsValue.Contains(RED[repeatingEventType]))
        {
            Log(repeatingEventType, RED[repeatingEventType].ToString());
        }

        data.Save(TheSTAR.Data.DataSectionType.AnalyticsData);
    }

    public void Trigger(SingleEventType singleEventType)
    {
        var SED = data.gameData.analyticsData.singleEventsData;
        if (SED.ContainsKey(singleEventType) && SED[singleEventType]) return;
        if (!SED.ContainsKey(singleEventType)) SED.Add(singleEventType, true);
        else SED[singleEventType] = true;

        Log(singleEventType, "True");

        data.Save(TheSTAR.Data.DataSectionType.AnalyticsData);
    }

    public void LogForTutorial(TutorialType tutorialType)
    {
        /*if (tutorialType == TutorialType.LookAround)
        {
            Debug.Log("Look around Metrica");
            Log(RepeatingEventType.TutorialLookAround, tutorialType.ToString());
        }

        
        if (tutorialType == TutorialType.Move)
        {
            Debug.Log("Move Metrica");
            Log(RepeatingEventType.TutorialMove, tutorialType.ToString());
        }*/
        
        
        Log(RepeatingEventType.Tutor, tutorialType.ToString());
    }

    public void Log(RepeatingEventType section, string eventText)
    {
        AppMetricLog(section.ToString(), eventText);
    }

    public void Log(SingleEventType section, string eventText)
    {
        AppMetricLog(section.ToString(), eventText);
    }

    private void AppMetricLog(string sectionString, string eventString)
    {
        var data = new Dictionary<string, object>
        {
            [eventString] = null
        };
        ReportEvent(sectionString, data);

        OnAnalyticSent($"{sectionString} | {eventString}");
    }

    private void ReportEvent(string sectionString, Dictionary<string, object> data)
    {
        AppMetrica.Instance.ReportEvent(sectionString, data);
    }

    private void OnAnalyticSent(string debugMessage)
    {
        if (showDebugs) Debug.Log("[analytic] " + debugMessage);
    }
}

[Serializable]
public struct AdAnalyticData
{
    public string ad_type;
    public string placement;
    public string result;
    public bool connection;

    public AdAnalyticData(string ad_type, string placement, string result, bool connection)
    {
        this.ad_type = ad_type;
        this.placement = placement;
        this.result = result;
        this.connection = connection;
    }
}

public enum AdEventType
{
    ad_available,
    ad_started,
    ad_watch
}

public enum AdType
{
    interstitial,
    rewarded,
    banner
}

// Ивенты, срабатывающие множество раз
public enum RepeatingEventType
{
    StartSession,

    OrderAccepted,
    OrderCompleted,
    
    Give_Coffee,
    Give_BarberrySoda,
    Give_OrangeSoda,
    Give_LemonSoda,
    Give_PlumColdSoda,
    Give_Burger_S,
    Give_Cheeseburger,
    Give_Burger_M,
    Give_StarBurger,
    Give_BigBurger,
    Give_MegaBurger,

    Purchase_SingleChair,
    Purchase_DoubleChair,
    Purchase_Sofa,
    Purchase_SofaAndChair,

    LevelUp,

    Purchase_CoffeeMachine,
    Purchase_DeepFryer,
    Purchase_SodaMachine,
    Purchase_Section,
    
    Tutor,
    TutorialLookAround,
    TutorialMove,

    ShowBannerAd,
    ShowInterstitialAd,
    ShowRewardedAd,
    
    //////// ВЫШЕ ГОТОВЫЕ
    
    // todo сделать для наггетсов и картошки фри
    Give_FrenchFries,
    Give_Naggets,
}

// Ивенты, срабатывающие только один раз
public enum SingleEventType
{   
    
}
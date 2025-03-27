using UnityEngine;

public class AdaptiveQuality : MonoBehaviour
{
    [SerializeField] private int neededMemory; // требуемое количество оперативной памяти

    private AdaptiveQualityType currentQuality;
    public AdaptiveQualityType CurrentQuality => currentQuality;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void AutoUpdateQuality()
    {
        //if (GameController.Instance.Data.gameData.qualityWasUpdated) SetQuality(GameController.Instance.Data.gameData.qualityLevel, false);
        //else
        //{
            int ram = SystemInfo.systemMemorySize;
            AdaptiveQualityType quality = ram > neededMemory ? AdaptiveQualityType.Height : AdaptiveQualityType.Low;
            SetQuality(quality, true);
        //}
    }

    private void SetQuality(AdaptiveQualityType quality, bool autoSetToData)
    {
        currentQuality = quality;

        /*
        if (autoSetToData)
        {
            GameController.Instance.Data.gameData.qualityLevel = quality;
            GameController.Instance.Data.gameData.qualityWasUpdated = true;
        }
        */
    }
}

public enum AdaptiveQualityType
{
    Height,
    Low
}
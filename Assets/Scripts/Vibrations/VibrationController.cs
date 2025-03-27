using UnityEngine;
using TheSTAR.Data;
using Zenject;

public class VibrationController
{
    private DataController data;

    [Inject]
    private void Construct(DataController data)
    {
        this.data = data;
    }

    public void Vibrate(int durationMilliseconds)
    {
        if (!data.gameData.settingsData.isVibrationOn) return;

        Vibrator.Vibrate(durationMilliseconds);

        /*
        switch (vibrationType)
        {
            case VibrationType.Default:
                Vibrator.Vibrate(DefaultVibrationDuration);
                break;

            case VibrationType.Small:
                Vibrator.Vibrate(SmallVibrationDuration);
                break;
        }
        */
    }
}

public enum VibrationType
{
    Default,
    Small
}
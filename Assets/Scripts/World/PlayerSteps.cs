using TheSTAR.Sound;
using TheSTAR.Utility;
using UnityEngine;
using Zenject;

public class PlayerSteps : MonoBehaviour
{
    private SoundController sounds;

    [SerializeField] private SoundType[] stepSoundVariants;

    private float stepProgress = 0; // когда достигает значения 1 воспроизводится звук шага

    [Inject]
    private void Construct(SoundController sounds)
    {
        this.sounds = sounds;
    }

    public void AddValue(float oneStepProgress)
    {
        stepProgress += oneStepProgress;

        if (stepProgress >= 1)
        {
            stepProgress = 0;
            sounds.Play(ArrayUtility.GetRandomValue(stepSoundVariants), Random.Range(0.9f, 1.1f));
        }
    }
}
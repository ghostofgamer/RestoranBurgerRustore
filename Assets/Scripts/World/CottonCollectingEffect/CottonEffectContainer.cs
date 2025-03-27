using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Collections;

public class CottonEffectContainer : MonoBehaviour
{
    [SerializeField] private CottonEffectObject[] effectObjects;
    [SerializeField] private float distance = 1;
    [SerializeField] private float startOffsetY = -0.5f;
    [SerializeField] private float timeStep;
    [SerializeField] private float flyDuration;

    private int currentEffectObjectIndex = -1;

    private bool effectIsActive = false;

    [ContextMenu("Play")]
    public void Play()
    {
        effectIsActive = true;
        StartCoroutine(EffectCor());
    }

    private IEnumerator EffectCor()
    {
        while (effectIsActive)
        {
            currentEffectObjectIndex++;
            if (currentEffectObjectIndex >= effectObjects.Length) currentEffectObjectIndex = 0;
            StartFlyForEffectObject(effectObjects[currentEffectObjectIndex]);
            yield return new WaitForSeconds(timeStep);
        }
    }

    private void StartFlyForEffectObject(CottonEffectObject effectObject)
    {
        effectObject.Stop();
        effectObject.transform.localPosition = GetRandomOffset();
        effectObject.Play();

        effectObject.transform.DOLocalMove(Vector3.zero, flyDuration).OnComplete(() =>
        {
            effectObject.Stop();
        });
    }

    private Vector3 GetRandomOffset()
    {
        Vector2 offsetFlat = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        offsetFlat = offsetFlat.normalized * distance;
        return new Vector3(offsetFlat.x, startOffsetY, offsetFlat.y);
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        StopAllCoroutines();

        foreach (var effect in effectObjects) effect.Stop();

        effectIsActive = false;
    }
}
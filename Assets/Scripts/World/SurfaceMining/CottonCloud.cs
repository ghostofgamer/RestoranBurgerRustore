using UnityEngine;
using DG.Tweening;

public class CottonCloud : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void AnimateHide(float delay)
    {
        if (delay == 0) StartAnimate();
        else
        {
            DOVirtual.Float(0, 1, delay, value => { }).OnComplete(() =>
            {
                StartAnimate();
            });
        }
    }

    private void StartAnimate()
    {
        anim.Play("Hide");
    }
}
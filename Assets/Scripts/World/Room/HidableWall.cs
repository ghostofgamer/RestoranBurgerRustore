using UnityEngine;
using DG.Tweening;

public class HidableWall : Wall
{
    [SerializeField] private MeshRenderer rend;

    private const float VisibleAlpha = 1;
    private const float InvisibleAlpha = 0.7f;
    private const float TweenDuration = 0.5f;

    private Tweener tweener;

    public void Show()
    {
        ClearTweener();
        tweener = rend.material.DOColor(new Color(1, 1, 1, VisibleAlpha), TweenDuration);
    }

    public void Hide()
    {
        ClearTweener();
        tweener = rend.material.DOColor(new Color(1, 1, 1, InvisibleAlpha), TweenDuration);
    }

    private void ClearTweener()
    {
        if (tweener != null)
        {
            tweener.Kill();
            tweener = null;
        }
    }
}
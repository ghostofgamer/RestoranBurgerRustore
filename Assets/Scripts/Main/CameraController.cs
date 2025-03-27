using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera main;
    [SerializeField] private RayVision rayVision;

    private int currentMultiplierIndex = 0;
    public int CurrentMultiplierIndex => currentMultiplierIndex;
    public float CurrentCameraAngleY => main.transform.eulerAngles.y;
    public RayVision RayVision => rayVision;

    private ICameraFocusable mainFocus;

    public Vector3 ForwardDirection => main.transform.forward;

    private Tweener moveDraggableTweener;
    private Tweener rotateDraggableTweener;

    private const float MoveDuration = 0.25f;
    private readonly Ease MoveEase = Ease.OutCubic;

    /// <summary>
    /// Задать объект, за которым камера будет постоянно следить
    /// </summary>
    public void SetMainFocus(ICameraFocusable focus)
    {
        mainFocus = focus;
        main.transform.parent = focus.FocusTransform;
        main.transform.localEulerAngles = Vector3.zero;
        main.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Камера фокусируется на этом временном объекте
    /// </summary>
    public void TempFocus(ICameraFocusable focus, bool cloneRotation)
    {
        moveDraggableTweener?.Kill();
        rotateDraggableTweener?.Kill();

        if (focus == null)
        {
            moveDraggableTweener =
            main.transform.DOLocalMove(Vector3.zero, MoveDuration).SetEase(MoveEase);

            rotateDraggableTweener =
            main.transform.DOLocalRotate(Vector3.zero, MoveDuration).SetEase(MoveEase);
        }
        else
        {
            moveDraggableTweener =
            main.transform.DOMove(focus.FocusTransform.position, MoveDuration).SetEase(MoveEase);

            if (cloneRotation)
            {
                rotateDraggableTweener = 
                main.transform.DORotate(focus.FocusTransform.eulerAngles, MoveDuration).SetEase(MoveEase);
            }
        }
    }

    public bool IsObjectInView(Collider col)
    {
        // Получаем фрустум камеры в виде плоскостей
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        // Проверяем, находится ли AABB объекта в поле зрения камеры
        return GeometryUtility.TestPlanesAABB(frustumPlanes, col.bounds);
    }

    public void ActivateRayVision()
    {
        rayVision.Activate();
    }

    public void DeactivateRayVision()
    {
        rayVision.Deactivate();
    }
}

public interface ICameraFocusable
{
    Transform FocusTransform { get; }
}
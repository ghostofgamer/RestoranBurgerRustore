using UnityEngine;

public class CanvasRotator : MonoBehaviour
{
    [SerializeField] private Canvas _worldCanvas; 

    void LateUpdate()
    {
        Camera mainCamera = Camera.main;
        
        if (mainCamera != null && _worldCanvas != null)
        {
            // Получаем направление от канваса к камере
            Vector3 directionToCamera = mainCamera.transform.position - _worldCanvas.transform.position;
            // Вычисляем угол поворота по оси Y
            float angleY = Mathf.Atan2(directionToCamera.x, directionToCamera.z) * Mathf.Rad2Deg;
            // Применяем поворот к канвасу
            _worldCanvas.transform.rotation = Quaternion.Euler(0, angleY, 0);
        }
    }
}
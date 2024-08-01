using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target; // Персонаж, за которым будет следовать камера
    public Vector3 offset = new Vector3(0, 2, -5); // Смещение камеры от персонажа
    public float smoothSpeed = 0.125f; // Скорость сглаживания движения камеры
    public float fastSmoothSpeed = 0.5f; // Скорость сглаживания для быстрого движения камеры
    public float rotationSpeed = 5.0f; // Скорость поворота камеры
    public float minPitch = -30f; // Минимальное значение угла наклона камеры
    public float maxPitch = 60f; // Максимальное значение угла наклона камеры
    public float positionThreshold = 0.5f; // Порог значительного движения камеры

    private float yaw = 0.0f; // Угол поворота вокруг вертикальной оси
    private float pitch = 0.0f; // Угол наклона камеры вверх/вниз
 
   void LateUpdate()
   {
        // Получение ввода мыши
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Обновление углов вращения
        yaw += mouseX * rotationSpeed * Time.deltaTime;
        pitch -= mouseY * rotationSpeed * Time.deltaTime;

        // Ограничение вертикального вращения
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Расчет смещения камеры
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(offset.x, offset.y, offset.z);

        // Плавное перемещение камеры
        float distance = Vector3.Distance(transform.position, desiredPosition);
        float appliedSmoothSpeed = distance > positionThreshold ? fastSmoothSpeed : smoothSpeed;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, appliedSmoothSpeed);
        transform.position = smoothedPosition;

        // Камера смотрит на персонажа
        transform.LookAt(target.position + Vector3.up * offset.y);
   }
} 
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target; // ��������, �� ������� ����� ��������� ������
    public Vector3 offset = new Vector3(0, 2, -5); // �������� ������ �� ���������
    public float smoothSpeed = 0.125f; // �������� ����������� �������� ������
    public float fastSmoothSpeed = 0.5f; // �������� ����������� ��� �������� �������� ������
    public float rotationSpeed = 5.0f; // �������� �������� ������
    public float minPitch = -30f; // ����������� �������� ���� ������� ������
    public float maxPitch = 60f; // ������������ �������� ���� ������� ������
    public float positionThreshold = 0.5f; // ����� ������������� �������� ������

    private float yaw = 0.0f; // ���� �������� ������ ������������ ���
    private float pitch = 0.0f; // ���� ������� ������ �����/����
 
   void LateUpdate()
   {
        // ��������� ����� ����
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ���������� ����� ��������
        yaw += mouseX * rotationSpeed * Time.deltaTime;
        pitch -= mouseY * rotationSpeed * Time.deltaTime;

        // ����������� ������������� ��������
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // ������ �������� ������
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(offset.x, offset.y, offset.z);

        // ������� ����������� ������
        float distance = Vector3.Distance(transform.position, desiredPosition);
        float appliedSmoothSpeed = distance > positionThreshold ? fastSmoothSpeed : smoothSpeed;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, appliedSmoothSpeed);
        transform.position = smoothedPosition;

        // ������ ������� �� ���������
        transform.LookAt(target.position + Vector3.up * offset.y);
   }
} 
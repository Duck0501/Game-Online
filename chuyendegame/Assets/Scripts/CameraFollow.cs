using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Đối tượng mà camera sẽ theo dõi (người chơi)
    public Vector3 offset = new Vector3(0, 2.89f, -9.29f); // Khoảng cách từ camera đến người chơi
    public float smoothSpeed = 0.125f; // Tốc độ làm mượt chuyển động camera

    private void LateUpdate()
    {
        if (target == null) return;

        // Tính toán vị trí mong muốn của camera
        Vector3 desiredPosition = target.position + offset;
        // Làm mượt chuyển động camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Đảm bảo camera luôn hướng về người chơi
        transform.LookAt(target);
    }

    // Hàm để gán target cho camera
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
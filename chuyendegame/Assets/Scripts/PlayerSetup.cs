using Cinemachine;
using Fusion;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    public CinemachineVirtualCamera VirtualCameraPrefab;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            CinemachineVirtualCamera virtualCamera = Instantiate(VirtualCameraPrefab);
            virtualCamera.Follow = transform; // Camera di chuyển theo player
            virtualCamera.LookAt = transform; // Camera nhìn vào player

            virtualCamera.Priority = 20;

            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_FollowOffset = new Vector3(0, 2, -10); // Camera ở phía sau và trên player
                transposer.m_XDamping = 0; // Không có độ trễ trên trục X
                transposer.m_YDamping = 0; // Không có độ trễ trên trục Y
                transposer.m_ZDamping = 0; // Không có độ trễ trên trục Z
            }

            // Cấu hình Aim để camera luôn nhìn vào player
            var composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer != null)
            {
                composer.m_TrackedObjectOffset = Vector3.zero; // Không offset thêm khi nhìn
                composer.m_ScreenX = 0.5f; // Đặt player ở giữa màn hình theo trục X
                composer.m_ScreenY = 0.5f; // Đặt player ở giữa màn hình theo trục Y
            }
        }
    }
}
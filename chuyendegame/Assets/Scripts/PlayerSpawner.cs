using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public CameraFollow cameraFollow;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            // Spawn người chơi
            NetworkObject playerObject = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            if (cameraFollow == null)
            {
                cameraFollow = Camera.main.GetComponent<CameraFollow>();
            }

            if (cameraFollow != null)
            {
                cameraFollow.SetTarget(playerObject.transform);
            }
        }
    }
}
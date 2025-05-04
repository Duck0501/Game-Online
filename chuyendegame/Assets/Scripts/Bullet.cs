using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    public float speed = 20f;
    public float lifetime = 5f; // Thời gian tồn tại của đạn
    public float damage = 10f;

    [Networked] private TickTimer LifeTimer { get; set; }

    public override void Spawned()
    {
        // Khởi tạo timer để hủy đạn sau một khoảng thời gian
        LifeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);
    }

    public override void FixedUpdateNetwork()
    {
        // Di chuyển đạn
        transform.position += transform.forward * speed * Runner.DeltaTime;

        // Hủy đạn khi hết thời gian
        if (LifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Object.HasStateAuthority)
        {
            var player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Gọi RPC để gây sát thương
                player.RPC_TakeDamage(damage);
            }
            // Hủy đạn khi va chạm
            Runner.Despawn(Object);
        }
    }
}
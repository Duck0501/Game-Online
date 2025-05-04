using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    private int Health { get; set; } = 100;

    public Slider healthSlider;

    private void OnHealthChanged()
    {
        healthSlider.value = Health;
    }

    void Update()
    {
        if (!Object.HasInputAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Health -= 10;
            }
        }
    }
}
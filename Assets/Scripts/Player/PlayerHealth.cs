using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;

private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1f);
        }
    }
    public void TakeDamage(float amount)
    {
        stats.Health -= amount;
        if (stats.Health <= 0)
        {
            PlayerDead();
        }
    }

    private void PlayerDead()
    {
        Debug.Log("Player is dead");
    }
}

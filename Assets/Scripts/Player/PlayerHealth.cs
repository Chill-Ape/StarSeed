using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    
    private PlayerAnimations playerAnimations;
    private bool isDead = false;

    private void Awake()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    private void Update()
    {
        if (stats.Health <= 0f && !isDead)
        {
            PlayerDead();
        }
    }

    public void TakeDamage(float amount)
    {
        if (stats.Health <= 0f || isDead) return;
        stats.Health -= amount;
        DamageManager.Instance.ShowDamageText(amount, transform);
        if (stats.Health <= 0f)
        {
            stats.Health = 0f;
            PlayerDead();
        }
    }

    public void RestoreHealth(float amount)
    {
        stats.Health += amount;
        if (stats.Health > stats.MaxHealth)
        {
            stats.Health = stats.MaxHealth;
        }
    }
    
    public bool CanRestoreHealth()
    {
        return stats.Health > 0 && stats.Health < stats.MaxHealth;
    }
    
    private void PlayerDead()
    {
        if (isDead) return;
        isDead = true;
        
        // Stop all movement and actions
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;
        
        // Reset any ongoing animations
        playerAnimations.SetMoveBoolTransition(false);
        playerAnimations.SetAttackAnimation(false);
        
        // Set the death animation
        playerAnimations.SetDeadAnimation();
        
        // Disable the Rigidbody2D to prevent any physics interactions
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void ResetPlayer()
    {
        isDead = false;
        stats.ResetPlayer();
        
        // Re-enable components
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerAttack>().enabled = true;
        
        // Re-enable physics
        GetComponent<Rigidbody2D>().isKinematic = false;
        
        // Reset animations
        playerAnimations.ResetPlayer();
    }
}

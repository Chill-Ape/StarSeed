using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    
    private PlayerAnimation playerAnimations;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        playerAnimations = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (stats.Health <= 0f && !IsDead)
        {
            PlayerDead();
        }
    }

    public void TakeDamage(float amount)
    {
        if (stats.Health <= 0f || IsDead) return;
        
        // Get the PlayerAttack component to check blocking state
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            // Let PlayerAttack handle the damage reduction
            float finalDamage = playerAttack.ProcessDamage(amount);
            stats.Health -= finalDamage;
            DamageManager.Instance.ShowDamageText(finalDamage, transform, false);
        }
        else
        {
            // Fallback if PlayerAttack is not found
            stats.Health -= amount;
            DamageManager.Instance.ShowDamageText(amount, transform, false);
        }
        
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
        if (IsDead) return;
        IsDead = true;
        
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
        IsDead = false;
        stats.Health = stats.MaxHealth;
        
        // Re-enable components
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerAttack>().enabled = true;
        
        // Reset the Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        
        // Reset all animation triggers and states
        playerAnimations.ResetAllTriggers();
        playerAnimations.SetReviveAnimation();
        playerAnimations.SetMoveBoolTransition(false);
        playerAnimations.SetAttackAnimation(false);
    }
}

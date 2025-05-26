using UnityEngine;
using System.Collections;

public class ActionAttack : FSMAction
{
    [Header("Attack Settings")]
    [SerializeField] private float windupDuration = 0.5f;
    [SerializeField] private float dashDistance = 2f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float minDamage = 5f;
    [SerializeField] private float maxDamage = 10f;

    private EnemyBrain enemyBrain;
    private Animator animator;
    private Coroutine attackCoroutine;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
        animator = GetComponent<Animator>();
    }

    public override void Act()
    {
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(WindupAttack());
        }
    }

    private IEnumerator WindupAttack()
    {
        if (enemyBrain == null || enemyBrain.Player == null)
        {
            attackCoroutine = null;
            yield break;
        }

        // Start windup animation
        if (animator != null)
        {
            animator.SetBool("IsWindup", true);
        }

        // Wait for windup duration
        yield return new WaitForSeconds(windupDuration);

        // Stop windup animation
        if (animator != null)
        {
            animator.SetBool("IsWindup", false);
        }

        // Calculate dash direction
        Vector3 dashDirection = (enemyBrain.Player.transform.position - transform.position).normalized;
        
        // Store initial position
        Vector3 startPosition = transform.position;
        
        // Calculate target position
        Vector3 targetPosition = startPosition + dashDirection * dashDistance;
        
        // Perform dash
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dashDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Check if player is still in range after the dash
        float distanceToPlayer = Vector3.Distance(transform.position, enemyBrain.Player.transform.position);
        if (distanceToPlayer <= dashDistance)
        {
            // Deal damage with variation
            PlayerHealth playerHealth = enemyBrain.Player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                float damage = GetRandomDamage();
                playerHealth.TakeDamage(damage);
            }
        }

        // Wait for attack cooldown
        yield return new WaitForSeconds(attackCooldown);
        
        // Reset the coroutine reference
        attackCoroutine = null;
    }

    private float GetRandomDamage()
    {
        return Mathf.Round(Random.Range(minDamage, maxDamage));
    }
}

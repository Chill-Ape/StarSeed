using UnityEngine;
using System.Collections;

public class ActionAttack : FSMAction
{
    [Header("Config")]
    [SerializeField] private float damage = 10f;  // Base damage
    [SerializeField] private float damageVariation = 0.25f;  // 25% variation
    [SerializeField] private float timeBtwAttacks;
    [SerializeField] private float windupDuration = 0.2f;  // How long to flash red
    [SerializeField] private Color windupColor = Color.red;  // The red flash color
    [SerializeField] private float dashDuration = 0.2f;  // How long the dash takes
    [SerializeField] private float dashDistance = 2f;    // How far to dash

    private EnemyBrain enemyBrain;
    private float timer;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isAttacking = false;
    private Vector3 originalPosition;
    
    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private float GetRandomDamage()
    {
        float minDamage = damage * (1f - damageVariation);
        float maxDamage = damage * (1f + damageVariation);
        return Mathf.Round(Random.Range(minDamage, maxDamage));
    }

    public override void Act()
    {
        if (!isAttacking)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (enemyBrain.Player == null) return;
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            StartCoroutine(WindupAttack());
        }
    }

    private IEnumerator WindupAttack()
    {
        isAttacking = true;
        originalPosition = transform.position;
        
        // Flash red
        if (spriteRenderer != null)
        {
            spriteRenderer.color = windupColor;
        }

        yield return new WaitForSeconds(windupDuration);

        // Calculate dash target
        Vector3 directionToPlayer = (enemyBrain.Player.transform.position - transform.position).normalized;
        Vector3 dashTarget = transform.position + (directionToPlayer * dashDistance);

        // Perform dash
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            float t = elapsedTime / dashDuration;
            // Use smoothstep for better movement
            float smoothT = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(originalPosition, dashTarget, smoothT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we end up exactly at the target position
        transform.position = dashTarget;

        // Check if player is still in range after the dash
        float distanceToPlayer = Vector3.Distance(transform.position, enemyBrain.Player.transform.position);
        if (distanceToPlayer <= dashDistance)
        {
            // Deal damage with variation
            IDamageable player = enemyBrain.Player.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(GetRandomDamage());
            }
        }
        else
        {
            Debug.Log("Player dodged out of range!");
        }

        timer = timeBtwAttacks;

        // Reset color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isAttacking = false;
    }
}

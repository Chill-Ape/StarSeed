using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")] 
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Weapon initialWeapon;
    [SerializeField] private Transform[] attackPositions;

    [Header("Melee Config")]
    [SerializeField] private ParticleSystem slashFX;
    [SerializeField] private float minDistanceMeleeAttack;
    [SerializeField] private float meleeAttackCooldown = 1f;

    [Header("Dodge Config")]
    [SerializeField] private float dodgeDistance = 1f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeHeight = 0.5f;
    private bool isDodging = false;
    private bool isBlocking = false;

    [Header("Block Config")]
    [SerializeField] private float blockDuration = 0.5f;
    [SerializeField] private float damageReduction = 0.75f;
    [SerializeField] private ParticleSystem blockEffect;

    public Weapon CurrentWeapon { get; set; }

    private PlayerActions actions;
    private PlayerAnimation playerAnimations;
    private PlayerMovement playerMovement;
    private PlayerMana playerMana;
    private EnemyBrain enemyTarget;
    private Coroutine attackCoroutine;
    private float lastAttackTime;

    private Transform currentAttackPosition;
    private float currentAttackRotation;

    public bool IsAttacking { get; private set; }

    private Coroutine blockCoroutine;

    private void Awake()
    {
        actions = new PlayerActions();
        playerMana = GetComponent<PlayerMana>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimations = GetComponent<PlayerAnimation>();
    }

    private void Start()
    {
        CurrentWeapon = initialWeapon;
        actions.Attack.ClickAttack.performed += ctx => Attack();

    }

    private void Update()
    {
        GetFirePosition();
    }

    public void Attack()
    {
        if (isDodging || isBlocking) return; // Can't attack while dodging or blocking
        
        if (Time.time - lastAttackTime < meleeAttackCooldown) return;
        
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        
        lastAttackTime = Time.time;
        IsAttacking = true;
        attackCoroutine = StartCoroutine(IEAttack());
    }

    private IEnumerator IEAttack()
    {
        IsAttacking = true;
        playerAnimations.SetAttackAnimation(true);
        
        // Perform the appropriate attack based on weapon type
        if (CurrentWeapon.WeaponType == WeaponType.Magic)
        {
            MagicAttack();
        }
        else
        {
            MeleeAttack();
        }
        
        // Wait for the attack animation to complete
        yield return new WaitForSeconds(0.5f);
        
        IsAttacking = false;
        playerAnimations.SetAttackAnimation(false);
    }

    private void MagicAttack()
    {
        if (CurrentWeapon == null)
        {
            Debug.LogError("CurrentWeapon is null!");
            return;
        }

        if (CurrentWeapon.ProjectilePrefab == null)
        {
            Debug.LogError($"ProjectilePrefab is not assigned to weapon: {CurrentWeapon.name}");
            return;
        }
        
        if (playerMana == null)
        {
            Debug.LogError("PlayerMana component is missing!");
            return;
        }
        
        if (playerMana.CurrentMana < CurrentWeapon.RequiredMana) return;
        
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, currentAttackRotation));
        Projectile projectile = Instantiate(CurrentWeapon.ProjectilePrefab, 
            currentAttackPosition.position, rotation);
        projectile.Direction = Vector3.up;
        projectile.Damage = GetAttackDamage();
        playerMana.UseMana(CurrentWeapon.RequiredMana);
    }

    private void MeleeAttack()
    {
        if (currentAttackPosition == null) return;
        if (slashFX == null) return;
        
        // Always play the slash effect
        slashFX.transform.position = currentAttackPosition.position;
        slashFX.Play();
        
        // Find the closest enemy in range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, minDistanceMeleeAttack);
        EnemyBrain closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            EnemyBrain enemy = collider.GetComponent<EnemyBrain>();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        if (closestEnemy == null) return;
        
        // Store the target position and damageable component before any operations
        Vector3 targetPosition = closestEnemy.transform.position;
        IDamageable damageable = closestEnemy.GetComponent<IDamageable>();
        
        float currentDistanceToEnemy = Vector3.Distance(targetPosition, transform.position);
        
        // Get the direction to the enemy
        Vector2 directionToEnemy = (targetPosition - transform.position).normalized;
        
        // Get the player's facing direction
        Vector2 facingDirection = Vector2.zero;
        switch (currentAttackRotation)
        {
            case 0f: // Up
                facingDirection = Vector2.up;
                break;
            case -90f: // Right
                facingDirection = Vector2.right;
                break;
            case -180f: // Down
                facingDirection = Vector2.down;
                break;
            case -270f: // Left
                facingDirection = Vector2.left;
                break;
        }
        
        // Check if player is facing the enemy (using dot product)
        float dotProduct = Vector2.Dot(facingDirection, directionToEnemy);
        bool isFacingEnemy = dotProduct > 0.5f; // This means within about 60 degrees
        
        if (currentDistanceToEnemy <= minDistanceMeleeAttack && isFacingEnemy)
        {
            if (damageable != null)
            {
                damageable.TakeDamage(GetAttackDamage());
            }
            
            // Apply knockback based on attack direction
            Vector3 knockbackDirection = Vector3.zero;
            
            // Determine primary knockback direction based on currentAttackRotation
            if (currentAttackRotation == -90f) // Right
            {
                knockbackDirection = new Vector3(1f, Random.Range(-1f, 2f), 0f);
            }
            else if (currentAttackRotation == -270f) // Left
            {
                knockbackDirection = new Vector3(-1f, Random.Range(-1f, 2f), 0f);
            }
            else if (currentAttackRotation == 0f) // Up
            {
                knockbackDirection = new Vector3(Random.Range(-1f, 2f), 1f, 0f);
            }
            else if (currentAttackRotation == -180f) // Down
            {
                knockbackDirection = new Vector3(Random.Range(-1f, 2f), -1f, 0f);
            }
            
            // Normalize the knockback direction to ensure consistent distance
            knockbackDirection.Normalize();
            
            // Apply the weapon's knockback force
            knockbackDirection *= CurrentWeapon.KnockbackForce;
            
            // Check if enemy still exists before applying knockback
            if (closestEnemy != null && closestEnemy.transform != null)
            {
                StartCoroutine(KnockbackCoroutine(closestEnemy, knockbackDirection));
            }
        }
    }

    private IEnumerator KnockbackCoroutine(EnemyBrain enemy, Vector3 knockbackDirection)
    {
        float knockbackDuration = 0.2f; // Duration of the knockback animation
        float elapsedTime = 0f;
        Vector3 startPosition = enemy.transform.position;
        Vector3 targetPosition = startPosition + knockbackDirection;

        while (elapsedTime < knockbackDuration)
        {
            if (enemy == null || enemy.transform == null) yield break;

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / knockbackDuration;
            
            // Use smoothstep for a more natural movement
            t = t * t * (3f - 2f * t);
            
            enemy.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Ensure the enemy ends up exactly at the target position
        if (enemy != null && enemy.transform != null)
        {
            enemy.transform.position = targetPosition;
        }
    }

    private float GetAttackDamage()
    {
        float damage = stats.BaseDamage;
        damage += CurrentWeapon.Damage;
        float randomPerc = Random.Range(0f, 100);
        if (randomPerc <= stats.CriticalChance)
        {
            damage += damage * (stats.CriticalDamage / 100f);
        }

        // Apply damage reduction if blocking
        if (isBlocking)
        {
            damage *= (1f - damageReduction);
        }

        return damage;
    }

    private void GetFirePosition()
    {
        Vector2 moveDirection = playerMovement.MoveDirection;
        switch (moveDirection.x)
        {
            case > 0f:
                currentAttackPosition = attackPositions[1];
                currentAttackRotation = -90f;
                break;
            case < 0f:
                currentAttackPosition = attackPositions[3];
                currentAttackRotation = -270f;
                break;
        }
        
        switch (moveDirection.y)
        {
            case > 0f:
                currentAttackPosition = attackPositions[0];
                currentAttackRotation = 0f;
                break;
            case < 0f:
                currentAttackPosition = attackPositions[2];
                currentAttackRotation = -180f;
                break;
        }
    }

    private void EnemySelectedCallback(EnemyBrain enemySelected)
    {
        enemyTarget = enemySelected;
    }

    private void NoEnemySelectionCallback()
    {
        enemyTarget = null;
    }

    private void OnEnable()
    {
        actions.Enable();
        SelectionManager.OnEnemySelectedEvent += EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent += NoEnemySelectionCallback;
        EnemyHealth.OnEnemyDeadEvent += NoEnemySelectionCallback;
    }

    private void OnDisable()
    {
        actions.Disable();
        SelectionManager.OnEnemySelectedEvent -= EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent -= NoEnemySelectionCallback;
        EnemyHealth.OnEnemyDeadEvent -= NoEnemySelectionCallback;
    }

    public void InitializeDodgeSettings(float distance, float duration, float height)
    {
        dodgeDistance = distance;
        dodgeDuration = duration;
        dodgeHeight = height;
    }

    public void InitializeBlockSettings(float duration, float reduction, ParticleSystem effect)
    {
        blockDuration = duration;
        damageReduction = reduction;
        blockEffect = effect;
    }

    public void Block()
    {
        if (isDodging) return; // Can't block while dodging
        
        isBlocking = true;
        if (blockEffect != null)
        {
            blockEffect.Play();
        }

        // Stop any existing block coroutine
        if (blockCoroutine != null)
        {
            StopCoroutine(blockCoroutine);
        }
        
        // Start new block coroutine
        blockCoroutine = StartCoroutine(BlockCoroutine());
    }

    private IEnumerator BlockCoroutine()
    {
        yield return new WaitForSeconds(blockDuration);
        isBlocking = false;
        if (blockEffect != null)
        {
            blockEffect.Stop();
        }
    }

    public void Dodge()
    {
        if (isDodging || isBlocking) return; // Can't dodge while already dodging or blocking
        
        Vector3 dodgeDirection = GetDodgeDirection();
        StartCoroutine(DodgeCoroutine(dodgeDirection));
    }

    private Vector3 GetDodgeDirection()
    {
        // Get the opposite direction of where the player is facing
        switch (currentAttackRotation)
        {
            case 0f: // Up
                return Vector3.down;
            case -90f: // Right
                return Vector3.left;
            case -180f: // Down
                return Vector3.up;
            case -270f: // Left
                return Vector3.right;
            default:
                return Vector3.zero;
        }
    }

    private IEnumerator DodgeCoroutine(Vector3 direction)
    {
        isDodging = true;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (direction * dodgeDistance);

        while (elapsedTime < dodgeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dodgeDuration;
            
            // Use smoothstep for a more natural movement
            t = t * t * (3f - 2f * t);
            
            // Add a parabolic arc to the movement
            float height = Mathf.Sin(t * Mathf.PI) * dodgeHeight;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
            currentPosition.y += height;
            
            transform.position = currentPosition;
            yield return null;
        }

        // Ensure we end up exactly at the target position
        transform.position = targetPosition;
        isDodging = false;
    }
}

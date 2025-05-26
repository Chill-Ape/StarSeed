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

    private void Attack()
    {
        if (enemyTarget == null) return;
        
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
        
        // Check if enemy still exists before attacking
        if (enemyTarget != null)
        {
            // Perform the appropriate attack based on weapon type
            if (CurrentWeapon.WeaponType == WeaponType.Magic)
            {
                MagicAttack();
            }
            else
            {
                MeleeAttack();
            }
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
        if (enemyTarget == null) return;
        if (enemyTarget.transform == null) return;
        if (currentAttackPosition == null) return;
        if (slashFX == null) return;
        
        // Store the target position and damageable component before any operations
        Vector3 targetPosition = enemyTarget.transform.position;
        IDamageable damageable = enemyTarget.GetComponent<IDamageable>();
        
        slashFX.transform.position = currentAttackPosition.position;
        slashFX.Play();
        
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
            
            // Check if enemy still exists before applying knockback
            if (enemyTarget != null && enemyTarget.transform != null)
            {
                enemyTarget.transform.position += knockbackDirection;
            }
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
}

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
        if (CurrentWeapon.ProjectilePrefab == null)
        {
            Debug.LogError($"ProjectilePrefab is not assigned to weapon: {CurrentWeapon.name}");
            return;
        }
        
        if (playerMana.CurrentMana < CurrentWeapon.RequiredMana) return;
        
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, currentAttackRotation));
        Projectile projectile = Instantiate(CurrentWeapon.ProjectilePrefab, 
            currentAttackPosition.position, rotation);
        projectile.Direction = GetFacingDirection();
        projectile.Damage = GetAttackDamage();
        playerMana.UseMana(CurrentWeapon.RequiredMana);
    }
    
    private void MeleeAttack()
    {
        if (enemyTarget == null) return;
        
        // Store the target position before any operations
        Vector3 targetPosition = enemyTarget.transform.position;
        
        slashFX.transform.position = currentAttackPosition.position;
        slashFX.Play();
        
        float currentDistanceToEnemy = Vector3.Distance(targetPosition, transform.position);
        
        // Calculate direction to enemy
        Vector2 directionToEnemy = (targetPosition - transform.position).normalized;
        float angleToEnemy = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
        if (angleToEnemy < 0) angleToEnemy += 360f;
        
        // Check if player is facing the enemy (within 45 degrees)
        bool isFacingEnemy = Mathf.Abs(Mathf.DeltaAngle(currentAttackRotation, angleToEnemy)) <= 45f;
        
        if (currentDistanceToEnemy <= minDistanceMeleeAttack && isFacingEnemy)
        {
            // Get the damageable component before applying knockback
            IDamageable damageable = enemyTarget.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(GetAttackDamage());
            }
            
            // Apply knockback based on attack direction
            Vector3 knockbackDirection = Vector3.zero;
            
            // Determine primary knockback direction based on currentAttackRotation
            if (currentAttackRotation == 0f) // Right
            {
                knockbackDirection = new Vector3(1f, Random.Range(-1f, 2f), 0f);
            }
            else if (currentAttackRotation == 180f) // Left
            {
                knockbackDirection = new Vector3(-1f, Random.Range(-1f, 2f), 0f);
            }
            else if (currentAttackRotation == 90f) // Up
            {
                knockbackDirection = new Vector3(Random.Range(-1f, 2f), 1f, 0f);
            }
            else if (currentAttackRotation == 270f) // Down
            {
                knockbackDirection = new Vector3(Random.Range(-1f, 2f), -1f, 0f);
            }
            
            // Normalize the knockback direction to ensure consistent distance
            knockbackDirection.Normalize();
            
            // Check if enemy still exists before applying knockback
            if (enemyTarget != null)
            {
                enemyTarget.transform.position += knockbackDirection;
            }
        }
    }

    private Vector2 GetFacingDirection()
    {
        // Convert currentAttackRotation to a direction vector
        float angleInRadians = (currentAttackRotation - 90f) * Mathf.Deg2Rad; // Subtract 90 to align with Unity's coordinate system
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
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
                currentAttackRotation = 0f; // Right
                break;
            case < 0f:
                currentAttackPosition = attackPositions[3];
                currentAttackRotation = 180f; // Left
                break;
        }
        
        switch (moveDirection.y)
        {
            case > 0f:
                currentAttackPosition = attackPositions[0];
                currentAttackRotation = 90f; // Up
                break;
            case < 0f:
                currentAttackPosition = attackPositions[2];
                currentAttackRotation = 270f; // Down
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
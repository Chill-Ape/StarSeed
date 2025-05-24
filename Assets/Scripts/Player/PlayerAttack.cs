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
        
        slashFX.transform.position = currentAttackPosition.position;
        slashFX.Play();
        
        float currentDistanceToEnemy = Vector3.Distance(enemyTarget.transform.position, transform.position);
        
        if (currentDistanceToEnemy <= minDistanceMeleeAttack)
        {
            enemyTarget.GetComponent<IDamageable>().TakeDamage(GetAttackDamage());
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
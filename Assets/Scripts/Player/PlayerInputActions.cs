using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputActions : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeDistance = 2f;
    [SerializeField] private float dodgeDuration = 0.5f;
    [SerializeField] private float dodgeHeight = 0.5f;

    [Header("Block Settings")]
    [SerializeField] private float damageReduction = 0.75f;
    [SerializeField] private ParticleSystem blockEffect;

    private InputAction clickAttackAction;
    private InputAction blockAction;
    private InputAction dodgeAction;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        // Create the input actions
        clickAttackAction = new InputAction("ClickAttack", binding: "<Mouse>/leftButton");
        blockAction = new InputAction("Block", binding: "<Mouse>/rightButton");
        dodgeAction = new InputAction("Dodge", binding: "<Keyboard>/leftShift");

        playerAttack = GetComponent<PlayerAttack>();
        
        // Pass the settings to PlayerAttack
        playerAttack.InitializeDodgeSettings(dodgeDistance, dodgeDuration, dodgeHeight);
        playerAttack.InitializeBlockSettings(damageReduction, blockEffect);
        
        // Debug log to check if block effect is assigned
        if (blockEffect == null)
        {
            Debug.LogWarning("Block effect is not assigned in PlayerInputActions!");
        }
        else
        {
            Debug.Log("Block effect is properly assigned");
        }
    }

    private void OnEnable()
    {
        // Enable the actions
        clickAttackAction.Enable();
        blockAction.Enable();
        dodgeAction.Enable();

        // Subscribe to the actions
        clickAttackAction.performed += ctx => playerAttack.Attack();
        blockAction.performed += ctx => playerAttack.StartBlock();
        blockAction.canceled += ctx => playerAttack.EndBlock();
        dodgeAction.performed += ctx => playerAttack.Dodge();
    }

    private void OnDisable()
    {
        // Unsubscribe from the actions
        clickAttackAction.performed -= ctx => playerAttack.Attack();
        blockAction.performed -= ctx => playerAttack.StartBlock();
        blockAction.canceled -= ctx => playerAttack.EndBlock();
        dodgeAction.performed -= ctx => playerAttack.Dodge();

        // Disable the actions
        clickAttackAction.Disable();
        blockAction.Disable();
        dodgeAction.Disable();
    }
} 
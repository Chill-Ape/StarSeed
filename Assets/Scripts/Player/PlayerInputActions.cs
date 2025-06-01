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

    // Store event handlers as fields to prevent allocations
    private System.Action<InputAction.CallbackContext> onAttackPerformed;
    private System.Action<InputAction.CallbackContext> onBlockPerformed;
    private System.Action<InputAction.CallbackContext> onBlockCanceled;
    private System.Action<InputAction.CallbackContext> onDodgePerformed;

    private void Awake()
    {
        // Create the input actions
        clickAttackAction = new InputAction("ClickAttack", binding: "<Keyboard>/space");
        blockAction = new InputAction("Block");
        blockAction.AddBinding("<Mouse>/rightButton");
        blockAction.AddBinding("<Keyboard>/slash");
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

        // Initialize event handlers
        onAttackPerformed = ctx => playerAttack.Attack();
        onBlockPerformed = ctx => playerAttack.StartBlock();
        onBlockCanceled = ctx => playerAttack.EndBlock();
        onDodgePerformed = ctx => playerAttack.Dodge();
    }

    private void OnEnable()
    {
        // Enable the actions
        clickAttackAction.Enable();
        blockAction.Enable();
        dodgeAction.Enable();

        // Subscribe to the actions using stored handlers
        clickAttackAction.performed += onAttackPerformed;
        blockAction.performed += onBlockPerformed;
        blockAction.canceled += onBlockCanceled;
        dodgeAction.performed += onDodgePerformed;
    }

    private void OnDisable()
    {
        // Unsubscribe from the actions using stored handlers
        clickAttackAction.performed -= onAttackPerformed;
        blockAction.performed -= onBlockPerformed;
        blockAction.canceled -= onBlockCanceled;
        dodgeAction.performed -= onDodgePerformed;

        // Disable the actions
        clickAttackAction.Disable();
        blockAction.Disable();
        dodgeAction.Disable();
    }
} 
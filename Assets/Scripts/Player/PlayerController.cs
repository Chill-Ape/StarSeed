using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    [Header("Combat")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;
    
    // Animation parameters
    private readonly int moveX = Animator.StringToHash("MoveX");
    private readonly int moveY = Animator.StringToHash("MoveY");
    private readonly int moving = Animator.StringToHash("Moving");
    
    // Farming Tools
    public enum ToolType
    {
        plough,
        wateringCan,
        seeds,
        basket
    }

    [Header("Farming Tools")]
    public ToolType currentTool;
    public float toolWaitTime = 0.5f;
    private float toolWaitCounter;
    public Transform toolIndicator;
    public float toolRange = 3f;
    public CropController.CropType seedCropType;
    public InputActionReference actionInput;
    
    public Vector2 movement { get; private set; }
    private bool isAttacking;
    private bool isUsingTool;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        if (UIController.instance != null)
        {
            UIController.instance.SwitchTool((int)currentTool);
        }
    }
    
    private void Update()
    {
        // Get movement input
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        // Handle tool switching
        bool hasSwitchedTool = false;
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            currentTool++;
            if ((int)currentTool >= 4) currentTool = ToolType.plough;
            hasSwitchedTool = true;
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) { currentTool = ToolType.plough; hasSwitchedTool = true; }
            if (Keyboard.current.digit2Key.wasPressedThisFrame) { currentTool = ToolType.wateringCan; hasSwitchedTool = true; }
            if (Keyboard.current.digit3Key.wasPressedThisFrame) { currentTool = ToolType.seeds; hasSwitchedTool = true; }
            if (Keyboard.current.digit4Key.wasPressedThisFrame) { currentTool = ToolType.basket; hasSwitchedTool = true; }
        }

        if (hasSwitchedTool && UIController.instance != null)
        {
            UIController.instance.SwitchTool((int)currentTool);
        }

        // Update tool indicator position
        if (GridController.instance != null && toolIndicator != null && Camera.main != null)
        {
            toolIndicator.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            toolIndicator.position = new Vector3(toolIndicator.position.x, toolIndicator.position.y, 0f);

            if (Vector3.Distance(toolIndicator.position, transform.position) > toolRange)
            {
                Vector2 direction = toolIndicator.position - transform.position;
                direction = direction.normalized * toolRange;
                toolIndicator.position = transform.position + new Vector3(direction.x, direction.y, 0f);
            }

            toolIndicator.position = new Vector3(
                Mathf.FloorToInt(toolIndicator.position.x) + 0.5f,
                Mathf.FloorToInt(toolIndicator.position.y) + 0.5f,
                0f
            );
        }
        else if (toolIndicator != null)
        {
            toolIndicator.position = new Vector3(0f, 0f, -20f);
        }

        // Handle tool use
        if (actionInput != null && actionInput.action != null && actionInput.action.WasPressedThisFrame() && toolWaitCounter <= 0)
        {
            UseTool();
        }

        // Handle attack input (right mouse button for combat)
        if (Input.GetMouseButtonDown(1) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        
        // Update animations
        if (animator != null)
        {
            animator.SetFloat(moveX, movement.x);
            animator.SetFloat(moveY, movement.y);
            animator.SetBool(moving, movement.magnitude > 0);
        }
        
        // Flip sprite based on movement
        if (movement.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = movement.x < 0;
        }

        // Update tool cooldown
        if (toolWaitCounter > 0)
        {
            toolWaitCounter -= Time.deltaTime;
        }
    }
    
    private void FixedUpdate()
    {
        // Move the player
        rb.linearVelocity = movement * moveSpeed;
    }
    
    private void Attack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;
        
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Perform attack logic here
        // You can add raycasting or collider checks to detect enemies
    }
    
    void UseTool()
    {
        GrowBlock block = GridController.instance.GetBlock(toolIndicator.position.x - 0.5f, toolIndicator.position.y - 0.5f);
        toolWaitCounter = toolWaitTime;

        if (block != null)
        {
            switch (currentTool)
            {
                case ToolType.plough:
                    block.PloughSoil();
                    break;
                case ToolType.wateringCan:
                    block.WaterSoil();
                    break;
                case ToolType.seeds:
                    if(CropController.instance.GetCropInfo(seedCropType).seedAmount > 0)
                    {
                        block.PlantCrop(seedCropType);
                    }
                    break;
                case ToolType.basket:
                    block.HarvestCrop();
                    break;
            }
        }
    }

    public void SwitchSeed(CropController.CropType newSeed)
    {
        seedCropType = newSeed;
    }
    
    public void HardFreeze()
    {
        rb.linearVelocity = Vector2.zero;
        enabled = false;
    }
    
    public void Unfreeze()
    {
        enabled = true;
    }
}
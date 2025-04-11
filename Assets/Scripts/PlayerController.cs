using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;


    public Rigidbody2D theRB;
    public float moveSpeed;

    public InputActionReference moveInput, actionInput;

    public Animator anim;

    public enum ToolType
    {
        plough,
        wateringCan,
        seeds,
        basket
    }

    public ToolType currentTool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIController.instance.SwitchTool((int)currentTool);
    }

    // Update is called once per frame
    void Update()
    {
        // theRB.linearVelocity = new Vector2(moveSpeed, 0f);
        theRB.linearVelocity = moveInput.action.ReadValue<Vector2>().normalized * moveSpeed;

        if(theRB.linearVelocity.x <0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if(theRB.linearVelocity.x >0f)
        {
            transform.localScale = Vector3.one;
        }

        bool hasSwitchedTool = false;

        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            currentTool ++;

            if((int)currentTool >= 4)
            {
                currentTool = ToolType.plough;
            }
            hasSwitchedTool = true;
        }

        if(Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            currentTool = ToolType.plough;
            hasSwitchedTool = true;
        }

        if(Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            currentTool = ToolType.wateringCan;
            hasSwitchedTool = true;
        }

         if(Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            currentTool = ToolType.seeds;
            hasSwitchedTool = true;
        }

         if(Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            currentTool = ToolType.basket;
            hasSwitchedTool = true;
        }

        if (hasSwitchedTool == true)
        {
            //FindFirstObjectByType<UIController>().SwitchTool((int)currentTool);

            UIController.instance.SwitchTool((int)currentTool);
        }


         

        if(actionInput.action.WasPressedThisFrame())
        {
            UseTool();

            void UseTool()
            {
                GrowBlock block = null;

                block = FindFirstObjectByType<GrowBlock>();

                //block.PloughSoil();

                if(block != null)
                {
                    switch(currentTool)
                    {
                        case ToolType.plough:

                        block.PloughSoil();

                        break;

                        case ToolType.wateringCan:

                        break;

                        case ToolType.seeds:

                        break;

                        case ToolType.basket:

                        break;
                    }
                }
            }
        }


        anim.SetFloat("speed", theRB.linearVelocity.magnitude);
    }

        private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

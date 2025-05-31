using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;  // <--- This is crucial!

    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    [Header("Test")]
    public ItemHealthPotion HealthPotion;
    public ItemManaPotion ManaPotion;
    public PlayerMana PlayerMana { get; private set; }
    public PlayerHealth PlayerHealth { get; private set; }
    public PlayerStats Stats => stats;
    public PlayerAttack PlayerAttack { get; private set; }

    private PlayerAnimation playerAnimation;
    

    private void Awake()
    {
        PlayerMana = GetComponent<PlayerMana>();
        PlayerHealth = GetComponent<PlayerHealth>();
        PlayerAttack = GetComponent<PlayerAttack>();
        playerAnimation = GetComponent<PlayerAnimation>();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //playerAnimations = GetComponent<PlayerAnimation>();
    }

   
   
    private void Update()
 {
     if (Input.GetKeyDown(KeyCode.T))
     {
         if (HealthPotion.UseItem())
         {
             Debug.Log("Using Health Potion");
         }
         
         if (ManaPotion.UseItem())
         {
             Debug.Log("Using Mana Potion");
         }
     }
 }
   
   

    public void ResetPlayer()
    {
        stats.ResetPlayer();
        playerAnimation.ResetAllTriggers();
        PlayerMana.ResetMana();
    }

    public void SwitchSeed(CropController.CropType newSeed)
    {
        // You can implement this when you add crop selection/planting logic
        // For now, this just makes the code compile
    }

    // Freezes the player (e.g., to disable movement/input during scene transitions or menus)
    public void HardFreeze()
    {
        // You can implement this to stop all player movement/input
        // For now, this just makes the code compile
    }

    // Unfreezes the player (restores movement/input)
    public void Unfreeze()
    {
        // You can implement this to re-enable player control
        // For now, this just makes the code compile
    }
}

using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;  // <--- This is crucial!

    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    public PlayerMana PlayerMana { get; private set; }
    public PlayerStats Stats => stats;

    private PlayerAnimation playerAnimations;
    private PlayerHealth playerHealth;

    private void Awake()
    {
         PlayerMana = GetComponent<PlayerMana>();
         playerAnimations = GetComponent<PlayerAnimation>();
         playerHealth = GetComponent<PlayerHealth>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        playerAnimations = GetComponent<PlayerAnimation>();
    }

    public void ResetPlayer()
    {
        stats.ResetPlayer();
        playerHealth.ResetPlayer();
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

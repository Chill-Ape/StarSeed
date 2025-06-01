using UnityEngine;

public class EnemySelector : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GameObject selectorSprite;

    private EnemyBrain enemyBrain;
    private EnemyHealth enemyHealth;
    private EnemyLoot enemyLoot;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyLoot = GetComponent<EnemyLoot>();
    }

    private void EnemySelectedCallback(EnemyBrain enemySelected)
    {
        if (enemySelected == enemyBrain)
        {
            selectorSprite.SetActive(true);
            
            // If enemy is dead, show loot panel
            if (enemyHealth.CurrentHealth <= 0f)
            {
                LootManager.Instance.ShowLoot(enemyLoot);
            }
        }
        else
        {
            selectorSprite.SetActive(false);
        }
    }

    public void NoSelectionCallback()
    {
        selectorSprite.SetActive(false);
    }
    
    private void OnEnable()
    {
        SelectionManager.OnEnemySelectedEvent += EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent += NoSelectionCallback;
    }

    private void OnDisable()
    {
        SelectionManager.OnEnemySelectedEvent -= EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent -= NoSelectionCallback;
    }
}
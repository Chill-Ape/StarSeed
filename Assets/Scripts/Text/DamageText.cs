using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TextMeshProUGUI damageTMP;

    private void Awake()
    {
        
    }

    public void SetDamageText(float damage)
    {
        if (damageTMP == null)
        {
            Debug.LogError("[DamageText] TextMeshProUGUI component is null when trying to set damage text!");
            return;
        }
        
        damageTMP.text = damage.ToString();
      
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
}

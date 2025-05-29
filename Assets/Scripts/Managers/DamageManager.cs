using UnityEngine;
using TMPro;

public class DamageManager : Singleton<DamageManager>
{

    public DamageText damageTextPrefab;


    public void ShowDamageText(float damageAmount, Transform parent, bool isPlayerDamage = false)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogError("[DamageManager] Damage Text Prefab is not assigned!");
            return;
        }

        DamageText text = Instantiate(damageTextPrefab, parent.position, Quaternion.identity);
        
        Canvas canvas = text.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1f;
            canvas.transform.forward = Camera.main.transform.forward;
            canvas.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            TextMeshProUGUI tmp = text.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.fontSize = 36;
                tmp.color = isPlayerDamage ? Color.white : Color.red;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
            }
        }

        text.transform.position += Vector3.up * 1.5f;
        text.SetDamageText(damageAmount);
    }
}
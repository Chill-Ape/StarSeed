using UnityEngine;
using TMPro;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set; }
    public DamageText damageTextPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowDamageText(float damageAmount, Transform parent)
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
                tmp.color = Color.red;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
            }
        }

        text.transform.position += Vector3.up * 1.5f;
        text.SetDamageText(damageAmount);
    }
}
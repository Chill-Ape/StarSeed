using UnityEngine;

public class InventoryController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
public void OpenClose()
{
    if(gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        } else{
            gameObject.SetActive(false);
        }
    }
}


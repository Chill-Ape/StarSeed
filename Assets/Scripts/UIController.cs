using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance; 
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);

        } else
        {
            Destroy(gameObject);
        }

    }

    public GameObject [] toolbarActivatorIcons;

    public TMP_Text timeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SwitchTool(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchTool(int selected)
    {
        foreach(GameObject icon in toolbarActivatorIcons)
        {
            icon.SetActive(false);
        }
        toolbarActivatorIcons[selected].SetActive(true);
    }

    public void UpdateTimeText(float currentTime)
    {
        if(currentTime < 12)
        {
            timeText.text = Mathf.FloorToInt(currentTime) + "AM";
        } else if(currentTime < 13)
        {
           timeText.text = "12:PM";
        } else if(currentTime < 24)
        {
            timeText.text = (Mathf.FloorToInt(currentTime) - 12) + "PM";
        } else if(currentTime < 25)
        {
            timeText.text = "12AM";
        } else
        {
            timeText.text = Mathf.FloorToInt(currentTime -24) + "AM";
        }
    }
}

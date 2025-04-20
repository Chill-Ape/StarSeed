using UnityEngine;

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
}

using UnityEngine;

public class TimeController : MonoBehaviour
{

    public static TimeController instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public float currentTime; 

    public float dayStart, dayEnd;

    public float timeSpeed = .25f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = dayStart;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime * timeSpeed;

        if(currentTime > dayEnd)
        {
            currentTime = dayEnd;
        }

        if(UIController.instance != null)
        {
            UIController.instance.UpdateTimeText(currentTime);
        }

    } 
}

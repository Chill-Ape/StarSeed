using UnityEngine;

public class AreaSwitcher : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public string sceneToLoad;
    public string transitionName;

    [Header("Optional Start Point")]
    public Transform startPoint;

    [Header("Player Components")]
    public Player farmingController;

    void Start()
    {
        if (PlayerPrefs.HasKey("Transition") && Player.instance != null)
        {
            if (PlayerPrefs.GetString("Transition") == transitionName && startPoint != null)
            {
                Player.instance.transform.position = startPoint.position;
                PlayerPrefs.DeleteKey("Transition");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPrefs.SetString("Transition", transitionName);

            SceneLoader loader = FindObjectOfType<SceneLoader>();
            if (loader != null)
            {
                loader.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("SceneLoader not found in the scene!");
            }
        }
    }
}

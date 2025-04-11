using UnityEngine;
using UnityEngine.SceneManagement; // Required for SceneManager

public class AreaSwitcher : MonoBehaviour
{
    public string sceneToLoad;

    //public Transform startPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
    {
       //PlayerController.instance.transform.position = startPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //Debug.Log("player enters");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

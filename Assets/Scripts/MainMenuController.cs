using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

   public string levelToStart;

   private void Start()
   {
    AudioManager.instance.PlayTitle();
   }

   public void PlayGame()
   {
        SceneManager.LoadScene(levelToStart);

        AudioManager.instance.PlayNextBGM();
   }

   public void QuitGame()
   {
        Application.Quit();

        Debug.Log("Quitting the Game");
   }

  
}

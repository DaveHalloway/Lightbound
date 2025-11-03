using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Custom Methods
    // Loads the given game scene
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Quits the application
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}

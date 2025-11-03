using UnityEngine;
using UnityEngine.Video;   
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    #region Variables
    public VideoPlayer videoPlayer;
    public Button SkipButton;
    public string tutorialSceneName = "Tutorial";
    #endregion

    #region Unity Methods
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        videoPlayer.Play();
        SkipButton.onClick.AddListener(SkipCutscene);
        videoPlayer.loopPointReached += OnVideoEnd;
    }
    #endregion

    #region Custom Methods
    void OnVideoEnd(VideoPlayer vp)
    {
        LoadTutorial();
    }

    void SkipCutscene()
    {
        videoPlayer.Stop();
        LoadTutorial();
    }

    void LoadTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }
    #endregion
}

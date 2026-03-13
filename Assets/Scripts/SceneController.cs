using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Loads the scene specified by the nextScene parameter.
    /// </summary>
    /// <param name="nextScene"></param>
    public void ChangeScene(string nextScene)
    {
        //We reset the timeScale.
        Time.timeScale = 1;
        //We load the indicated scene.
        SceneManager.LoadScene(nextScene);
    }
    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

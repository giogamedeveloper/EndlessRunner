using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
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
    
}

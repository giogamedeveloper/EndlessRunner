using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    //Scene to load after splash time ends.
    public string sceneAfterSplash = "MainMenu";
    [Range(0, 5)]
    public float splashDuration = 3f;
    //Reference to the Fade image.
    public Image fadeImage;
    //Gradient to configure the fade effect.
    public Gradient fadeGradient;

    private float _timer = 0f;

    private void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        //We increase the time in each frame.
        _timer += Time.deltaTime;
        //We evaluate the color that the fade image should have according to the gradient and the elapsed time.
        fadeImage.color = fadeGradient.Evaluate(_timer / splashDuration);
        //If time has already come to an end.
        if (_timer > splashDuration)
        {
            //We load the indicated scene.
            SceneManager.LoadScene(sceneAfterSplash);
        }
    }
}

using System.Collections;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    private Coroutine colorFlashCorutine;
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    /// <summary>
    /// Assigns the flash color and starts the recovery coroutine for the specified time.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="time"></param>
    public void StartColorFlash(Color color, float time)
    {
        //First we set the color of the flash.
        spriteRenderer.color = color;
        // If there was already a previous color, we cut the coroutine.
        if (colorFlashCorutine != null) StopCoroutine(colorFlashCorutine);
        //We start the routine that gradually recovers the color according to the indicated time.
        colorFlashCorutine = StartCoroutine(ColorRecover(time));
    }
    /// <summary>
    /// Restores the original color in the indicated time.
    /// </summary>
    /// <param name="tiempo"></param>
    /// <returns></returns>
    private IEnumerator ColorRecover(float time)
    {
        //We create a variable to count the recovery time.
        float counter = 0f;
        //We store the color before starting.
        Color StartColor = spriteRenderer.color;
        //As time runs out we will repeat the loop without stopping.
        while (counter < time)
        {
            spriteRenderer.color = Color.Lerp(StartColor, Color.white, counter / time);
            counter += Time.deltaTime;
            //We wait until the current frame ends.
            yield return new WaitForEndOfFrame();
        }
    }

}

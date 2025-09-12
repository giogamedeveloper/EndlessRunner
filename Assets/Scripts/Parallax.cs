using UnityEngine;

public class Parallax : MonoBehaviour
{
    #region Variabls
    //Bottom velocity where the maximum value equals the camera velocity.
    [Range(0f, .1f)]
    public float speedFactor = .066f;
    //Position to control the offset of the texture.
    private Vector2 pos = Vector2.zero;
    //Previous position of the camera.
    private Vector2 camLastPosition;
    //Reference to the Main camera.
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Renderer rend;
    #endregion

    #region Unity Events
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (rend == null) rend = GetComponent<Renderer>();
    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        camLastPosition = cam.transform.position;
        //We start the camera position.
        Vector2 backgroundHalfSize = new Vector2((cam.orthographicSize * Screen.width) / Screen.height, cam.orthographicSize);
        //We scaled the background to fit the screen size.
        transform.localScale = new Vector3(backgroundHalfSize.x * 2f, backgroundHalfSize.y * 2f, transform.localScale.z);
        //We adjust the tilling so that it is correctly proportioned to the scale.
        //We leave it halfway to reduce the number of repetitions as it offers a more aesthetic result.
        // rend.material.SetTextureScale("_MainText", backgroundHalfSize); //ver esto
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

        //Calculate the camera displacement relative to the previous frame.
        Vector2 camVariation = new Vector2(cam.transform.position.x - camLastPosition.x,
                                            cam.transform.position.y - camLastPosition.y);
        //We modify the offset that is applied to the texture.
        pos.x += camVariation.x * speedFactor;
        pos.y += camVariation.y * speedFactor;
        //We apply the offset to the main texture.
        rend.material.mainTextureOffset = pos;
        //We update the last camera position.
        camLastPosition = cam.transform.position;


    }
    #endregion
}

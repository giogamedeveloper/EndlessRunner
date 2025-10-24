using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    [Header("Variabls")]
    //Transform of the object to follow, it is a value by reference so we will always get the updated value.
    public Transform targer;
    //Indicates whether the camera will track on the X axis.
    public bool isFollowX = true;
    //Deviation of the position in x.
    [Range(0, 100)]
    public float offsetX = 1;


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>

    void Update()
    {
        Vector3 nextPosition = transform.position;
        if (isFollowX) nextPosition.x = targer.position.x + offsetX;
        transform.position = nextPosition;
    }
}

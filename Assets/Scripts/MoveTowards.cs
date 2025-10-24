using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [Range(-2, 2)]
    public float velocityX;
    [Range(-2, 2)]
    public float velocityY;

    private Vector3 tempVector;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        tempVector = Vector3.zero;
        tempVector.x = transform.position.x + velocityX * Time.deltaTime;
        tempVector.y = transform.position.y + velocityY * Time.deltaTime;
        tempVector.z = transform.position.z;
        transform.position = tempVector;
    }
}

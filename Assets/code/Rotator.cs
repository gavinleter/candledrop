using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed = 5f;

    void Update()
    {
        // Rotate the GameObject smoothly clockwise
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }
}

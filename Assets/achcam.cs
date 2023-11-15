using UnityEngine;

public class achcam : MonoBehaviour
{
    public bool inAchievementsPage = true;
    public Camera draggedCamera;
    public float dragStrength = 0.1f;

    private Vector2 lastMousePosition;

    void Update()
    {
        if (!inAchievementsPage)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePosition;
            MoveCamera(delta.y);
            lastMousePosition = Input.mousePosition;
        }
    }

    void MoveCamera(float deltaY)
    {
        if (draggedCamera == null)
        {
            Debug.LogWarning("No camera assigned for dragging.");
            return;
        }

        // Adjust the camera movement speed using dragStrength
        float moveAmount = -deltaY * dragStrength;

        // Apply the movement to the assigned camera's position
        draggedCamera.transform.Translate(Vector3.up * moveAmount);
    }
}

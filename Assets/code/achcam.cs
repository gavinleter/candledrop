using UnityEngine;

public class achcam : MonoBehaviour
{
    bool inAchievementsPage = false;
    [SerializeField] Camera draggedCamera;
    [SerializeField] float dragStrength;

    private Vector2 lastMousePosition;

    float upperBound = 0;
    float lowerBound = 0;

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

        if(draggedCamera.transform.position.y > upperBound) {
            draggedCamera.transform.position = new Vector3(draggedCamera.transform.position.x, upperBound, draggedCamera.transform.position.z);
        }
        if (draggedCamera.transform.position.y < lowerBound) {
            draggedCamera.transform.position = new Vector3(draggedCamera.transform.position.x, lowerBound, draggedCamera.transform.position.z);
        }
    }


    public void setActive(bool x) {
        inAchievementsPage = x;
    }


    public void setBounds(float upper, float lower) {
        lowerBound = lower;
        upperBound = upper;
    }
}

using UnityEngine;

public class achcam : MonoBehaviour
{
    bool inAchievementsPage = false;
    [SerializeField] Camera draggedCamera;
    [SerializeField] float dragStrength;

    float inertia = 0f;
    private Vector2 lastMousePosition;

    float upperBound = 0;
    float lowerBound = 0;


    void Start() {

    }

    void Update()
    {
        //do nothing if not in achievements menu or if we are currently transitioning to achievements menu
        if (!inAchievementsPage || draggedCamera.GetComponent<camCtrl>().currentlyTransitioning()) {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            Vector2 delta;
            if (Input.GetMouseButton(0)) {
                delta = (Vector2)Input.mousePosition - lastMousePosition;
            }
            else {
                delta = Vector2.zero;
            }
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
        inertia += moveAmount * 0.1f;
        inertia = Mathf.Lerp(inertia, 0f, 0.05f);

        // Apply the movement to the assigned camera's position
        draggedCamera.transform.Translate(Vector3.up * (moveAmount + inertia));

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

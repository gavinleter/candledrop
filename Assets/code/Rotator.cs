using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 5f;

    void Update(){
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    private void Start()
    {
        // Store the initial rotation when the script starts
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // Reset the rotation to the initial rotation every frame
        transform.rotation = initialRotation;
    }
}

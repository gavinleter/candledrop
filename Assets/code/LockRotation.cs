using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    Quaternion initialRotation;

    private void Start(){
        initialRotation = transform.rotation;
    }

    private void LateUpdate(){
        transform.rotation = initialRotation;
    }
}

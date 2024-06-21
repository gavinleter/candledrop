using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeed : MonoBehaviour
{

    [SerializeField] float speed;

    void Start(){

        GetComponent<Animator>().speed = speed;
    }

}
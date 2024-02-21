using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animspeed : MonoBehaviour
{
    Animator m_Animator;
    [SerializeField]float speed;

    void Start()
    {
        //Get the animator, attached to the GameObject you are intending to animate.
        m_Animator = gameObject.GetComponent<Animator>();
        m_Animator.speed = speed;
    }

}
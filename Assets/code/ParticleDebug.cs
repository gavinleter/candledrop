using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<ParticleSystem>().isPlaying) {
            Debug.Log("Playing");
        }
        if(GetComponent<ParticleSystem>().isPaused || GetComponent<ParticleSystem>().isStopped) {
            Debug.Log("not playing");
        }
    }
}

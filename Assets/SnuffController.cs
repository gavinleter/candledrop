using UnityEngine;
using System.Collections;

public class SnuffController : MonoBehaviour
{
    bool candleEnabled = true;
    GameObject flickerObject;
    int layer;

    void Start()
    {
        flickerObject = transform.GetChild(0).gameObject;
        layer = LayerMask.NameToLayer("flame");
    }

    void OnCollisionEnter(Collision collision)
    {   
        Debug.Log(layer + " " + collision.gameObject.layer);
        if(candleEnabled && collision.gameObject.layer != layer){
            candleEnabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            flickerObject.GetComponent<SpriteRenderer>().enabled = false;
        } else if(!candleEnabled && collision.gameObject.layer == layer){
            candleEnabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
            flickerObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleIgniter : MonoBehaviour
{
    bool isActive = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        CandleLightController other = collision.gameObject.GetComponent<CandleLightController>();

        if (other != null && other.canIgnite() && isActive) {
            other.enableLight();
        }
    }

    public void setActive(bool x) {
        isActive = x;
    }
}

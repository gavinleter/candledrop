using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSunExplosion : MonoBehaviour
{

    [SerializeField] float maxRadius;
    float lerp = 0;

    CircleCollider2D c;

    void Start() {
        
        c = GetComponent<CircleCollider2D>();

    }


    void Update() {
        
        c.radius = Mathf.Lerp(0, maxRadius, lerp);
        lerp += Time.deltaTime;

    }


    private void OnTriggerEnter2D(Collider2D collision) {

        //if touching a candle light, ignite it if it is able to
        CandleLightController can = collision.GetComponent<CandleLightController>();

        if (can != null && can.canIgnite()) {
            can.enableLight();
        }

    }

}

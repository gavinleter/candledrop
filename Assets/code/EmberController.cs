using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmberController : MonoBehaviour
{

    [SerializeField] float sizeRange;
    Rigidbody2D rb;
    int bouncesRemaining = 4;
    float startTime;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        startTime = Time.time;

        float changeInSize = UnityEngine.Random.Range(-sizeRange, sizeRange);

        transform.localScale = transform.localScale + new Vector3(changeInSize, changeInSize, 0);
        rb.angularVelocity = UnityEngine.Random.Range(-360, 360);
        rb.linearVelocity = new Vector2(UnityEngine.Random.Range(-2, 2), 3);

    }


    void Update() {
        
        //failsafe if the ember gets stuck and doesnt bounce
        if(startTime + 4 < Time.time) {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        CandleLightController other = collision.GetComponent<CandleLightController>();

        //do nothing if hitting another ember or the candle igniter area
        if (collision.GetComponent<EmberController>() != null || collision.GetComponent<CandleIgniter>() != null) {
            return;
        }

        if (other != null && other.canIgnite()) {
            other.enableLight();
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x * -1.5f, rb.linearVelocity.y);
        bouncesRemaining--;

        if (bouncesRemaining <= 0) {
            Destroy(gameObject);
        }

    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] GameObject holeExplodePrefab;
    GameManager gameManagerScript;

    public void setup(GameManager g) {
        gameManagerScript = g;
    }

    private void OnCollisionEnter2D(Collision2D other){
        CandleLightController o = other.gameObject.GetComponentInChildren<CandleLightController>();

        //check if the colliding object is a candle
        if (o != null){
            gameManagerScript.destroyCandle(o.getId());

        }

        destroySelf();

    }


    void destroySelf() {
        GameObject holeExplode = Instantiate(holeExplodePrefab, transform.position, Quaternion.identity);
        Destroy(holeExplode, 2f); // Destroy the explosion effect after 2 seconds
        Destroy(gameObject);
    }
}

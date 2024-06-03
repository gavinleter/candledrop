using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour, ISpecialObject
{
    [SerializeField] GameObject holeExplodePrefab;
    GameManager gameManagerScript;
    int id = 0;

    public void setup(GameManager g, int id) {
        gameManagerScript = g;
        this.id = id;
    }

    private void OnCollisionEnter2D(Collision2D other){
        CandleLightController o = other.gameObject.GetComponentInChildren<CandleLightController>();

        //check if the colliding object is a candle
        if (o != null){
            gameManagerScript.destroyCandle(o.getId());

        }

        destroySelf();

    }


    public void destroySelf() {

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, gameManagerScript.blackHoleDestroyRadius, Vector2.one);

        for (int i = 0; i < hits.Length; i++) {
            CandleLightController o = hits[i].collider.gameObject.GetComponentInChildren<CandleLightController>();
            if (o != null) {
                gameManagerScript.destroyCandle(o.getId());
            }
        }

        GameObject holeExplode = Instantiate(holeExplodePrefab, transform.position, Quaternion.identity);
        Destroy(holeExplode, 2f); // Destroy the explosion effect after 2 seconds
        gameManagerScript.removeSpecialObject(id);
        Destroy(gameObject);
    }


    public int getId() {
        return id;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public GameObject holeExplodePrefab;
    public List<GameObject> candles;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is in the list of candles
        if (candles.Contains(other.gameObject))
        {
            Debug.Log(other.gameObject.name);
            Destroy(other.gameObject); // Destroy the object it collided with
        }

        // Instantiate and play the hole explode particle system at the black hole's position
        GameObject holeExplode = Instantiate(holeExplodePrefab, transform);

        // Destroy the black hole object
        Destroy(gameObject);
    }
}

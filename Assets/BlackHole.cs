using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public GameObject holeExplodePrefab;
    public List<GameObject> blackHoleDiesOn; // List of objects that cause the black hole to explode

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is in the list of objects that cause the black hole to explode
        if (blackHoleDiesOn.Contains(other.gameObject))
        {
            Debug.Log(other.gameObject.name);
            // Instantiate and play the hole explode particle system at the black hole's position
            GameObject holeExplode = Instantiate(holeExplodePrefab, transform.position, Quaternion.identity);
            Destroy(holeExplode, 2f); // Destroy the explosion effect after 2 seconds

            // Destroy the black hole object
            Destroy(gameObject);
        }
    }
}

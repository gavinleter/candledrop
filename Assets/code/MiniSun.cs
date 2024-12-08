using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSun : MonoBehaviour, ISpecialObject
{

    [SerializeField] GameObject ExplosionPrefab;
    GameManager gameManager;
    int id = 0;


    public void setup(GameManager gameManager, int id) {
        this.gameManager = gameManager;
        this.id = id;
    }


    private void OnCollisionEnter2D(Collision2D other) { 
        
        GameObject x = Instantiate(ExplosionPrefab, new Vector3(transform.position.x, transform.position.y, -0.99f), transform.rotation);
        Destroy(x, 2.5f);
        destroySelf();

    }


    public void destroySelf(){

        gameManager.removeSpecialObject(id);
        Destroy(gameObject);

    }


    public int getType() {
        return (int)specialObjectType.miniSun;
    }


    public GameObject getGameObject() {
        return gameObject;
    }


}

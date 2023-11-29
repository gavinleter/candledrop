using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWall : MonoBehaviour
{

    List<int> touching = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter2D(Collider2D collision) {
        CandleIgniter o = collision.gameObject.GetComponent<CandleIgniter>();

        if (o != null) {

            CandleLightController other = o.getParentCandleScript();
            //keep track of any candle that comes into contact with this one
            addToList(other);

        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        CandleIgniter o = collision.gameObject.GetComponent<CandleIgniter>();

        if (o != null) {

            CandleLightController other = o.getParentCandleScript();
            //keep track of any candle that comes into contact with this one
            removeFromList(other);

        }
    }




    //need to keep track of all other candle ids that are touching the wall
    public void addToList(CandleLightController other) {
        //make sure this candle isnt already in the list
        if (findIndex(other.getId()) == -1) {
            touching.Add(other.getId());
        }
    }

    public void removeFromList(CandleLightController other) {
        //make sure this candle is in the list
        int index = findIndex(other.getId());
        if (index != -1) {
            touching.RemoveAt(index);
        }
    }


    private int findIndex(int id) {
        for (int i = 0; i < touching.Count; i++) {
            if (touching[i] == id) {
                return i;
            }
        }
        return -1;
    }


    public bool containsId(int id) {
        return findIndex(id) != -1;
    }
}

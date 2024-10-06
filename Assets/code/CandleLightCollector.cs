using System.Collections.Generic;
using UnityEngine;

public class CandleLightCollector : MonoBehaviour
{
    
    public readonly List<Collider2D> touching = new List<Collider2D>();
    protected int touchingLength = 0;
    List<string> touchingHistory = new List<string>();

    protected Collider2D coll;
    protected ContactFilter2D contactFilter;


    private void Awake() {

        coll = GetComponent<Collider2D>();
        contactFilter = new ContactFilter2D().NoFilter();

    }



    public int updateTouchingList() {

        touchingLength = coll.OverlapCollider(contactFilter, touching);
        return touchingLength;
    }


    public bool containsCandle(CandleLightController c) {
        Collider2D otherColl = c.getCandleIgniter().GetComponent<Collider2D>();

        for(int i = 0; i < touchingLength; i++) {
            if (touching[i] == otherColl) {
                return true;
            }
        }

        return false;

    }


    public bool isTouchingAnyCandles() {

        CandleIgniter c;

        for (int i = 0; i < touchingLength; i++) {

            c = touching[i].GetComponent<CandleIgniter>();

            if(c && c != this && c.isLightActive()) { 
                return true; 
            }

        }

        return false;

    }


    public void printTouchingList() {

        CandleIgniter c = GetComponent<CandleIgniter>();

        if (c != null) {
            Debug.Log("---------\ncandle: " + c.getParentCandleScript().getParentObject().name);
        }
        else {
            Debug.Log("---------\norigin: " + name);
        }
        
        for(int i = 0; i < touchingLength; i++) {
            CandleLightController x = touching[i].GetComponent<CandleLightController>();
            if(x != null) {
                Debug.Log(x.getParentObject().name + " | " + x.getId());
            }
            else {
                Debug.Log("Null");
            }
        }
        Debug.Log("---------");
    }


    public void printTouchingHistory() {
        CandleIgniter c = GetComponent<CandleIgniter>();
        Debug.Log("------------\nOrigin:" + c.getParentCandleScript().getName());
        for (int i = 0; i < touchingHistory.Count; i++) {
            Debug.Log(touchingHistory[i]);
        }
    }

}

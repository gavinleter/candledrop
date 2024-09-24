using System.Collections.Generic;
using UnityEngine;

public class CandleLightCollector : MonoBehaviour
{
    
    public List<CandleLightController> touching = new List<CandleLightController>();
    List<string> touchingHistory = new List<string>();


    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        CandleIgniter o = collision.GetComponent<CandleIgniter>();
        /*if(o != null) {
            touchingHistory.Add("Starting collision with " + o.getParentCandleScript().getName());
        }*/
        
        if (o != null && o.isLightActive()) {

            CandleLightController other = o.getParentCandleScript();

            //keep track of any candle that comes into contact with this one
            //since this class is inherited by CandleLightController, it also shouldnt detect itself
            if (other.gameObject != gameObject) {
                addToList(other);
                //Debug.Log(collision.gameObject.name + " " + gameObject.name);
            }

            /*if(GetComponent<CandleIgniter>() != null) {
                CandleLightController selfCan = GetComponent<CandleIgniter>().getParentCandleScript();
                //Debug.Log(selfCan.getParentObject().name + " | " + selfCan.getId() + " started touching " + other.getParentObject().name + " | " + other.getId());
            }*/

        }


    }


    protected virtual void OnTriggerExit2D(Collider2D collision) {
        CandleIgniter o = collision.GetComponent<CandleIgniter>();

        if (o != null) {

            CandleLightController other = o.getParentCandleScript();
            //touchingHistory.Add("Stopping collision with " + other.getName());

            //keep track of any candle that comes into contact with this one
            //since this class is inherited by CandleLightController, it also shouldnt detect itself
            if (other.gameObject != gameObject) {
                removeFromList(other);
            }

            /*if (GetComponent<CandleIgniter>() != null) {
                CandleLightController selfCan = GetComponent<CandleIgniter>().getParentCandleScript();
                //Debug.Log(selfCan.getParentObject().name + " | " + selfCan.getId() + " stopped touching " + other.getParentObject().name + " | " + other.getId());
            }*/

        }
    }


    public void addToList(CandleLightController can) {
        //touchingHistory.Add("Started touching " + can.getName());

        if (!touching.Contains(can)) {

            for (int i = 0; i < touching.Count; i++) {
                if (touching[i] == null) {
                    touching[i] = can;
                    return;
                }
            }

            touching.Add(can);

        }

    }


    public virtual void removeFromList(CandleLightController can) {
        //bool x = touching.Remove(can);
        
        for (int i = 0; i < touching.Count; i++) {
            //Debug.Log(touching[i].getParentObject().name + " viewing " + can.getParentObject().name);
            if (touching[i] == can) {
                touching[i] = null;
                //touchingHistory.Add("Removed " + can.getName());
                return;
            }
        }
        /*touchingHistory.Add("UNTRACKED " + can.getName());
        if (GetComponent<CandleIgniter>() != null) {
            Debug.Log("Origin: " + GetComponent<CandleIgniter>().getParentCandleScript().getParentObject().name + " | " + GetComponent<CandleIgniter>().getParentCandleScript().getId());
        }
        else {
            Debug.Log("Origin: " + gameObject.name);
        }
        Debug.Log("Untracked candle attempted to be removed\nTouching:");
        for (int i = 0; i < touching.Count; i++) {
            if (touching[i] != null) {
                Debug.Log(touching[i].getParentObject().name + " | " + touching[i].getId());
            }
            else {
                Debug.Log("Null");
            }
        }
        Debug.Log("Untracked candle: " + can.getParentObject().name + " | " + can.getId());
        //Debug.Log("");*/
    }


    public bool containsCandle(CandleLightController c) {
        return touching.Contains(c);
    }


    public bool isTouchingAnyCandles() {

        for (int i = 0; i < touching.Count; i++) {
            if (touching[i] != null && touching[i].isEnabled()) {
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
        
        foreach(CandleLightController x in touching) {
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

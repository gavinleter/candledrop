using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{

    float opacity = 0f;
    float lerp = 0f;
    SpriteRenderer sr;
    SpriteRenderer[] children;
    bool active = false;

    // Start is called before the first frame update
    void Start(){
        sr = GetComponent<SpriteRenderer>();
        children = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update(){

        if (active) { 
            lerp += 0.1f * Time.deltaTime;
            opacity = Mathf.Lerp(opacity, 1f, lerp);
            setAlpha();
            //Debug.Log(transform.position);
        } else if(lerp > 0f) {
            lerp -= 0.1f * Time.deltaTime;
            opacity = Mathf.Lerp(opacity, 0f, lerp);
            setAlpha();
        }

    }


    void setAlpha() {
        Color temp;

        //set opacity of all children
        for (int i = 0; i < children.Length; i++) {
            temp = children[i].color;
            temp.a = opacity;
            children[i].color = temp;
        }

        //set opacity of this object
        temp = sr.color;
        temp.a = opacity;
        sr.color = temp;
    }

    //when the game pauses, start lerping
    public void pause() {
        lerp = 0f;
        active = true;
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 10f);


    }

    //when the game unpauses, start lerping in reverse
    public void unpause() {
        lerp = 1f;
        active = false;
        //transform.position = new Vector3(100f, 0f, 0f);
    }


}

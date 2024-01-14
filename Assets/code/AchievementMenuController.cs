using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementMenuController : MonoBehaviour, IMenu
{

    [SerializeField] GameObject topObject;
    [SerializeField] GameObject bottomObject;
    [SerializeField] float topBoundOffset;
    [SerializeField] float bottomBoundOffset;

    [SerializeField] GameObject parentMenuObject;
    IMenu parentMenu;
    [SerializeField] List<ButtonPress> btns;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void unpause() {

    }


    public void pause() { 
    
    }


    public Vector3 getTransitionPosition() {
        return Vector3.zero;
    }
}

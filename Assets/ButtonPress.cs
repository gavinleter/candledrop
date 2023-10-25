using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    private Color originalColor;
    private Color highlightColor;
    private Renderer rend;

    // Customize these colors to your liking
    public Color normalColor = Color.white;
    public Color pressedColor = Color.gray;

    private bool isPressed = false;

    virtual protected void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        highlightColor = normalColor;
    }

    virtual protected void OnMouseDown()
    {
        isPressed = true;
        ChangeColor();
    }

    virtual protected void OnMouseUp()
    {
        isPressed = false;
        ChangeColor();
    }

    virtual protected void OnMouseExit()
    {
        OnMouseUp();
    }

    virtual protected void ChangeColor()
    {
        if (isPressed)
        {
            rend.material.color = pressedColor;
        }
        else
        {
            rend.material.color = normalColor;
        }
    }

    public bool btnPressed() {
        return isPressed;
    }

}

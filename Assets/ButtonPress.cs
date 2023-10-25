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

    private void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        highlightColor = normalColor;
    }

    private void OnMouseDown()
    {
        isPressed = true;
        ChangeColor();
    }

    private void OnMouseUp()
    {
        isPressed = false;
        ChangeColor();
    }

    private void OnMouseExit()
    {
        if (isPressed)
        {
            isPressed = false;
            ChangeColor();
        }
    }

    private void ChangeColor()
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
}

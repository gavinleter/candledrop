using UnityEngine;

public class ColorFadingObject : Lerpable
{

    [SerializeField] Color targetColor;
    [SerializeField] bool changeAlpha;
    Color initialColor;
    Color currentColor;
    SpriteRenderer sr;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        initialColor = sr.color;
        currentColor = sr.color;
    }


    protected override void Start() {
        base.Start();

        //lerpIn();
    }


    protected override void increaseLerp() {
        base.increaseLerp();
        updateColor();
    }


    protected override void decreaseLerp() {
        base.decreaseLerp();
        updateColor();
    }


    void updateColor() {
        currentColor.r = Mathf.Lerp(initialColor.r, targetColor.r, lerp);
        currentColor.g = Mathf.Lerp(initialColor.g, targetColor.g, lerp);
        currentColor.b = Mathf.Lerp(initialColor.b, targetColor.b, lerp);

        if (changeAlpha) {
            currentColor.a = Mathf.Lerp(initialColor.a, targetColor.a, lerp);
        }
        else {
            currentColor.a = sr.color.a;
        }

        sr.color = currentColor;
    }


    public void setTargetColor(Color c) {
        targetColor = c;
    }

    public void canChangeAlpha(bool x) {
        changeAlpha = x;
    }

}

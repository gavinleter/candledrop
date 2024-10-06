using UnityEngine;

public class RainbowObject : Lerpable
{

    Color32 currentColor = new Color32();
    SpriteRenderer sr;
    //this is set to 2 so an initial condition always passes
    byte valueAtLastSwitch = 2;

    //this array goes in the order "rbg" rather than "rgb" because this is the order the colors are cycled from their
    //max and min values when spinning the color wheel in the unity editor
    //at all times, one color must be at 0, one at 255, and one at some point inbetween
    byte[] colors = new byte[3];
    int currentIndex = 0;

    override protected void Start() {
        base.Start();

        sr = GetComponent<SpriteRenderer>();
        currentColor.a = 255;

        //This next section of code determines the starting rgb values. There must be one at 255 (max), one at 0 (min), and one at a random value between 0-255.
        //The variables here determine the index where the max, min, and random values will be.

        //choose which color will start at max value (255)
        int maxPoint = UnityEngine.Random.Range(0, 3);

        //choose which color will be at min value (0)
        int minPoint = UnityEngine.Random.Range(0, 1);
        //convert to 1 or -1 and add with maxPoint later, resulting in it either being in front or behind it
        minPoint = minPoint == 1 ? 1 : -1;

        //get the last unused index for the random point
        int randomPoint = (maxPoint + 3 + minPoint * 2) % 3;

        minPoint = (minPoint + 3 + maxPoint) % 3;


        colors[maxPoint] = 255;
        colors[minPoint] = 0;
        colors[randomPoint] = (byte)UnityEngine.Random.Range(0, 256);

        lerp = (colors[randomPoint] * 1f) / (255 * 1f);

        currentIndex = randomPoint;

        //have to determine whether to increase or decrease the value of colors[randomPoint] based on the previous point
        if (colors[getPreviousIndex()] == 0) {
            lerpingIn = true;
        }
        else if (colors[getPreviousIndex()] == 255) {
            lerpingIn = false;
        }

    }


    protected override void Update() {

        if (isActive()) {
            base.Update();

            colors[currentIndex] = (byte)Mathf.Lerp(0, 255, lerp);

            //keep in mind the colors array is ordered in "rbg"
            currentColor.r = colors[0];
            currentColor.b = colors[1];
            currentColor.g = colors[2];
            currentColor.a = (byte)(sr.color.a * 255);
            sr.color = currentColor;


            if (valueAtLastSwitch != colors[currentIndex] && (colors[currentIndex] == 0 || colors[currentIndex] == 255)) {
                
                if (colors[getPreviousIndex()] == 0) {
                    //since this color is at its max the next color should decrease from max to min
                    increaseCurrentIndex();
                    lerp = 1;
                    lerpOut();
                    valueAtLastSwitch = colors[currentIndex];
                }
                else if (colors[getPreviousIndex()] == 255) {
                    //since this color is at its min the next color should increase from min to max
                    increaseCurrentIndex();
                    lerp = 0;
                    lerpIn();
                    valueAtLastSwitch = colors[currentIndex];
                }

            }
        }

    }


    int getPreviousIndex() {
        return (currentIndex + 2) % 3;
    }


    void increaseCurrentIndex() {
        currentIndex = (currentIndex + 1) % 3;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingObject : Lerpable
{
    
    [SerializeField] Vector3 targetSizeMultiplier;
    [SerializeField] bool looping;
    Vector3 initialSize;
    Vector3 targetSize;

    private void Awake() {
        
        initialSize = transform.localScale;
        targetSize = Vector3.Scale(initialSize, targetSizeMultiplier);

    }


    protected override void Update() {
        base.Update();

        if (looping && isActive()) {
            if (lerpInFinished() && lerpingIn) {
                lerpOut();
            }
            else if (lerpOutFinished() && !lerpingIn) {
                lerpIn();
            }
        }

    }


    protected override void increaseLerp() {
        base.increaseLerp();
        transform.localScale = Vector3.Lerp(initialSize, targetSize, lerp);
    }

    protected override void decreaseLerp() {
        base.decreaseLerp();
        transform.localScale = Vector3.Lerp(initialSize, targetSize, lerp);
    }


    public void setTargetSizeMultiplier(Vector3 s) {
        targetSizeMultiplier = s;
    }


}

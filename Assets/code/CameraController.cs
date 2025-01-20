using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 initialPosition = new Vector3(0f, 76f, -10f);
    Vector3 targetPosition;

    Vector3 gameStartPosition = new Vector3(0f, 55f, -10f);
    //Vector3 gamePosition = new Vector3(0f, 10.55f, -10f);

    [SerializeField] float transitionSpeed;
    [SerializeField] float delayBeforeTransition;
    [SerializeField] float candleFallTransitionSpeed;

    [SerializeField] FadingObject blackFadeObject;

    //this is an object that should represent the minimum width of the camera
    [SerializeField] GameObject minCameraWidthObj;
    float minCameraWidth;

    //represents the bottom of the game area
    [SerializeField] Transform minimumGameAreaPosition;
    Vector3 gamePosition = new Vector3(0, 0, -10);

    private float transitionStartTime;
    private bool isTransitioning = false;
    private bool isBlackFadeTransitioning = false;
    private bool initialBlackFadeInCompleted = false;

    private bool introDelayFinished = false;

    Camera cam;
    //the distance between the middle of the screen and top in world units
    float camHeight;
    float lastMouseYPosition = 0;
    float dragInertia = 0;
    //auto scroll exists to make a video of the achievements and shouldnt be used in normal gameplay
    float autoScrollSpeed = 0;

    bool scrollMode = false;
    [SerializeField] float scrollModeDragStrength;
    [SerializeField] float scrollModeInertiaStrength;
    float scrollModeUpperBound;
    float scrollModeLowerBound;
    //for having something happen when the user tries to go above the upper/lower limit of the scroll area
    System.Action scrollModeUpperLimitAction;
    System.Action scrollModeLowerLimitAction;

    System.Action endTransitionAction;


    private void Awake() {
        cam = GetComponent<Camera>();
        transform.position = initialPosition;
        targetPosition = gameStartPosition;
        minCameraWidth = minCameraWidthObj.GetComponent<SpriteRenderer>().bounds.size.x;

        setTargetCameraZoom();
        camHeight = Mathf.Abs(transform.position.y - cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y);
        setTargetGamePosition();
    }


    void Update()
    {

        //wait for x seconds before transitioning downwards when the game starts
        if(!introDelayFinished && delayBeforeTransition < Time.time) {
            introDelayFinished = true;
            startTransition();
        }


        if (isTransitioning)
        {
            //check if the target position is the same as the starting position to prevent division by 0
            if (initialPosition.Equals(targetPosition)) {
                isTransitioning = false;
                return;
            }

            float timeSinceStart = Time.time - transitionStartTime;
            float journeyLength = Vector3.Distance(initialPosition, targetPosition);

            float journeyFraction = Mathf.SmoothStep(0f, 1f, timeSinceStart / (journeyLength / transitionSpeed));

            transform.position = Vector3.Lerp(initialPosition, targetPosition, journeyFraction);

            //check if the transition is complete
            if (journeyFraction >= 1.0f)
            {
                isTransitioning = false;

                if(endTransitionAction != null) {
                    endTransitionAction();
                    //end transition actions are discarded after being used once
                    endTransitionAction = null;
                }

            }
        }


        //controls the black fade in and out transitions
        if (isBlackFadeTransitioning) {
            if (blackFadeObject.fadeInFinished() && !initialBlackFadeInCompleted) {

                blackFadeObject.lerpOut();
                transform.position = targetPosition;
                initialBlackFadeInCompleted = true;

            }

            if (initialBlackFadeInCompleted) {

                //end transition actions happen when the fade out starts rather than when the entire transition is over
                if (endTransitionAction != null) {
                    endTransitionAction();
                    //end transition actions are discarded after being used once
                    endTransitionAction = null;
                }

                if (blackFadeObject.fadeOutFinished()) {
                    isBlackFadeTransitioning = false;
                    initialBlackFadeInCompleted = false;
                }

            }

        }


        updateScrollModePosition();
        checkScrollModeBounds();
    }


    public void skipIntroTransition() {
        if (!introDelayFinished) {
            introDelayFinished = true;
            transitionSpeed = 20f;
            startTransition();
        }
    }

    //used to go directly to game area if save data is found
    public void skipIntroToBottom() {
        introDelayFinished = true;
        fadeToBlackTransitionToBottom(0.1f);
    }

    public void setTransitionSpeed(float speed) {
        transitionSpeed = speed;
    }

    public void startTransition()
    {
        isTransitioning = true;
        transitionStartTime = Time.time;
        scrollMode = false;
        initialBlackFadeInCompleted = false;
    }

    public void startGameTransition() {
        setNewTarget(gamePosition, candleFallTransitionSpeed);
        startTransition();
    }


    public void restartTransition() {
        setNewTarget(gameStartPosition, candleFallTransitionSpeed);
        startTransition();
    }


    public void transitionToTop(float speed) {
        setNewTarget(gameStartPosition, speed);
        startTransition();
    }


    public void transitionToBottom(float speed) {
        transitionToBottom(speed, null);
    }


    public void transitionToBottom(float speed, System.Action endAction) {
        setNewTarget(gamePosition, speed);
        startTransition();
        endTransitionAction = endAction;
    }



        public void setNewTarget(Vector3 targetPosition, float transitionSpeed, System.Action endAction) {
        introDelayFinished = true;
        isTransitioning = false;
        initialPosition = transform.position;
        this.targetPosition = targetPosition;
        this.transitionSpeed = transitionSpeed;
        endTransitionAction = endAction;
    }
    public void setNewTarget(Vector3 targetPosition, System.Action endAction) {
        setNewTarget(targetPosition, transitionSpeed, endAction);
    }
    public void setNewTarget(Vector3 targetPosition, float transitionSpeed) {
        setNewTarget(targetPosition, transitionSpeed, null);
    }
    public void setNewTarget(Vector3 targetPosition) {
        setNewTarget(targetPosition, transitionSpeed);
    }



    public bool currentlyTransitioning() {
        return isTransitioning || isBlackFadeTransitioning || !introDelayFinished;
    }


    public bool currentlyScrollTransitioning() {
        return isTransitioning || !introDelayFinished;
    }


    public void fadeToBlackTransition(Vector3 targetPosition, float fadeSpeed, System.Action endAction) {
        blackFadeObject.forceLerpOut();
        blackFadeObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        blackFadeObject.setSpeed(fadeSpeed);
        blackFadeObject.lerpIn();

        this.targetPosition = targetPosition;
        isBlackFadeTransitioning = true;
        initialBlackFadeInCompleted = false;
        scrollMode = false;

        endTransitionAction = endAction;
    }
    public void fadeToBlackTransition(Vector3 targetPosition, float fadeSpeed) {
        fadeToBlackTransition(targetPosition, fadeSpeed, null);
    }



    public void fadeToBlackTransitionToTop(float fadeSpeed, System.Action endAction) {
        fadeToBlackTransition(gameStartPosition, fadeSpeed, endAction);
    }
    public void fadeToBlackTransitionToTop(float fadeSpeed) {
        fadeToBlackTransition(gameStartPosition, fadeSpeed, null);
    }



    public void fadeToBlackTransitionToBottom(float fadeSpeed, System.Action endAction) {
        fadeToBlackTransition(gamePosition, fadeSpeed, endAction);
    }
    public void fadeToBlackTransitionToBottom(float fadeSpeed) {
        fadeToBlackTransition(gamePosition, fadeSpeed, null);
    }



    public void toggleScrollMode(bool x, float upperBound, float lowerBound) {
        scrollMode = x;
        scrollModeUpperBound = upperBound;
        scrollModeLowerBound = lowerBound;

        scrollModeUpperLimitAction = null;
        scrollModeLowerLimitAction = null;
    }
    public void toggleScrollMode(bool x) {
        toggleScrollMode(x, scrollModeUpperBound, scrollModeLowerBound);
    }



    void checkScrollModeBounds() {

        if (scrollMode) {

            if(transform.position.y + camHeight > scrollModeUpperBound) {
                transform.position = new Vector3(transform.position.x, scrollModeUpperBound - camHeight - 0.001f, transform.position.z);
                dragInertia = 0;
                autoScrollSpeed = 0;

                if (scrollModeUpperLimitAction != null) {
                    scrollModeUpperLimitAction();
                }
            }

            if (transform.position.y - camHeight < scrollModeLowerBound) {
                transform.position = new Vector3(transform.position.x, scrollModeLowerBound + camHeight + 0.001f, transform.position.z);
                dragInertia = 0;
                autoScrollSpeed = 0;

                if (scrollModeLowerLimitAction != null) {
                    scrollModeLowerLimitAction();
                }
            }

        }

    }


    void updateScrollModePosition() {

        if (scrollMode) {

            float mouseY = 0;

            //set the initial value for lastMouseYPosition when the user starts pressing down
            if (Input.GetMouseButtonDown(0)) {
                lastMouseYPosition = Input.mousePosition.y;
                dragInertia = 0;
            }

            if (Input.GetMouseButton(0)) {
                mouseY = Input.mousePosition.y;
            }
            else {
                mouseY = lastMouseYPosition;
            }

            float dragStrength = (lastMouseYPosition - mouseY) * scrollModeDragStrength;
            dragInertia += dragStrength * scrollModeInertiaStrength;
            dragInertia = Mathf.Lerp(dragInertia, 0, 0.05f);

            if(autoScrollSpeed != 0) {
                dragStrength = autoScrollSpeed;
                dragInertia = 0;
            }

            transform.Translate(new Vector3(0, dragStrength, 0));

            //inertia only works when the user isnt holding down
            if (!Input.GetMouseButton(0)) {
                transform.Translate(new Vector3(0, dragInertia, 0));
            }

            lastMouseYPosition = mouseY;

        }

    }


    void setTargetCameraZoom() {

        float currentWidth = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth - 1, 0, 0)).x - cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        
        if (currentWidth < minCameraWidth) {
            cam.orthographicSize = minCameraWidth / cam.aspect * 0.5f;

        }

    }


    void setTargetGamePosition() { 
        
        gamePosition.y = minimumGameAreaPosition.position.y + getCamHeight();

    }


    public void setScrollModeLimitActions(System.Action top, System.Action bottom) {
        scrollModeUpperLimitAction = top;
        scrollModeLowerLimitAction = bottom;
    }


    public float getCamHeight() {
        return camHeight;
    }


    public void clearEndTransitionAction() {
        endTransitionAction = null;
    }


    public void setEndTransitionAction(System.Action endAction) {
        endTransitionAction = endAction;
    }


    public void setAutoScrollSpeed(float x) {
        autoScrollSpeed = x;
    }

}

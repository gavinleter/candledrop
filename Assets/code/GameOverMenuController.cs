
using TMPro;
using UnityEngine;

public class GameOverMenuController : FadingMenuController
{

    ParticleSystem[] sideCandleSmoke = new ParticleSystem[2];

    [SerializeField] ParticleSystem goodScoreParticles;
    [SerializeField] ParticleSystem badScoreParticles;

    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI highScoreText;

    [SerializeField] GameManager gameManager;

    [SerializeField] CameraController mainCam;

    [SerializeField] MusicManager musicManager;

    //these two control the high score ticking up after a better score is achieved
    [SerializeField] float highScoreInitialDelay;
    float highScoreTickingDelay;
    float initialTime = 0;
    float tickingTime = 0;
    bool waitingForTick = false;

    float textLerp = 0;
    float initialFontSize;
    [SerializeField] float growFontSize;
    Color initialFontColor;
    [SerializeField] Color growFontColor;


    int currentScore = 0;
    int highScore = 0;


    protected override void Start(){
        base.Start();

        initialFontSize = highScoreText.fontSize;
        initialFontColor = highScoreText.color;

        //the first two children must be the two candles on the side of the menu
        for (int i = 0; i < sideCandleSmoke.Length; i++) {
            sideCandleSmoke[i] = transform.GetChild(i).GetComponent<ParticleSystem>();
        }
        
        //restart button
        btns[0].onPress(() => { 
            unpause();
            gameManager.resetGame();
            //mainCam.restartTransition();
            mainCam.fadeToBlackTransitionToTop(0.1f);
            gameManager.unpause();
        });

    }

    
    protected override void Update(){
        base.Update();

        //make sure textLerp doesn't go over 1
        if(highScore < currentScore) {
            textLerp = Mathf.Clamp(textLerp + Time.deltaTime * 2f, 0, 1);
        }
        else {
            textLerp -= Time.deltaTime * 2f;
        }
        

        if (lerpingIn) {

            //once the initial delay has passed, get ready to start ticking the new high score up
            if(!waitingForTick && Time.time > highScoreInitialDelay + initialTime && highScore < currentScore) {
                waitingForTick = true;
                tickingTime = Time.time;
            }

            //once enough time has passed, increase the text on the high score by one
            if(waitingForTick && Time.time > highScoreTickingDelay + tickingTime) {
                highScore++;
                highScoreText.text = "" + highScore;
                waitingForTick = false;
            }

            float newSize = Mathf.Lerp(initialFontSize, growFontSize, textLerp);
            highScoreText.fontSize = newSize;

            Color newColor = new Color(Mathf.Lerp(initialFontColor.r, growFontColor.r, textLerp), 
                                       Mathf.Lerp(initialFontColor.g, growFontColor.g, textLerp), 
                                       Mathf.Lerp(initialFontColor.b, growFontColor.b, textLerp));
            highScoreText.color = newColor;
        }

    }


    //highScore represents the LAST high score, it does not include if currentScore sets a new record
    public void setScores(int currentScore, int highScore) {
        this.currentScore = currentScore;
        this.highScore = highScore;
    }


    public override void pause() {
        base.pause();

        musicManager.setSelectedMusic(4);

        transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, 0f);
        initialTime = Time.time;
        highScoreText.fontSize = initialFontSize;
        highScoreText.color = initialFontColor;
        textLerp = 0f;
        //the minimum speed points go up by on the game over menu is 0.4f - it cant go slower
        highScoreTickingDelay = Mathf.Min(0.4f, 1f/Mathf.Abs(currentScore - highScore));

        for (int i = 0; i < sideCandleSmoke.Length; i++) {
            sideCandleSmoke[i].Play();
        }

        currentScoreText.text = "" + currentScore;
        highScoreText.text = "" + highScore;

        if (currentScore > highScore) {
            goodScoreParticles.Play();
        }
        else{
            badScoreParticles.Play();
        }

    }


}

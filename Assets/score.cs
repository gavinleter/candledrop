
// using unity UI is important since its needed for text obj
using UnityEngine.UI;
using UnityEngine;

public class score : MonoBehaviour {



// two objects that will need to be seen ingame
    public Text score;
    public Text highScore;


    void Start ()

    // remember what the previous high score was in a previous session and set it to it
    {
        highScore.text = PlayerPrefs.GetInt("Highscore",0).ToString();
    }


    // process to get score
    public void getScore ()
    {
        int number = //sum of all number of candles that were lined up, and then add bonuses              !!!! need help here !!!!

        // once line above calculates score, line below turns to string to be shown ingame
        score.text = number.ToString();

        // if the score above is greater than the current highscore, which is set to zero at first by default, because the ",0" above and below,
        // this checks that
        if (number > PlayerPrefs.GetInt("Highscore",0))
        {
        
        // if above is true, set the new highscore to the current score and also make it text to be able to be seen by attached text obj
        PlayerPrefs.SetInt("HighScore", number);
        highScore.text = number;
        }


    }




// below is a simple function that i also looked up, basically if you call it with a button, it will wipe all user data,
// like achievement progress, score, and highscore stuff. Easy way to cover everything. Might have to come back here to
// set the default values of everything again, like all the achievements to false and skins to locked and stuff.
    public void Reset ()
    {

        PlayerPrefs.DeleteAll();
        highScore.text = "0";
        score.text = "0";


    }
}


// in summary, we should us playerprefs to store all the data, it seems like the easiest and most effective thing to do :)
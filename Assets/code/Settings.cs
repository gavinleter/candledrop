using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Settings
{

    private static bool soundEnabled = true;
    private static bool musicEnabled = true;

    private static int highScore = 0;

    private static bool[] secretButtonFound = new bool[3];
    private static readonly string secretButtonPrefName = "secretButton";


    public static void initSettings() {

        //by default music and sound are enabled
        soundEnabled = isSoundEnabled();
        musicEnabled = isMusicEnabled();

        highScore = getHighScore();

        for (int i = 0; i < secretButtonFound.Length; i++) {
            secretButtonFound[i] = isSecretButtonFound(i);
        }

    }


    public static int getStarterCandleId() {
        return 0;
    }


    public static int getStarterCandleSkinId() {
        return 0;
    }


    public static bool skinUnlocked(int candle, int skin) {
        return skin < 2;
    }


    public static bool candleUnlocked(int candle) {
        return true;
    }

    public static bool isSecretButtonFound(int id) {
        return PlayerPrefs.GetInt(secretButtonPrefName + id, 0) == 1;
    }

    public static void setSecretButtonFound(int id) {
        PlayerPrefs.SetInt(secretButtonPrefName + id, 1);
        PlayerPrefs.Save();

        secretButtonFound[id] = true;
    }

    public static int getSecretButtonCounter() {
        int result = 0;

        for(int i = 0; i < secretButtonFound.Length; i++) {
            if (secretButtonFound[i]) {
                result++;
            }
        }

        return result;
    }



    public static void toggleMusic(bool x) {
        musicEnabled = x;
        int xInt = x ? 1 : 0;

        PlayerPrefs.SetInt("musicEnabled", xInt);
        PlayerPrefs.Save();
    }


    public static bool isMusicEnabled() {
        return PlayerPrefs.GetInt("musicEnabled", 1) == 1;
    }


    public static void toggleSound(bool x) {
        soundEnabled = x;
        int xInt = x ? 1 : 0;

        PlayerPrefs.SetInt("soundEnabled", xInt);
        PlayerPrefs.Save();
    }

    public static bool isSoundEnabled() {
        return PlayerPrefs.GetInt("soundEnabled", 1) == 1;
    }



    public static void setHighScore(int x) {
        PlayerPrefs.SetInt("highScore", x);
        PlayerPrefs.Save();
    }


    public static int getHighScore() {
        return PlayerPrefs.GetInt("highScore", 0);
    }


}

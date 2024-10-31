using Unity.Burst.CompilerServices;
using UnityEngine;

public class Settings
{

    private static bool soundEnabled = true;
    //musicStatus can be   0: muted  |  1: enabled (default track)  |  2: enabled (alt track)
    private static int musicStatus = 1;

    private static int highScore = 0;

    private static bool[] secretButtonFound = new bool[3];
    private static readonly string secretButtonPrefName = "secretButton";


    public static void initSettings(GameManager gameManager) {

        //by default music and sound are enabled
        soundEnabled = isSoundEnabled();
        musicStatus = getMusicStatus();

        highScore = getHighScore();

        for (int i = 0; i < secretButtonFound.Length; i++) {
            secretButtonFound[i] = isSecretButtonFound(i);
        }


        gameManager.toggleWaffleRain(isWaffleRainEnabled());
        gameManager.toggleSnowy(isSnowyEnabled());

    }


    public static void deleteAllSaveData(GameManager gameManager) {
        PlayerPrefs.DeleteAll();

        //music and sound are the only settings that shouldn't be deleted
        setMusicStatus(musicStatus);
        toggleSound(soundEnabled);

        initSettings(gameManager);
    }


    public static int getStarterCandleId() {
        return PlayerPrefs.GetInt("starterCandleId");
    }

    public static int getStarterCandleSkinId() {
        return PlayerPrefs.GetInt("starterCandleSkinId");
    }

    public static void setStarterCandleId(int x) {
        PlayerPrefs.SetInt("starterCandleId", x);
        PlayerPrefs.Save();
    }

    public static void setStarterCandleSkinId(int x) {
        PlayerPrefs.SetInt("starterCandleSkinId", x);
        PlayerPrefs.Save();
    }


    public static bool skinUnlocked(int candle, int skin) {
        return true;
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


    //generic method for setting any playerpref
    private static void toggleOption(string key, bool x) {
        int xInt = x ? 1 : 0;

        PlayerPrefs.SetInt(key, xInt);
        PlayerPrefs.Save();
    }





    public static void setMusicStatus(int x) {
        musicStatus = x;

        PlayerPrefs.SetInt("musicEnabled", x);
        PlayerPrefs.Save();
    }


    public static int getMusicStatus() {
        return PlayerPrefs.GetInt("musicEnabled", 1);
    }


    public static bool isAltMusicEnabled() {
        return PlayerPrefs.GetInt("musicEnabled", 1) == 2;
    }


    public static bool isMusicEnabled() {
        return PlayerPrefs.GetInt("musicEnabled", 1) != 0;
    }


    public static void toggleSound(bool x) {
        soundEnabled = x;
        toggleOption("soundEnabled", x);
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





    public static bool waffleRainUnlocked() {
        return PlayerPrefs.GetInt("waffleRainUnlock", 0) == 1 || true;
    }


    public static bool isWaffleRainEnabled() {
        return PlayerPrefs.GetInt("waffleRain", 0) == 1;
    }


    public static void toggleWaffleRain(bool x) {
        toggleOption("waffleRain", x);
    }


    public static bool snowyUnlocked() {
        return PlayerPrefs.GetInt("snowyUnlock", 0) == 1 || true;
    }


    public static bool isSnowyEnabled() {
        return PlayerPrefs.GetInt("snowy", 0) == 1;
    }


    public static void toggleSnowy(bool x) {
        toggleOption("snowy", x);
    }


}

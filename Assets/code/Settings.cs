using System.IO;
using UnityEngine;


[System.Serializable]
public class Achievement {

    public bool unlocked = false;
    //"tapped" indicates whether the user has tapped to unlock the achievement, not if the conditions to unlock are met
    public bool tapped = false;

}

//arrays have to be wrapped in a class to be serializeable
[System.Serializable]
public class AchievementHolder {

    public Achievement[] achs;

    public AchievementHolder(int achievementCount) {
        achs = new Achievement[achievementCount];

        for (int i = 0; i < achievementCount; i++) { 
            achs[i] = new Achievement();
        }
    }

}


public class Settings
{

    private static GameManager gameManager;

    private static bool soundEnabled = true;
    //musicStatus can be   0: muted  |  1: enabled (default track)  |  2: enabled (alt track)
    private static int musicStatus = 1;

    private static int highScore = 0;

    private static bool[] secretButtonFound = new bool[3];
    private static readonly string secretButtonPrefName = "secretButton";

    private static AchievementHolder achievements;
    private static readonly string achievementFileName = Application.dataPath + "/achievements.json";


    public static void initSettings(GameManager g) {

        gameManager = g;

        //by default music and sound are enabled
        soundEnabled = isSoundEnabled();
        musicStatus = getMusicStatus();

        highScore = getHighScore();

        for (int i = 0; i < secretButtonFound.Length; i++) {
            secretButtonFound[i] = isSecretButtonFound(i);
        }


        gameManager.toggleWaffleRain(isWaffleRainEnabled());
        gameManager.toggleSnowy(isSnowyEnabled());
        gameManager.resetHighScoreText();


        achievements = getAchievements();
        //if the achievements do not exist yet, the file needs to be created immediately
        saveAchievements();

    }



    public static void deleteAllSaveData(GameManager gameManager) {
        PlayerPrefs.DeleteAll();
        File.Delete(achievementFileName);
        achievements = getAchievements();
        saveAchievements();

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


    public static int getPointlessAdCount() {
        return PlayerPrefs.GetInt("pointlessAdCount");
    }


    public static void increasePointlessAdCount() {
        PlayerPrefs.SetInt("pointlessAdCount", getPointlessAdCount() + 1);
        PlayerPrefs.Save();

        int adCount = getPointlessAdCount();

        if(adCount >= 3) {
            //"I love you" unlocked by watching 3 pointless ads
            setAchievementUnlocked(24);
        }
        if (adCount >= 10) {
            //"I get to eat tonight" unlocked by watching 10 pointless ads
            setAchievementUnlocked(25);
        }
        if (adCount >= 25) {
            //"Student debt fund" unlocked by watching 25 pointless ads
            setAchievementUnlocked(26);
        }
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
        //achievement 33 unlocks waffle rain
        return isAchievementTapped(33);
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




    static AchievementHolder getAchievements() {

        if (File.Exists(achievementFileName)) {

            string x = File.ReadAllText(achievementFileName);

            return JsonUtility.FromJson<AchievementHolder>(x);

        }

        return new AchievementHolder(39);
    }


    static void saveAchievements() {

        File.WriteAllText(achievementFileName, JsonUtility.ToJson(achievements));

    }


    public static bool isAchievementTapped(int x) {
        return achievements.achs[x].tapped;
    }


    public static bool isAchievementUnlocked(int x) {
        return achievements.achs[x].unlocked;
    }


    public static void setAchievementTapped(int x) {

        if (!achievements.achs[x].tapped) {
            
            achievements.achs[x].tapped = true;
            saveAchievements();
        }

    }


    public static void setAchievementUnlocked(int x) {

        if (!achievements.achs[x].unlocked) {

            achievements.achs[x].unlocked = true;
            saveAchievements();
            gameManager.achievementUnlockPopup(x);

            if (isAchievementTapped(35) && isAchievementTapped(36) && isAchievementTapped(37) && isAchievementTapped(38)) {
                //"Black Hole Expert" unlocked by also unlocking "Waxicide" (#35), "Friendly Fire!" (#36), "Void Savior" (#37), and "Void-maxxer" (#38)
                setAchievementUnlocked(39);
            }

        }

    }



}

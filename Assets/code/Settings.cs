using System;
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

    internal AchievementHolder(int achievementCount) {
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

    private static int achievementCount = 47;
    private static AchievementHolder achievements;
    private static readonly string achievementFileName = Application.persistentDataPath + "/achievements.json";

    static bool loadSaveFailed = false;


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
        //saveAchievements();

        //if test mode is on, grant all achievements
        if (g.unlockAllAchievements()) {

            for (int i = 0; i < achievements.achs.Length; i++) {
                setAchievementUnlocked(i);
                setAchievementTapped(i);
            }

        }
        
    }



    public static void deleteAllSaveData(GameManager gameManager) {
        PlayerPrefs.DeleteAll();

        deleteAchievementData();

        //music and sound are the only settings that shouldn't be deleted
        setMusicStatus(musicStatus);
        toggleSound(soundEnabled);

        initSettings(gameManager);
    }


    public static void deleteAchievementData() {
        File.Delete(achievementFileName);
        File.Delete(achievementFileName + ".meta");

        achievements = getAchievements();
        saveAchievements();
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


    public static bool skinUnlocked(int skin) {
        
        switch (skin) {

            //default skin
            case 0:
                return true;

            //bronze skin
            case 1:
                return isAchievementTapped(13);

            //silver skin
            case 2:
                return isAchievementTapped(14);

            //gold skin
            case 3:
                return isAchievementTapped(15);

            //glass skin
            case 4:
                return isAchievementTapped(34);

            //glitch/hacker skin
            case 5:
                return isAchievementTapped(16);

            //love skin
            case 6:
                return isAchievementTapped(24);

            //rainbow skin
            case 7:
                return isAchievementTapped(19);

            //black hole skin
            case 8:
                return isAchievementTapped(39);

            //sun skin
            case 9:
                return isAchievementTapped(43);

            default:
                return false;

        }

    }


    //how many skins are unlocked
    public static int skinsUnlockedCount() {
        int count = 0;

        for (int i = 0; i < SkinManager.SKIN_COUNT; i++) {

            if (skinUnlocked(i)) {
                count++;
            }
        
        }

        return count;
    }


    public static bool candleUnlocked(int candle) {

        switch (candle) {

            //default candle
            case 0:
                return true;

            //n candle
            case 1:
                return isAchievementTapped(3);

            //candleabra
            case 2:
                return isAchievementTapped(44);

            //flare
            case 3:
                return isAchievementTapped(6);

            //fat candle
            case 4:
                return isAchievementTapped(10);

            default:
                return false;

        }
    }


    //how many candles are unlocked
    public static int candlesUnlockedCount() {
        int count = 0;

        for (int i = 0; i < SkinManager.CANDLE_COUNT; i++) {

            if (candleUnlocked(i)) {
                count++;
            }

        }

        return count;
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


    public static bool altTrackUnlocked() {
        return isAchievementTapped(27);
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
        return isAchievementTapped(45);
    }


    public static bool isSnowyEnabled() {
        return PlayerPrefs.GetInt("snowy", 0) == 1;
    }


    public static void toggleSnowy(bool x) {
        toggleOption("snowy", x);
    }


    static AchievementHolder getAchievements() {

        try {

            if (File.Exists(achievementFileName)) {
                //throw new Exception();
                string x = File.ReadAllText(achievementFileName);
                AchievementHolder ah = JsonUtility.FromJson<AchievementHolder>(x);

                //if the achievement holder has less achievements than there should be
                if (ah.achs.Length != achievementCount) {

                    Debug.LogWarning("Missing achievements, counted " + ah.achs.Length + ", expected " + achievementCount);

                    Achievement[] expandedAchievements = new Achievement[achievementCount];

                    //copy all old achievements
                    for (int i = 0; i < ah.achs.Length; i++) {
                        expandedAchievements[i] = ah.achs[i];
                    }

                    //create new empty achievements
                    for (int i = ah.achs.Length; i < expandedAchievements.Length; i++) {
                        expandedAchievements[i] = new Achievement();
                    }

                    ah.achs = expandedAchievements;
                }

                return ah;
            }

        }
        catch (Exception e) {

            //delete achievement data if it cant be read properly
            Debug.LogError(e);
            Debug.LogWarning("Cannot read achievement data");
            //deleteAchievementData();
            //gameManager.openFailedSaveMenu();
            loadSaveFailed = true;

        }
        
        return new AchievementHolder(achievementCount);
    }


    static void saveAchievements() {

        try {
            File.WriteAllText(achievementFileName, JsonUtility.ToJson(achievements));
        }
        catch (Exception e) {
            Debug.LogError(e);
            Debug.LogWarning("Failed to save achievements");
        }

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

            if (isAchievementTapped(40) && isAchievementTapped(41) && isAchievementTapped(42)) {
                //"Sun Expert" unlocked by also unlocking "You Are My Sunshine" (#40), "Solar Savior" (#41), and "Praise the Suns" (#42)
                setAchievementUnlocked(43);
            }

            if (achievementsUnlockedCount() >= 30) {
                //"Triad and true" unlocked by unlocking 30 other achievements
                setAchievementUnlocked(44);
            }

            if (achievementsUnlockedCount() >= 40) {
                //"Snowy the budgie" unlocked by unlocking 40 other achievements
                setAchievementUnlocked(45);
            }

        }

    }


    public static int getAchievementCount() {
        return achievements.achs.Length;
    }


    public static int achievementsUnlockedCount() {
        int count = 0;

        for(int i = 0; i < achievements.achs.Length; i++) {
            if (achievements.achs[i].tapped) {
                count++;
            }
        }

        return count;
    }


    //how many secret rewards are unlocked
    public static int secretsUnlockedCount() {

        int count = 0;

        bool[] x = {
            isAchievementTapped(45), //snowy
            isAchievementTapped(33), //waffle rain
            isAchievementTapped(27) //alt soundtrack
        };

        for (int i = 0; i < x.Length; i++) {
            if (x[i]) {
                count++;
            }
        }

        return count;

    }


    public static bool loadFromSaveFailed() {
        return loadSaveFailed;
    }


}

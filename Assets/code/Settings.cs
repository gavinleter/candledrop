using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{

    public static bool soundEnabled = true;
    public static bool musicEnabled = true;
    public static int secretButtonCounter = 0;



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
        return candle < 2;
    }


    public static void increaseSecretButtonCounter(){

        secretButtonCounter += 1;


    }

}

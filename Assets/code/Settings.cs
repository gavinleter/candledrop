using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{

    public static bool soundEnabled = true;
    public static bool musicEnabled = true;
    public static int secretButtonCounter = 0;




    public static void increaseSecretButtonCounter(){

        secretButtonCounter += 1;


    }

}

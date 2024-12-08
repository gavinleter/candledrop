using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class CandleData {

    //the state of each light in a candle
    //0 == off, 1 == on, 2 == flare, 3 == supercharged
    public int[] lightState;
    public int candleId = 0;
    public float[] position = new float[3];
    public float[] rotation = new float[4];
    public float linearVelocityX;
    public float linearVelocityY;
    public float angularVelocity;
    //if this is a special object, this should not be -1
    //0 == black hole, 1 == mini sun
    public int specialObject = -1;

    internal CandleData(GameObject can) {

        //if this is a special object, store the type it is
        ISpecialObject x = can.GetComponentInChildren<ISpecialObject>();

        if (x != null) {
            specialObject = x.getType();
        }

        //if this object is a candle
        if (specialObject == -1) {

            CandleLightController[] lights = can.GetComponentsInChildren<CandleLightController>();
            lightState = new int[lights.Length];

            //get the state of each light
            for (int i = 0; i < lights.Length; i++) {

                if (lights[i].isMiniSunIgnited()) {
                    lightState[i] = 3;

                }
                else if (lights[i].isCurrentlyFlare()) {
                    lightState[i] = 2;

                }
                else if (lights[i].isEnabled()) {
                    lightState[i] = 1;

                }
                //if none of these conditions are met, the lightState will be 0 (light is off) because thats the default int value

            }

            //if a candle id is -1, that means its a starter candle and its type and skin can be pulled from the Settings script
            candleId = can.GetComponent<CandleId>().getId();

        }

        position[0] = can.transform.position.x;
        position[1] = can.transform.position.y;
        position[2] = can.transform.position.z;

        rotation[0] = can.transform.rotation.x;
        rotation[1] = can.transform.rotation.y;
        rotation[2] = can.transform.rotation.z;
        rotation[3] = can.transform.rotation.w;

        Rigidbody2D rb = can.GetComponent<Rigidbody2D>();

        linearVelocityX = rb.velocity.x;
        linearVelocityY = rb.velocity.y;
        angularVelocity = rb.angularVelocity;

    }

}


[System.Serializable]
class SaveData {

    public float chainProgress = 0;
    public CandleData heldCandle;
    public CandleData[] droppedCandles;

    internal SaveData(CandleData[] droppedCandles, CandleData heldCandle, float chainProgress) {

        this.droppedCandles = droppedCandles;
        this.heldCandle = heldCandle;
        this.chainProgress = chainProgress;

    }

}


public class SaveManager
{

    static readonly string saveFileName = Application.dataPath + "/Save/save.json";
    static SaveData save;



    //should be run once on game startup
    //returns false if there is no save data
    public static bool initSaveData() {

        if (File.Exists(saveFileName)) {

            string x = File.ReadAllText(saveFileName);
            save = JsonUtility.FromJson<SaveData>(x);

            return true;

        }

        return false;

    }


    public static float getChainProgress() {
        return save.chainProgress;
    }


    public static CandleData getHeldCandle() {
        return save.heldCandle;
    }


    public static CandleData[] getSavedObjects() {
        return save.droppedCandles;
    }


    public static void clearSaveData() {
        File.Delete(saveFileName);
        File.Delete(saveFileName + ".meta");
    }


    public static void updateSave(GameObject[] droppedCandles, GameObject heldCandle, GameObject[] specialObjects, float chainProgress) {

        CandleData[] d = new CandleData[droppedCandles.Length + specialObjects.Length];

        //normal candles
        for (int i = 0; i < droppedCandles.Length; i++) {
            d[i] = new CandleData(droppedCandles[i]);
        }

        //special objects
        for (int i = droppedCandles.Length; i < specialObjects.Length; i++) {
            d[i] = new CandleData(droppedCandles[i]);
        }

        save = new SaveData(d, new CandleData(heldCandle), chainProgress);
        File.WriteAllText(saveFileName, JsonUtility.ToJson(save));

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{

    [SerializeField] GameObject[] can1Prefabs;
    [SerializeField] GameObject[] nPrefabs;
    [SerializeField] GameObject[] candleabraPrefabs;
    [SerializeField] GameObject[] flarePrefabs;
    [SerializeField] GameObject[] can7Prefabs;

    [SerializeField] Sprite[] candleCovers;

    List<GameObject[]> skins = new List<GameObject[]>();

    public static readonly int SKIN_COUNT = 12;
    public static readonly int CANDLE_COUNT = 6;


    void Awake() {
        
        skins.Add(can1Prefabs);
        skins.Add(nPrefabs);
        skins.Add(candleabraPrefabs);
        skins.Add(flarePrefabs);
        skins.Add(can7Prefabs);

    }


    public Sprite getCandleCover(int candle) {
        return candleCovers[candle];
    }


    public GameObject getCandlePrefab(int candle, int skin) {
        return skins[candle][skin];
    }

}

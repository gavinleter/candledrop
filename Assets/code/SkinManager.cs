using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{

    [SerializeField] Sprite[] can1Skins;
    [SerializeField] Sprite[] nSkins;
    [SerializeField] Sprite[] candleabraSkins;
    [SerializeField] Sprite[] flareSkins;
    [SerializeField] Sprite[] can7Skins;

    [SerializeField] Sprite[] candleCoverSkins;

    List<Sprite[]> skins = new List<Sprite[]>();


    void Awake() {
        
        skins.Add(can1Skins);
        skins.Add(nSkins);
        skins.Add(candleabraSkins);
        skins.Add(flareSkins);
        skins.Add(can7Skins);

    }

    public Sprite getSkin(int candle, int skin) {
        return skins[candle][skin];
    }

    public Sprite getCover(int candle) {
        return candleCoverSkins[candle];
    }

}

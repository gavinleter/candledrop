using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candletest : MonoBehaviour
{

    [SerializeField] ButtonPress[] btns;

    void Start()
    {

        btns[0].onPress(() => {
            CandleLightCollector[] x = GetComponentsInChildren<CandleLightCollector>();
            for (int i = 0; i < x.Length; i++) {
                x[i].printTouchingHistory();
            }
        });

    }

    
}

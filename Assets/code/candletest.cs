using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candletest : MonoBehaviour
{

    [SerializeField] ButtonPress[] btns;

    void Start()
    {

        btns[0].onPress(() => {
            /*CandleLightCollector[] x = GetComponentsInChildren<CandleLightCollector>();
            for (int i = 0; i < x.Length; i++) {
                x[i].printTouchingHistory();
            }*/

            CandleIgniter x = GetComponentInChildren<CandleIgniter>();
            List<Collider2D> c = new List<Collider2D>();

            x.GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), c);
            for(int i = 0; i < c.Count; i++) {
                Debug.Log(c[i].name);
                if (c[i].GetComponent<CandleIgniter>()) {
                    Debug.Log(c[i].GetComponent<CandleIgniter>().getParentCandleScript().getName() + " <------");
                }
            }
            Debug.Log("------------");
        });

    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCandleSpawner : ButtonPress
{

    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject candleToSpawn;
    [SerializeField] int candleId;
    [SerializeField] bool isFlare;
    [SerializeField] Transform spawnLocation;
    


    override protected void Start(){
        base.Start();

        onPress(() => {

            GameObject spawnedCandle = Instantiate(candleToSpawn, spawnLocation.position, Quaternion.identity);
            Destroy(spawnedCandle.GetComponent<StartCandleFall>());
            spawnedCandle.GetComponent<Rigidbody2D>().gravityScale = 1;

            gameManager.addCandleLight(spawnedCandle);
            spawnedCandle.GetComponent<CandleId>().setInfo(candleId, candleId == -1);

            if (isFlare) {
                CandleLightController[] c = spawnedCandle.GetComponentsInChildren<CandleLightController>();

                for (int i = 0; i < c.Length; i++) {
                    c[i].convertToFlare();
                }
            }

        });
    }


}

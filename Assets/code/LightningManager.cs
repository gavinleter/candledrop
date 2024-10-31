using UnityEngine;


[System.Serializable]
class LightningGroup {

    public GameObject lightningPrefab;
    public Transform spawnPosition;
    public Vector2 minOffset;
    public Vector2 maxOffset;
    public float sizeMultiplier;

}


public class LightningManager : MonoBehaviour{


    [SerializeField] LightningGroup[] lightningGroups;


    //triggers a single lightning in all groups
    public void triggerLightning() {

        float xOffset;
        float yOffset;
        Vector3 newPosition;

        for(int i = 0; i < lightningGroups.Length; i++) {

            newPosition = lightningGroups[i].spawnPosition.position;

            xOffset = UnityEngine.Random.Range(lightningGroups[i].minOffset.x, lightningGroups[i].maxOffset.x);
            yOffset = UnityEngine.Random.Range(lightningGroups[i].minOffset.y, lightningGroups[i].maxOffset.y);

            newPosition.x += xOffset;
            newPosition.y += yOffset;

            GameObject x = Instantiate(lightningGroups[i].lightningPrefab, lightningGroups[i].spawnPosition);
            x.transform.localScale *= lightningGroups[i].sizeMultiplier;
            x.transform.position = newPosition;

        }

    }


}

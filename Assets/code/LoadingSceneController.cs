using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{

    void Start(){

        StartCoroutine(loadMainScene());

    }

    
    IEnumerator loadMainScene() {
        AsyncOperation load = SceneManager.LoadSceneAsync("Scenes/Curtis Scene");

        while (!load.isDone) { 
            yield return null;
        }
    }


}

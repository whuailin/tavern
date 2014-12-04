using UnityEngine;
using System.Collections;

[AddComponentMenu("Test/Script_Mobile/BreakAndEnd")]
public class BreakAndEnd : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        endGame();
	}

    private void endGame()
    {
       if(Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))){
           Application.Quit();
       }
    }
}

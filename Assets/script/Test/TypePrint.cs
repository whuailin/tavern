using UnityEngine;
using System.Collections;

public class TypePrint : MonoBehaviour
{
    public float letterPause = 0.2f;
    public AudioClip sound;
    private string word;
    private string text = "欢迎来到武林客栈！";

	// Use this for initialization
	void Start () {
        word = text;
        text = "";
        StartCoroutine(TypeText());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
       // GUI.Label(new Rect(100, 75, 250, 25), "欢迎来到武林客栈！");
        GUI.Box(new Rect(100, 100, 250, 25), text);
    }

    IEnumerator TypeText()
    {
        foreach(char letter in word.ToCharArray()) {
            text += letter;
            if (sound) {
                audio.PlayOneShot(sound);
                yield return new WaitForSeconds(letterPause);
            }
        }
    }
}

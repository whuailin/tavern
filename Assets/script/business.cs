using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class business : MonoBehaviour {
	public int dayTime = 10;
	public enum BusinessStatus{wait, in_business};

	private BusinessStatus curStatus;
	private float curTime;

	private Text text;
 	// Use this for initialization
	void Start () {
		curStatus = BusinessStatus.wait;
		curTime = 0.0f;
		text = GameObject.Find ("BusinessTxt").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (curStatus == BusinessStatus.in_business) {
			if(curTime > dayTime) {
				curTime = 0;
				curStatus = BusinessStatus.wait;
				text.text = "等待开张.....";
			} else {
				curTime += Time.deltaTime;
				text.text = "开张中： " + (int)curTime;
			}

		} else {
			text.text = "等待开张.....";
		}
	}

	public void starBusiness() {
		curStatus = BusinessStatus.in_business;
	}
}

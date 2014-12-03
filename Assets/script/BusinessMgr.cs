using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BusinessMgr : MonoBehaviour {
    public Slider processBar;

    public int dayTime = 100;
    public enum BusinessStatus { wait, in_business };

    private BusinessStatus curStatus;
    private float curTime;

    private Text text;

	// Use this for initialization
	void Start () {
        curStatus = BusinessStatus.wait;
        curTime = 0.0f;
        text = GameObject.Find("BusinessTxt").GetComponent<Text>();
        processBar.value = dayTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (curStatus == BusinessStatus.in_business)
        {
            Application.LoadLevel(1);
            if (curTime > dayTime)
            {
                curTime = 0;
                curStatus = BusinessStatus.wait;
                text.text = "等待开张.....";
            }
            else
            {
                curTime += Time.deltaTime;
                text.text = "开张中： " + (int)curTime;
                processBar.value = processBar.value - 1 * Time.deltaTime;
            }

        }
        else
        {
            text.text = "等待开张.....";
        }
	}

    public void starBusiness()
    {
        curStatus = BusinessStatus.in_business;
    }
}

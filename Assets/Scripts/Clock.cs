using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public static float Value { get; private set; }
    public static string StringValue { get
        {
            int total = (int)Value;
            int sec = total % 60;
            int min = total / 60;

            return (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
        }
    }

    private Text time;
    void Start()
    {
        Value = 0;
        time = GetComponent<Text>();
    }

    void Update()
    {
        if(Menu.IsActive == false)
        {
            Value += Time.deltaTime;
            time.text = StringValue;
        }


        //string minutes, seconds;
        //if ((int)Value / 60 < 10) minutes = "0" + (int)Value / 60;
        //else minutes = (int)Value / 60 + "";
        //if ((int)Value % 60 < 10) seconds = "0" + (int)Value % 60;
        //else seconds = (int)Value / 60 + "";
        //time.text = minutes + ":" + seconds;
    }
}

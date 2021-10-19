using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static MenuMode MenuMode { get; set; }
    public static bool IsActive { get; set; }

    private Text Title;

    void Start()
    {
        Title = GameObject.Find("Title").GetComponent<Text>();
    }

    void LateUpdate()
    {
        switch (MenuMode)
        {
            case MenuMode.Start:
                Title.text = "Game Start";
                break;
            case MenuMode.Pause:
                Title.text = "Pause";
                break;
            case MenuMode.GameOver:
                Title.text = "Game Over\n" + "Balls: " + Controls.attempt + ", Time:" + Clock.StringValue;
                break;
        }
    }
}

public enum MenuMode
{
    Start, Pause, GameOver
}
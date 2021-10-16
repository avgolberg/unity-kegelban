using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static MenuMode MenuMode { get; set; }
    public static bool IsActive { get; set; }

    private Text Title;

    private float resultValue;

    void Start()
    {
        Title = GameObject.Find("Title").GetComponent<Text>();
    }

    void LateUpdate()
    {
        switch (MenuMode)
        {
            case MenuMode.Start:
                Title.text = "Начало игры";
                break;
            case MenuMode.Pause:
                Title.text = "Пауза";
                break;
            case MenuMode.GameOver:
                if(File.Exists(Application.dataPath + "/results.txt"))
                {
                    StreamReader sr = new StreamReader(Application.dataPath + "/results.txt"); // открываем файл
                    string line;
                    if ((line = sr.ReadLine()) != null)
                    {
                        resultValue = float.Parse(line);
                    }
                    sr.Close(); // закрываем файл
                }
                else
                {
                    StreamWriter sw = new StreamWriter(Application.dataPath + "/results.txt", false);
                    sw.WriteLine(Clock.Value); // записываем в файл строку
                    resultValue = Clock.Value;
                    sw.Close(); // закрываем файл
                }
               
                
                if(resultValue > Clock.Value)
                {
                    StreamWriter sw = new StreamWriter(Application.dataPath + "/results.txt", false);
                    sw.WriteLine(Clock.Value); // записываем в файл строку
                    sw.Close(); // закрываем файл
                    Title.text = "Конец игры\nВаше время: " + Clock.StringValue + "\nЛучшее время: " + Clock.StringValue;
                }
                else 
                {
                    int total = (int)resultValue;
                    int sec = total % 60;
                    int min = total / 60;

                    Title.text = "Конец игры\nВаше время: " + Clock.StringValue + "\nЛучшее время: " + (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;                 
                }
                break;
        }
    }
}

public enum MenuMode
{
    Start, Pause, GameOver
}
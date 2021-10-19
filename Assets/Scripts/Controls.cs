using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //Text, Image, Button

public class Controls : MonoBehaviour
{
    private GameObject Ball;
    private Rigidbody ballRigidBody;
    private Vector3 ballStartPosition;
    private bool isBallMoving;

    private GameObject Arrow;
    private GameObject ArrowTail;
    private float arrrowAngle;
    private float maxArrowAngle = 20f;

    private float MIN_FORCE = 1000f;
    private float MAX_FORCE = 2000f;
    private const string BEST_RES_FILE = "best.xml";

    private Text GameStat;
    public static int attempt = 0;

    private GameObject GameMenu;

    private List<GameResult> bestResults; //таблица рекордов
    public static GameObject BestRes = null;
    private Text BestResText;

    private int allkegelsDown;

    private Image ForceIndicator;
    private GameObject Indicator;

    void Start()
    {
        //получаем ссылку на компонент Image объекта ForceIndicator
        GameMenu = GameObject.Find("Menu");
        BestRes = GameObject.Find("BestRes");
        BestResText = GameObject.Find("BestResText").GetComponent<Text>();
        ForceIndicator = GameObject.Find("ForceIndicator").GetComponent<Image>();
        GameStat = GameObject.Find("GameStat").GetComponent<Text>();
        Indicator = GameObject.Find("Indicator");
        Arrow = GameObject.Find("Arrow");
        ArrowTail = GameObject.Find("ArrowTail");
        Ball = GameObject.Find("Ball");

        ballRigidBody = Ball.GetComponent<Rigidbody>();
        ballStartPosition = Ball.transform.position;
        Menu.MenuMode = MenuMode.Start;
        Menu.IsActive = true;
        BestRes.SetActive(false);
        GameMenu.SetActive(true); 

        attempt = 0;
        allkegelsDown = 0;
        arrrowAngle = 0f;
        isBallMoving = false;

        LoadBestResults();
    }

    // Update is called once per frame
    void Update()
    {
        #region Таблица рекордов
        if (Input.GetKeyDown(KeyCode.B) && !isBallMoving && !BestRes.activeInHierarchy)
        {
            if (Menu.IsActive)
            {
                Menu.IsActive = false;
                GameMenu.SetActive(false);
            }
            BestRes.SetActive(true);
            StringBuilder sb = new StringBuilder();
            if (bestResults != null)
            {
                bestResults.Sort();
                foreach (var res in bestResults)
                {
                    sb.Append(res + "\n");
                }
                BestResText.text = "Best results:\n" + sb.ToString();
            }
            else BestResText.text = "No best results yet";
        }
        else if (Input.GetKeyDown(KeyCode.B) && BestRes.activeInHierarchy)
        {
            if (!Menu.IsActive)
            {
                Menu.IsActive = true;
                GameMenu.SetActive(true);
            }
            BestRes.SetActive(false);
        }
        #endregion

        if (Menu.IsActive) return;
        
        #region Остановка шарика
        if (ballRigidBody.velocity.magnitude < 0.1f && isBallMoving)
        {
            isBallMoving = false;
            Ball.transform.position = ballStartPosition;
            ballRigidBody.velocity = Vector3.zero;
            ballRigidBody.angularVelocity = Vector3.zero;
            // Собираем информацию о кеглях
            int kegelsUp = 0;
            int kegelDown = 0;
            foreach (GameObject kegel in GameObject.FindGameObjectsWithTag("Kegel"))
            {
                // стоит y=0, лежит y=0.4, y>0.1 - упала
                if (kegel.transform.position.y > 0.08f || Mathf.Abs(kegel.transform.rotation.x) > 0.01 || Mathf.Abs(kegel.transform.rotation.z) > 0.01)
                {
                    kegel.SetActive(false);
                    kegelDown++;
                    allkegelsDown++;
                }
                else
                {
                    kegelsUp++;
                    kegel.transform.rotation = Quaternion.Euler(0, 0, 0);
                    kegel.transform.position.Set(kegel.transform.position.x, 0, kegel.transform.position.z);
                }
            }
            Arrow.SetActive(true);
            Indicator.SetActive(true);
            attempt++;
            GameStat.text += "\n" + attempt + "   " + Clock.StringValue + "   "+ kegelDown + "   " + kegelsUp;
            if (allkegelsDown == 10)
            {
                Menu.MenuMode = MenuMode.GameOver;
                Menu.IsActive = true;
                GameMenu.SetActive(true);
                if (bestResults == null)
                {
                    bestResults = new List<GameResult>();
                }
                bestResults.Add(new GameResult { Balls = attempt, Time = Clock.Value });
                if (bestResults.Count>5)
                {
                    bestResults.Sort();
                    bestResults.RemoveAt(bestResults.Count - 1);
                } 
                using (StreamWriter writer = new StreamWriter(BEST_RES_FILE))
                {
                    XmlSerializer serializer = new XmlSerializer(bestResults.GetType());
                    serializer.Serialize(writer, bestResults);
                }
            }
        }
        #endregion

        #region Запуск шарика
        if (Input.GetKeyDown(KeyCode.Space) && !isBallMoving && !BestRes.activeInHierarchy)
        {
            Vector3 forceDirection = Arrow.transform.forward;
            float forceFactor = MIN_FORCE + (MAX_FORCE - MIN_FORCE) * ForceIndicator.fillAmount;
           
            ballRigidBody.AddForce(forceDirection * forceFactor);
            ballRigidBody.velocity = forceDirection * 0.1f;
            Arrow.SetActive(false);
            Indicator.SetActive(false);
            isBallMoving = true;
        }
        #endregion

        #region Вращение стрелки
        if (!isBallMoving && !BestRes.activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.LeftArrow) && arrrowAngle > -maxArrowAngle)
            {
                Arrow.transform.RotateAround(ArrowTail.transform.position, Vector3.up, -1f);
                arrrowAngle -= 1f;
            }
            if (Input.GetKey(KeyCode.RightArrow) && arrrowAngle < maxArrowAngle)
            {
                Arrow.transform.RotateAround(ArrowTail.transform.position, Vector3.up, 1f);
                arrrowAngle += 1f;
            }
        }
        #endregion

        #region Индикатор силы
        if (!isBallMoving && !BestRes.activeInHierarchy)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                // ForceIndicator.fillAmount += 0.01f;  // ! time dependent !
                float val = ForceIndicator.fillAmount + Time.deltaTime / 2;
                if (val <= 1)
                    ForceIndicator.fillAmount = val;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                float val = ForceIndicator.fillAmount - Time.deltaTime / 2;
                if (val >= .1f)
                    ForceIndicator.fillAmount = val;
            }
        }
        #endregion

        #region Пауза
        if (Input.GetKeyDown(KeyCode.Escape) && Menu.IsActive == false && !isBallMoving)
        {
            Menu.MenuMode = MenuMode.Pause;
            Menu.IsActive = true;
            GameMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Menu.IsActive)
        {
            Menu.IsActive = false;
            GameMenu.SetActive(false);
        }
        #endregion
    }

    public void PlayClick()
    {
        if (Menu.MenuMode == MenuMode.GameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        GameMenu.SetActive(false);
        Menu.IsActive = false;
    }

    private void LoadBestResults()
    {
        // файл с результатами - объявлен в константах
        if (File.Exists(BEST_RES_FILE))
        {
            using(StreamReader reader = new StreamReader(BEST_RES_FILE))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<GameResult>));
                bestResults = (List<GameResult>)serializer.Deserialize(reader);
            }
        }
    }
}

public class GameResult : IComparable<GameResult>
{
    public int Balls { get; set; } //количество бросков
    public float Time { get; set; } //время раунда

    public string TimeToString
    {
        get
        {
            int total = (int)Time;
            int sec = total % 60;
            int min = total / 60;

            return (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
        }
    }
    public int CompareTo(GameResult y)
    {
        if (this.Balls < y.Balls) return -1;
        else if(this.Balls == y.Balls)
        {
            if (this.Time < y.Time) return -1;
            else if (this.Time == y.Time) return 0;
        }
        return 1;
    }

    public override string ToString()
    {
        return "Balls: " + Balls + ", Time:" + TimeToString;
    }
}

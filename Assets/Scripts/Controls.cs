using System.Collections;
using System.Collections.Generic;
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

    private Text GameStat;
    private int attempt;

    private GameObject GameMenu;


    //private int fell;
    private int allkegelsDown;

    private Image ForceIndicator;
    private GameObject Indicator;

    private bool isReloaded;

    void Start()
    {
        //startGame = 0;
        //fell = 0;
        //left = 10;
        //text.text = "Попытка: " + startGame + "\nCбито: " + fell + "\nОсталось: " + left;

        //получаем ссылку на компонент Image объекта ForceIndicator
        GameMenu = GameObject.Find("Menu");
        ForceIndicator = GameObject.Find("ForceIndicator").GetComponent<Image>();
        GameStat = GameObject.Find("GameStat").GetComponent<Text>();
        Indicator = GameObject.Find("Indicator");
        Arrow = GameObject.Find("Arrow");
        ArrowTail = GameObject.Find("ArrowTail");
        Ball = GameObject.Find("Ball");
        ballRigidBody = Ball.GetComponent<Rigidbody>();
        ballStartPosition = Ball.transform.position;
        Menu.MenuMode = MenuMode.Start;
        newGame();
    }

    private void newGame()
    {
        Menu.IsActive = true;
        GameMenu.SetActive(true);
      
        attempt = 0;
        allkegelsDown = 0;
        arrrowAngle = 0f;
        isBallMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
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
                
                if(allkegelsDown==10)
                {
                    Menu.MenuMode = MenuMode.GameOver;
                    Menu.IsActive = true;
                    GameMenu.SetActive(true);
                }
               // Debug.Log(kegel.transform.position);
            }
            //left = 10 - fell;
            //text.text = "Попытка: " + startGame + "\nCбито: " + fell + "\nОсталось: " + left;
            Arrow.SetActive(true);
            Indicator.SetActive(true);
            attempt++;
            GameStat.text += "\n" + attempt + "   " + Clock.StringValue + "   "+ kegelDown + "   " + kegelsUp;
        }
        #endregion

        #region Запуск шарика
        if (Input.GetKeyDown(KeyCode.Space) && !isBallMoving)
        {
            //startGame++;
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
        #endregion

        #region Индикатор силы
        if (!isBallMoving)
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
        if (Input.GetKeyDown(KeyCode.Escape) && Menu.IsActive == false)
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
}

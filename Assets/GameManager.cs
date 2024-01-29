using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 적 생성위치 배열
    Vector3[] arrays = new Vector3[9];
    int num;
    public int enemyNum;

    // 점수
    int bestScore;
    int previousScere;
    public int score;
    public Text bestScoreText;
    public Text scoreText;

    // 폭탄 이미지
    public GameObject bombImg;

    // 타이머
    float timer;
    public Text timerText;
    // 타이머2
    float delayTime;

    // 게임 오버
    bool gameOver;
    GameObject overImg;

    void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == 0)
                {
                    arrays[i * 3 + j] = new Vector3(-30, 0, 25 + (j * 20));
                }
                else if (i == 1)
                {
                    arrays[i * 3 + j] = new Vector3(0, 0, 25 + (j * 20));
                }
                else
                {
                    arrays[i * 3 + j] = new Vector3(30, 0, 25 + (j * 20));
                }
            }
            enemyNum = 0;
            num = 0;
        }
        arrays = ShuffleArray(arrays);

        //bestScore 가져오기
        bestScore = this.gameObject.GetComponent<DataManager>().GetData();
        previousScere = bestScore;
        score = 0;

        // 폭탄이미지
        bombImg = GameObject.Find("bombLife");

        // 타이머
        timer = 6;
        timerText.enabled = false;
        delayTime = 0;

        // 게임 오버
        gameOver = false;
        overImg = GameObject.Find("GameOver");
        overImg.SetActive(gameOver);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 적생성
        InvokeRepeating("create", 1f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            // 폭탄 이미지 
            bombImage();

            // 점수 업데이트
            bestScoreText.text = "최고점수 : " + bestScore;
            scoreText.text = "점수 : " + score;
            if (bestScore < score)
            {
                bestScore = score;
            }

            gameTimer();
        }
        else
        {
            if(Input.anyKeyDown)
            {
                SceneManager.LoadScene("TitleScene");
            }
        }
    }

    // 셔플
    private T[] ShuffleArray<T>(T[] array)
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < array.Length; ++i)
        {
            random1 = Random.Range(0, array.Length);
            random2 = Random.Range(0, array.Length);

            temp = array[random1];
            array[random1] = array[random2];
            array[random2] = temp;
        }

        return array;
    }

    void create()
    {
        if (!gameOver)
        {
            if (enemyNum < 15)
            {
                // 생성 하고
                var Enemy = ObjectPool.Instance.GetObjectEnemy();
                // 생성 위치
                float randX = Random.Range(-10f, 10f);
                float randZ = Random.Range(-10f, 10f);

                var EnemyPos = arrays[num] + new Vector3(randX, 2f, randZ);
                //Debug.Log("왔다");
                Enemy.GetComponent<Enemy>().CreateEnemy(EnemyPos);
                num++;
                enemyNum++;
                if (num >= 9)
                {
                    arrays = ShuffleArray(arrays);
                    num = 0;
                }
            }
        }
    }


    // 폭탄 이미지
    void bombImage()
    {
        GameObject tank = GameObject.Find("Tank");//.GetComponent<TankControl>();
        if (tank.GetComponent<TankControl>() == true)
        {
            switch (tank.GetComponent<TankControl>().bullet)
            {
                case 0:
                    bombImg.transform.GetChild(2).GetComponent<Image>().enabled = false;
                    break;
                case 1:
                    bombImg.transform.GetChild(1).GetComponent<Image>().enabled = false;
                    break;
                case 2:
                    bombImg.transform.GetChild(0).GetComponent<Image>().enabled = false;
                    break;
                case 3:
                    bombImg.transform.GetChild(0).GetComponent<Image>().enabled = true;
                    bombImg.transform.GetChild(1).GetComponent<Image>().enabled = true;
                    bombImg.transform.GetChild(2).GetComponent<Image>().enabled = true;
                    break;
            }
        }
    }

    // 타이머
    void gameTimer()
    {
        if (delayTime >= 5)
        {
            if (timerText.enabled == false)
                timerText.enabled = true;
            int emptTimer = (int)timer;
            timerText.text = emptTimer.ToString();
            Debug.Log(timer);
            timer -= 1 * Time.deltaTime;

            // 게임오버
            if (timer < 0)
            {
                if (!gameOver)
                {
                    if (previousScere != bestScore)
                    {
                        this.gameObject.GetComponent<DataManager>().SetData(bestScore);
                    }
                    gameOver = true;
                    overImg.SetActive(gameOver);
                }
            }
        }

        if (enemyNum >= 15)
        {
            if (delayTime <= 5)
                delayTime += 1 * Time.deltaTime;
        }
        else
        {
            if (timerText.enabled == true)
                timerText.enabled = false;
            if (timer != 6)
                timer = 6;
            if (delayTime != 0)
                delayTime = 0;
        }
    }
}

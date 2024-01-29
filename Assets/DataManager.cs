using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(int score)
    {
        // 저장된 모든 데이터 제거
        PlayerPrefs.DeleteAll();
        // 특정 데이터 제거
        //PlayerPrefs.DeleteKey("bestScore");

        PlayerPrefs.SetInt("bestScore", score);

        PlayerPrefs.Save(); // 저장하기 (필수)
    }

    public int GetData()
    {
        if (PlayerPrefs.HasKey("bestScore"))
        {
            int num = PlayerPrefs.GetInt("bestScore", 0);

            return num;
        }
        else
        {
            return 0;
        }
    }

}

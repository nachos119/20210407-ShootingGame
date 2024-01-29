using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    // 미사일
    [SerializeField]
    private GameObject poolingObjectPrefab;
    // Queue<missile> poolingObjectQueue = new Queue<missile>();
    Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();

    // 적
    [SerializeField]
    private GameObject poolingObjectEnemyPrefab;

    Queue<GameObject> poolingObjectEnemyQueue = new Queue<GameObject>();


    private void Awake()
    {
        Instance = this;
        Initialize(3, 20);
    }

    private void Initialize(int initCount, int enemyCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject());
        }
        for (int i = 0; i < enemyCount; i++)
        {
            poolingObjectEnemyQueue.Enqueue(EnemyCreateNewObject());
        }
    }

    // 미사일
    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(poolingObjectPrefab);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public GameObject GetObject()
    {
        if (Instance.poolingObjectQueue.Count > 0)
        {
            var obj = Instance.poolingObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            //obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObject();
            //newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    // 적
    private GameObject EnemyCreateNewObject()
    {
        GameObject newObj = Instantiate(poolingObjectEnemyPrefab);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public GameObject GetObjectEnemy()
    {
        if (Instance.poolingObjectEnemyQueue.Count > 0)
        {
            var obj = Instance.poolingObjectEnemyQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.EnemyCreateNewObject();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }

    }

    // 리턴
    public void ReturnObject(GameObject obj)
    {
        switch (obj.tag)
        {
            case "Missile":
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(Instance.transform);
                Instance.poolingObjectQueue.Enqueue(obj);
                break;
            case "Enemy":
                gameObject.GetComponent<GameManager>().enemyNum--;
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(Instance.transform);
                Instance.poolingObjectEnemyQueue.Enqueue(obj);
                break;
        }
    }
}

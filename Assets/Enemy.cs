using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    void Awake()
    {
    }

    public void CreateEnemy(Vector3 pos)
    {
        transform.position = pos;
    }

    public void DestroyBullet()
    { 
        //Debug.Log("충돌했당");
        gameObject.GetComponent<Rigidbody>().AddExplosionForce(5000.0f, transform.position + Vector3.right * 5f, 10.0f, 5000.0f);
        StartCoroutine("returnEnemy");
    }

    IEnumerator returnEnemy()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().score += 10;
        yield return new WaitForSeconds(2.5f);
        ObjectPool.Instance.ReturnObject(gameObject);
        Rigidbody rigid = gameObject.GetComponent<Rigidbody>();
        rigid.velocity = new Vector3(0f, 0f, 0f);
        rigid.angularVelocity = new Vector3(0f, 0f, 0f);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

        //gameObject.GetComponent<Rigidbody>().
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Missile")
        {
            gameObject.GetComponent<Rigidbody>().AddExplosionForce(5000.0f, other.gameObject.transform.position, 5.0f, 1000.0f);
            StartCoroutine("returnEnemy");
        }
    }
    void OnTriggerStay(Collider other)
    {
    }
    void OnTriggerExit(Collider other)
    {
    }
}

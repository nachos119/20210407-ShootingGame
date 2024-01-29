using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missile : MonoBehaviour
{
    public Vector3 target;
    
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    // public Transform Projectile;
    // private Transform myTransform;

    // 이펙트
    [SerializeField]
    GameObject prefabEffect;

    void Awake()
    {
    }

    public void Shoot(Vector3 start, Vector3 dest)
    {
        transform.position = start;
        this.target = dest;

        gameObject.SetActive(true);

        LaucherProjecttile();
        //Invoke("DestroyBullet", 5f);
    }

    public void DestroyBullet()
    {
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("도착 지점 : " + target);
        //Debug.Log(this.transform.position);
        // 땅에 도착하면
        if (this.transform.position.y <= -1f)
        {
            DestroyBullet();
        }
    }

    void LaucherProjecttile()
    {
        // 계산
        Vector3 newVelocity = CalculateVelcoity(target, transform.position, 1f);
        Rigidbody obj = this.GetComponent<Rigidbody>();
        obj.velocity = newVelocity;
        
        //setTrajectoryPoints(transform.position, Vo);
        //DrawPath(newVelocity);
    }

    //이 방법은 목표 벡터와 원점의 시작점이 필요합니다.
    //time : 비행시간
    Vector3 CalculateVelcoity(Vector3 target, Vector3 origin, float time)
    {
        //define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance; //x와z의 평면이면 기본적으로 거리와 같은 벡터
        distanceXZ.y = 0f;//y는 0으로 설정

        //create a float the represent our distance
        float Sy = distance.y;//세로 높이의 거리를 지정
        float Sxz = distanceXZ.magnitude;

        //속도 계산
        float Vxz = Sxz / time;
        float Vy = ((Sy / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time));

        //계산으로 인해 두축의 초기 속도 가지고 새로운 벡터를 만들수 있음
        Vector3 result = target.normalized;
        result *= Vxz;
        result.y = Vy;
        // Debug.Log(result);
        return result;
    }

    void DrawPath(Vector3 velocity)
    {
        Vector3 previousDrawPoint = transform.position;
        int resolution = 30;
        //lineRenderer.positionCount = resolution;
        for (int i = 1; i <= resolution; i++)
        {
            //float simulationTime = i / (float)resolution * launchData.timeToTarget;
            float simulationTime = i / (float)resolution * 5f;

            Vector3 displacement = velocity * simulationTime + Vector3.up * Physics.gravity.y * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = transform.position + displacement;
            //Debug.lin(drawPoint, 1, 1000f);//유니티 에셋스토어 Debug Extension
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green, 3f);
            //lineRenderer.SetPosition(i - 1, drawPoint);
            previousDrawPoint = drawPoint;
            if (previousDrawPoint.y <= 0f)
            {
                return;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        //if(other.tag == "Enemy")
        //{
        //    other.GetComponent<Enemy>().DestroyBullet();
        //}
        if (other.tag == "Wall")
        {
            Instantiate(prefabEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
            DestroyBullet();
        }
        else if (other.tag == "Ground")
        {
            GetComponent<SphereCollider>().radius = 5.0f;
            Instantiate(prefabEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
        }
        //DestroyBullet();
        //Debug.Log("충돌");
    }
    void OnTriggerStay(Collider other)
    {

    }
    void OnTriggerExit(Collider other)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankControl : MonoBehaviour
{
    // 탱크의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State
    {
        Idle,      // 발사 준비됨 : 이동O, 발사O 
        Delay,      // 발사 : 이동O, 발사X     
        Reloading   // 재장전 중 : 이동X, 발사X  
    }
    public LineRenderer line;

    public State state { get; private set; } // 현재 총의 상태
                                             // 탄
    [SerializeField]
    private GameObject prefabBullet;

    private GameObject barrel;
    private GameObject body;

    [SerializeField]
    private GameObject poolingObjectPrefab;

    Vector3 rotate;
    public int speed;
    private float x, y;

    public float power;
    public bool beforePower;
    private float maxPower;

    // 미사일 개수
    public int bulletMax;
    public int bullet;

    // 목표지점
    [SerializeField]
    GameObject prefabPoint;

    GameObject mainPoint;

    // 재장전
    GameObject reload;
    [SerializeField]
    Slider reloadBar;
    float barTimer;

    private void Awake()
    {
        // 포신 움직임
        body = transform.GetChild(1).gameObject;
        barrel = body.transform.GetChild(0).gameObject;
        rotate = body.transform.eulerAngles;
        barrel.transform.rotation = Quaternion.Euler(-30, 0, 0);
        speed = 20;
        x = -30.0f;
        y = 0.0f;
        power = 4;
        maxPower = 35.0f;
        beforePower = false;
        bulletMax = 3;
        bullet = 3;
        mainPoint = Instantiate(prefabPoint);
        mainPoint.SetActive(true);

        // 재장전
        reload = GameObject.Find("Reload");
        reload.SetActive(false);
        reloadBar.value = 0.1f;
        barTimer = 0.1f;
    }

    private void OnEnable()
    {

    }

    // Start is called before the first frame update
    private void Start()
    {
        state = State.Idle;
    }

    private IEnumerator Test()
    {
        if (state == State.Delay)
        {
            Debug.Log("딜레이");
            yield return new WaitForSeconds(3.5f);
            if (bullet <= 0)
                state = State.Reloading;
            else
                state = State.Idle;
        }
        if (state == State.Reloading)
        {
            Debug.Log("장전중");
            yield return new WaitForSeconds(5.0f);
            bullet = bulletMax;
            Debug.Log("장전완료");
            reload.SetActive(false);
            barTimer = 0.1f;
            reloadBar.value = 0.1f;
            state = State.Idle;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                Move();
                Fire();
                break;
            case State.Delay:
                Move();
                break;
            case State.Reloading:
                ReloadBarMove();
                break;
        }

        // 궤적
        // 시작 위치
        var startPos = barrel.transform.GetChild(0).gameObject.transform.position;
        // 방향정해주기
        var directionPos = startPos - barrel.transform.position;
        // 기울기에 따라서 파워 변환 예정
        var destPos = directionPos.normalized * power;
        DrawPath(CalculateVelcoity(destPos, startPos, 1f));
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (y > -50)
            {
                y -= 1.5f * Time.deltaTime * speed;
                body.transform.rotation = Quaternion.Euler(body.transform.rotation.eulerAngles.x, y, body.transform.rotation.eulerAngles.z);
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (y < 50)
            {
                y += 1.5f * Time.deltaTime * speed;
                body.transform.rotation = Quaternion.Euler(body.transform.rotation.eulerAngles.x, y, body.transform.rotation.eulerAngles.z);
            }
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (x > -45)
            {
                x -= 1.0f * Time.deltaTime * speed;
                barrel.transform.rotation = Quaternion.Euler(x, barrel.transform.rotation.eulerAngles.y, barrel.transform.rotation.eulerAngles.z);
                //Debug.Log(x);
                //Debug.Log(barrel.transform.rotation);
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (x < -15)
            {
                x += 1.0f * Time.deltaTime * speed;
                barrel.transform.rotation = Quaternion.Euler(x, barrel.transform.rotation.eulerAngles.y, barrel.transform.rotation.eulerAngles.z);
            }
        }

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    state = State.Reloading;
        //}
    }

    private void Fire()
    {
        if (bullet > 0)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (beforePower)
                {
                    power = 2;
                    beforePower = false;
                }
                if (power < maxPower)
                    power += 10 * Time.deltaTime;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                if (!beforePower)
                {
                    beforePower = true;
                }
                // 생성 하고
                var missile = ObjectPool.Instance.GetObject();
                // 날리는 방향
                // 시작 위치
                var startPos = barrel.transform.GetChild(0).gameObject.transform.position;
                // 방향정해주기
                var directionPos = startPos - barrel.transform.position;
                // 기울기에 따라서 파워 변환 예정
                var destPos = directionPos.normalized * power;/*(파워)*/
                // destPos.y = 0;
                missile.GetComponent<missile>().Shoot(startPos, destPos);
                // DrawPath(CalculateVelcoity(destPos, startPos, 1f));

                // 내 벡터 - 목표 벡터 = 목표벡터
                // 목표벡터 + 파워(간격)

                bullet--;
                if (bullet <= 0)
                    state = State.Reloading;
                else
                    state = State.Delay;
                StartCoroutine("Test");
            }
        }
    }
    private Vector3 CalculateVelcoity(Vector3 target, Vector3 origin, float time)
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

    private void DrawPath(Vector3 velocity)
    {
        //line.transform.position = barrel.transform.GetChild(1).gameObject.transform.position;
        Vector3 previousDrawPoint = line.transform.position;
        line.positionCount = 0;
        int resolution = (int)(100 * velocity.magnitude * velocity.y);
        //line.positionCount = resolution;
        //line.SetPosition(0, barrel.transform.GetChild(1).gameObject.transform.position);
        List<Vector3> index = new List<Vector3>();
        index.Add(barrel.transform.GetChild(0).gameObject.transform.position);

        Debug.Log(resolution);
        Debug.Log(velocity.magnitude);
        Debug.Log(velocity.y);

        for (int i = 1; i <= resolution; i++)
        {
            //float simulationTime = i / (float)resolution * launchData.timeToTarget;
            float simulationTime = i / (float)resolution * 10f;

            Vector3 displacement = velocity * simulationTime + Vector3.up * Physics.gravity.y * simulationTime * simulationTime / 2f;

            Vector3 drawPoint = barrel.transform.GetChild(0).gameObject.transform.position + displacement;
            index.Add(drawPoint);
            //line.SetPosition(i, drawPoint);
            previousDrawPoint = drawPoint;
            if (previousDrawPoint.y <= 0.0f)
            {
                line.positionCount = index.Count;
                line.SetPositions(index.ToArray());
                // 목표지점 그리기
                mainPoint.transform.position = new Vector3(previousDrawPoint.x, 0.1f, previousDrawPoint.z);

                return;
            }
        }
    }

    // 재장전
    private void ReloadBarMove()
    {
        if (reload.activeSelf == false)
            reload.SetActive(true);

        if (reload.activeSelf == true)
        {
            barTimer += Time.deltaTime;
            if (reloadBar.value < 1)
                reloadBar.value = barTimer / 5f;
        }
    }
}

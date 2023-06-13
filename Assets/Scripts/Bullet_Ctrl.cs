using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    Vector3 m_DirVec = Vector3.right;   //날아가야 할 방향 벡터
    float m_MoveSpeed = 15.0f;          //이동속도

    [HideInInspector] public bool isHoming = false;
    [HideInInspector] public bool isTarget= false;
    [HideInInspector] public GameObject targetObj = null;

    Vector3 m_DesiredDir;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isHoming == true)
        {
            if(targetObj == null && isTarget == false)
            {
                FindEnemy();
            }

            if(targetObj != null)
            {
                BulletHoming();
            }
            else // 타겟 사망시
            {
                transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;
            }
        }
        else
        {
            transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;
        }

        if (CameraResolution.m_ScreenWMax.x + 1.0f < transform.position.x || 
           transform.position.x < CameraResolution.m_ScreenWMin.x - 1.0f ||
           CameraResolution.m_ScreenWMax.y + 1.0f < transform.position.y ||
           transform.position.y < CameraResolution.m_ScreenWMin.y - 1.0f)
        { //총알이 화면을 벗어나면... 즉시 제거

            Destroy(gameObject);
        }
    }//void Update()

    public void BulletSpawn(Vector3 a_StPos, Vector3 a_DirVec,
                                float a_MySpeed = 15.0f, float att = 20.0f)
    {
        m_DirVec = a_DirVec;
        transform.position = new Vector3(a_StPos.x, a_StPos.y, 0.0f);
        m_MoveSpeed = a_MySpeed;
    }

    void FindEnemy()
    {
        GameObject[] a_EnemyList = GameObject.FindGameObjectsWithTag("Monster");

        if(a_EnemyList.Length <= 0)
        {
            return;
        }

        GameObject a_FindMon = null;
        float a_CalcDist = 0.0f;
        Vector3 a_CalcVec = Vector3.zero;
        for(int i = 0; i < a_EnemyList.Length; i++)
        {
            a_CalcVec = a_EnemyList[i].transform.position - transform.position;
            a_CalcVec.z = 0.0f;
            a_CalcDist = a_CalcVec.magnitude;

            if(4.0f < a_CalcDist) //총알로부터 4m이내의 몬스터만
            {
                continue;
            }

            a_FindMon = a_EnemyList[i].gameObject;
            break;
        }

        targetObj = a_FindMon;
        if(targetObj != null)
        {
            isTarget = true;
        }
    }

    void BulletHoming() //타겟을 향해 추적 이동하는 행동 패턴 함수
    {
        m_DesiredDir = targetObj.transform.position - transform.position;
        m_DesiredDir.z = 0.0f;
        m_DesiredDir.Normalize();

        float angle = Mathf.Atan2(m_DesiredDir.y, m_DesiredDir.x) * Mathf.Rad2Deg;
        Quaternion angleAxiz = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = angleAxiz;
        m_DirVec = transform.right;
        transform.Translate(Vector3.right * m_MoveSpeed * Time.deltaTime, Space.Self);
    }
}

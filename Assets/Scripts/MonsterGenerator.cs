using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] MonPrefab;

    float m_SpDelta = 0.0f;     //스폰 주기 계산용 변수
    float m_DiffSpawn = 1.0f;   //난이도에 따른 몬스터 스폰 주기 변수

    public static float m_StartTime = 0.0f;

    public static float m_BossSpTimer = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_StartTime = Time.time; //게임이 시작된 후 흐른 시간

        m_BossSpTimer = 20.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_SpDelta -= Time.deltaTime;
        if (m_SpDelta < 0.0f)
        {
            GameObject Go = null;

            int dice = Random.Range(1, 11);
            if (dice > 4)
            {
                Go = Instantiate(MonPrefab[0]) as GameObject;    //좀비스폰
            }
            else
            {
                Go = Instantiate(MonPrefab[1]) as GameObject;    //미사일스폰
            }


            float py = Random.Range(-3.0f, 3.0f);
            Go.transform.position =
                    new Vector3(CameraResolution.m_ScreenWMax.x + 1.0f, py, 0.0f);

            m_SpDelta = m_DiffSpawn;

        }//if(m_SpDelta < 0.0f)

        // 보스 스폰
        if(0.0f < m_BossSpTimer)
        {
            m_BossSpTimer -= Time.deltaTime;
            if(m_BossSpTimer <= 0.0f)
            {
                GameObject go = Instantiate(MonPrefab[2]) as GameObject;
                float py = Random.Range(-3.0f, 3.0f);
                go.transform.position = new Vector3(CameraResolution.m_ScreenWMax.x + 1.0f, py, 0.0f);
            }
        }

    }//void Update()
}

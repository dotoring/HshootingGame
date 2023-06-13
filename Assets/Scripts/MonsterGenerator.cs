using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] MonPrefab;

    float m_SpDelta = 0.0f;     //���� �ֱ� ���� ����
    float m_DiffSpawn = 1.0f;   //���̵��� ���� ���� ���� �ֱ� ����

    public static float m_StartTime = 0.0f;

    public static float m_BossSpTimer = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_StartTime = Time.time; //������ ���۵� �� �帥 �ð�

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
                Go = Instantiate(MonPrefab[0]) as GameObject;    //������
            }
            else
            {
                Go = Instantiate(MonPrefab[1]) as GameObject;    //�̻��Ͻ���
            }


            float py = Random.Range(-3.0f, 3.0f);
            Go.transform.position =
                    new Vector3(CameraResolution.m_ScreenWMax.x + 1.0f, py, 0.0f);

            m_SpDelta = m_DiffSpawn;

        }//if(m_SpDelta < 0.0f)

        // ���� ����
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

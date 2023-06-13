using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Ctrl : MonoBehaviour
{
    [HideInInspector] public Hero_Ctrl m_RefHero = null;
    float m_MoveSpeed = 4.0f;
    float m_MagnetSpeed = 9.0f;
    Vector3 m_MoveDir;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10.0f); //10초가 지나면 파괴
    }

    // Update is called once per frame
    void Update()
    {
        bool isMagnet = false;
        if(m_RefHero != null)
        {
            m_MoveDir = m_RefHero.transform.position - transform.position;
            m_MoveDir.z = 0.0f;
            if(m_MoveDir.magnitude < 3.0f)
            {
                m_MoveDir.Normalize();
                transform.position += m_MoveDir * Time.deltaTime * m_MagnetSpeed;
                isMagnet = true;
            }
        }

        if(isMagnet == false)
        {
            transform.position += Vector3.left * Time.deltaTime * m_MoveSpeed;
        }

        if(transform.position.x < CameraResolution.m_ScreenWMin.x - 0.5f)
        {
            Destroy(gameObject);
        }
    }
}

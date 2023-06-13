using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubHero_Ctrl : MonoBehaviour
{
    Hero_Ctrl m_RefHero = null;
    float angle = 0.0f; 
    float radius = 1.0f;
    float speed = 100.0f;

    Vector3 ParentPos = Vector3.zero;

    float m_LifeTime = 0.0f;

    public GameObject m_BulletObj = null;
    float m_AttSpeed = 0.5f;
    float m_ShootCool = 0.0f;

    GameObject a_CloneObj = null;
    Bullet_Ctrl a_BulletSc = null;

    bool isHoming = false;
    bool isDuble = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_LifeTime -= Time.deltaTime;
        if (m_LifeTime <= 0.0f) {
            Destroy(gameObject);
            return;
        }

        angle += Time.deltaTime * speed;
        if(360.0f < angle)
        {
            angle -= 360.0f;
        }

        if (m_RefHero == null)
        {
            return;
        }

        ParentPos = m_RefHero.transform.position;
        transform.position = ParentPos + new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                                                    radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0.0f);

        FireUpdate();
    }

    public void SubHeroSpawn(Hero_Ctrl a_Parent, float a_Angle, float a_LifeTime)
    {
        m_RefHero = a_Parent;
        angle = a_Angle;
        m_LifeTime = a_LifeTime;
    }

    void FireUpdate()
    {
        if(m_RefHero != null)
        {
            isHoming = m_RefHero.IsHoming;
            if (0.0f < m_RefHero.m_DoubleOnTime)
            {
                isDuble = true;
            }
            else
            {
                isDuble = false;
            }
        }

        if(0.0f < m_ShootCool)
        {
            m_ShootCool -= Time.deltaTime;
        }

        if(m_ShootCool <= 0.0f)
        {
            if(isDuble == true)
            {
                Vector3 a_Pos;
                for(int i = 0; i <2; i++)
                {
                    a_CloneObj = (GameObject)Instantiate(m_BulletObj);
                    a_Pos = transform.position;
                    a_Pos.y += 0.2f - (i * 0.4f);
                    a_CloneObj.transform.position = a_Pos;
                    a_BulletSc = a_CloneObj.GetComponent<Bullet_Ctrl>();
                    if(a_BulletSc != null)
                    {
                        a_BulletSc.isHoming = isHoming;
                    }
                }
            }
            else
            {
                a_CloneObj = (GameObject)Instantiate(m_BulletObj);
                a_CloneObj.transform.position = transform.position;
                a_BulletSc = a_CloneObj.GetComponent<Bullet_Ctrl>();
                if(a_BulletSc != null)
                {
                    a_BulletSc.isHoming = isHoming;
                }
            }

            m_ShootCool = m_AttSpeed;
        }
    }
}

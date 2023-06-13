using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero_Ctrl : MonoBehaviour
{
    float h = 0.0f;
    float v = 0.0f;

    float moveSpeed = 7.0f;
    Vector3 moveDir = Vector3.zero;

    //--- ���ΰ��� ���� ������ ���� �� ������ ���� ���� ����
    Vector3 HalfSize    = Vector3.zero;
    Vector3 m_CacCurPos = Vector3.zero;
    //--- ���ΰ��� ���� ������ ���� �� ������ ���� ���� ����

    //--- �Ѿ� �߻� ����
    public GameObject m_BulletPrefab = null;
    public GameObject m_ShootPos = null;
    float m_ShootCool = 0.0f;           //�Ѿ� �߻� �ֱ� ���� ����
    //--- �Ѿ� �߻� ����

    //Wolf ��ų
    public GameObject m_WolfPrefab = null;

    //���� ��ų
    float m_SdOnTime = 0.0f;
    float m_SdDuration = 12.0f;
    public GameObject shieldObj = null;

    //����ź ��ų
    [HideInInspector] public bool IsHoming = false;
    float m_HomingOnTime = 0.0f;
    float m_HomingDur = 12.0f;

    //���� ��ų
    [HideInInspector] public float m_DoubleOnTime = 0.0f;
    float m_DoubleDur = 12.0f;

    //��ȯ�� ��ų
    int subCount = 3;
    float m_SubOnTime = 0.0f;
    float m_SubDur = 12.0f;
    public GameObject subHeroPrefab = null;
    public GameObject subParent = null;

    //--- ���ΰ� ü�� ����
    float m_MaxHP = 200.0f;
    [HideInInspector] public float m_CurHP = 200.0f;
    public Image m_HpBar = null;
    //--- ���ΰ� ü�� ����

    // Start is called before the first frame update
    void Start()
    {
        //--- ĳ������ ���� �ݻ�����, ���� �ݻ����� ���ϱ�
        //���忡 �׷��� ��������Ʈ ������ ������
        SpriteRenderer sprRend =
            gameObject.GetComponentInChildren<SpriteRenderer>();
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f; //������ Ŀ�� ���� ����
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f;
        HalfSize.z = 1.0f;
        //���忡 �׷��� ��������Ʈ ������ ������
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");    // -1.0f ~ 1.0f
        v = Input.GetAxis("Vertical");

        if(h != 0.0f || v != 0.0f)
        {
            moveDir = new Vector3(h, v, 0);
            if (1.0f < moveDir.magnitude)
                moveDir.Normalize();

            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        LimitMove();

        FireUpdate();

        UpdateSkill();

    }//void Update()

    void LimitMove()
    {
        m_CacCurPos = transform.position;

        if (m_CacCurPos.x < CameraResolution.m_ScreenWMin.x + HalfSize.x)
            m_CacCurPos.x = CameraResolution.m_ScreenWMin.x + HalfSize.x;

        if (CameraResolution.m_ScreenWMax.x - HalfSize.x < m_CacCurPos.x)
            m_CacCurPos.x = CameraResolution.m_ScreenWMax.x - HalfSize.x;

        if (m_CacCurPos.y < CameraResolution.m_ScreenWMin.y + HalfSize.y)
            m_CacCurPos.y = CameraResolution.m_ScreenWMin.y + HalfSize.y;

        if (CameraResolution.m_ScreenWMax.y - HalfSize.y < m_CacCurPos.y)
            m_CacCurPos.y = CameraResolution.m_ScreenWMax.y - HalfSize.y;

        transform.position = m_CacCurPos;

    }//void LimitMove()

    void FireUpdate()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;

        if(m_ShootCool <= 0.0f)
        {
            Bullet_Ctrl a_BulletSc = null;

            if(0.0f < m_DoubleOnTime)
            {
                Vector3 a_Pos;
                GameObject a_CloneObj;
                for(int i = 0; i < 2; i++)
                {
                    a_CloneObj = (GameObject)Instantiate(m_BulletPrefab);
                    a_Pos = m_ShootPos.transform.position;
                    a_Pos.y += 0.2f - (i * 0.4f);
                    a_CloneObj.transform.position = a_Pos;
                    a_BulletSc = a_CloneObj.GetComponent<Bullet_Ctrl>();
                    if (a_BulletSc != null)
                    {
                        a_BulletSc.isHoming = IsHoming;
                    }
                }
            }
            else
            {
                GameObject a_CloneObj = Instantiate(m_BulletPrefab) as GameObject;
                a_CloneObj.transform.position = m_ShootPos.transform.position;
                a_BulletSc = a_CloneObj.GetComponent<Bullet_Ctrl>();
                if (a_BulletSc != null)
                {
                    a_BulletSc.isHoming = IsHoming;
                }
            }

            Sound_Mgr.Instance.PlayEffSound("gun", 0.5f);

            m_ShootCool = 0.15f;
        }
    }//void FireUpdate()

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.tag == "Monster")
        {
            TakeDamage(50.0f);
            Monster_Ctrl a_RefMon = coll.gameObject.GetComponent<Monster_Ctrl>();
            if (a_RefMon != null)
                a_RefMon.TakeDamage(1000);
        }
        else if(coll.tag == "EnemyBullet")
        {
            TakeDamage(20.0f);
            Destroy(coll.gameObject);
        }
        else if(coll.gameObject.name.Contains("CoinPrefab"))
        {
            Game_Mgr.Inst.AddGold();
            Destroy(coll.gameObject);
        }
        else if(coll.gameObject.name.Contains("Heart"))
        {
            m_CurHP += m_MaxHP * 0.5f;
            Game_Mgr.Inst.DamageText(m_MaxHP * 0.5f, transform.position, new Color(0.18f, 0.5f, 0.34f));
            if(m_MaxHP < m_CurHP)
            {
                m_CurHP = m_MaxHP;
            }
            if(m_HpBar != null)
            {
                m_HpBar.fillAmount = m_CurHP / m_MaxHP;
            }
            Destroy(coll.gameObject);
        }
    }

    void TakeDamage(float a_Value)
    {
        if (m_CurHP <= 0.0f)
            return;

        if(0.0f < m_SdOnTime)
        {
            return;
        }

        Game_Mgr.Inst.DamageText(-a_Value, transform.position, Color.blue);

        m_CurHP -= a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        if (m_HpBar != null)
            m_HpBar.fillAmount = m_CurHP / m_MaxHP;

        if(m_CurHP <= 0.0f)
        { //���ó��
            Time.timeScale = 0.0f;  //�Ͻ�����
        }
    }

    void UpdateSkill()
    {
        //���� ���� ������Ʈ
        if(0.0f < m_SdOnTime)
        {
            m_SdOnTime -= Time.deltaTime;
            if(shieldObj != null && shieldObj.activeSelf == false)
            {
                shieldObj.SetActive(true);
            }
        }
        else
        {
            if (shieldObj != null && shieldObj.activeSelf == true)
            {
                shieldObj.SetActive(false);
            }
        }

        //����ź ���� ������Ʈ
        if(0.0f < m_HomingOnTime)
        {
            m_HomingOnTime -= Time.deltaTime;
            if(m_HomingOnTime < 0.0f)
            {
                m_HomingOnTime = 0.0f;
            }

            IsHoming = true;
        }
        else
        {
            IsHoming = false;
        }

        //���� ���� 
        if(0.0f < m_DoubleDur) {
            m_DoubleOnTime -= Time.deltaTime;

            if(m_DoubleOnTime < 0.0f){
                m_DoubleOnTime = 0.0f;
            }
        }

        //��ȯ�� ��� ������Ʈ
        if(0.0f < m_SubOnTime)
        {
            m_SubOnTime -= Time.deltaTime;

            if(m_SubOnTime < 0.0f)
            {
                m_SubOnTime = 0.0f;
            }
        }
    }

    public void UseSkill(SkillType a_SkType)
    {
        if(m_CurHP <= 0)
        {
            return;
        }

        if(a_SkType == SkillType.Skill_0) //��
        {
            m_CurHP += m_MaxHP * 0.5f;
            if(m_CurHP > m_MaxHP)
            {
                m_CurHP = m_MaxHP;
            }
            Game_Mgr.Inst.DamageText(m_MaxHP * 0.5f, transform.position, new Color(0.18f, 0.5f, 0.34f));

            if(m_HpBar != null)
            {
                m_HpBar.fillAmount = m_CurHP / m_MaxHP;
            }
        }
        else if(a_SkType == SkillType.Skill_1) //���� ��ų
        {
            GameObject a_CloneObj = Instantiate(m_WolfPrefab) as GameObject;
            a_CloneObj.transform.position = new Vector3(CameraResolution.m_ScreenWMin.x - 1.0f, 0.0f, 0.0f);
        }
        else if(a_SkType == SkillType.Skill_2) //��ȣ��
        {
            if(0.0f < m_SdOnTime)
            {
                return;
            }

            m_SdOnTime = m_SdDuration;

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_SdOnTime, m_SdDuration);
        }
        else if(a_SkType == SkillType.Skill_3) //����ź
        {
            if(0.0f < m_HomingOnTime)
            {
                return;
            }

            m_HomingOnTime = m_HomingDur;

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_HomingOnTime, m_HomingDur);
        }
        else if (a_SkType == SkillType.Skill_4) //����
        {
            if (0.0f < m_DoubleOnTime)
            {
                return;
            }

            m_DoubleOnTime = m_DoubleDur;

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_DoubleOnTime, m_DoubleDur);
        }
        else if (a_SkType == SkillType.Skill_5) //��ȯ��
        {
            if(0.0f < m_SubOnTime)
            {
                return;
            }

            subCount = 3;
            m_SubOnTime = m_SubDur;

            for (int i = 0; i < subCount; i++)
            {
                GameObject obj = Instantiate(subHeroPrefab) as GameObject;
                obj.transform.SetParent(subParent.transform);
                SubHero_Ctrl sub = obj.GetComponent<SubHero_Ctrl>();
                if(sub != null)
                {
                    sub.SubHeroSpawn(this, (360 / subCount) * i, m_SubOnTime);
                }
            }

            Game_Mgr.Inst.SkillCoolMethod(a_SkType, m_SubOnTime, m_SubOnTime);
        }

        GlobalValue.m_SkDataList[(int)a_SkType].m_CurSkillCount--;
    }
}

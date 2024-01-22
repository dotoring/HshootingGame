using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    MT_Zombi,
    MT_Missile,
    MT_Boss
}

public enum BossState
{
    BS_Move,
    BS_NormalAtt,
    BS_FeverAtt
}

public class Monster_Ctrl : MonoBehaviour
{
    public MonType m_MonType = MonType.MT_Zombi;

    //--- ���� ü�� ����
    float m_MaxHP = 200.0f;
    float m_CurHP = 200.0f;
    public Image m_HpBar = null;
    //--- ���� ü�� ����

    float m_Speed = 4.0f;   //�̵��ӵ�
    Vector3 m_CurPos;       //��ġ ���� ����
    Vector3 m_SpawnPos;     //���� ��ġ

    float m_CacPosY = 0.0f; //���� �Լ��� �� ���� ���� ���� ����
    float m_RandY = 0.0f;   //������ ������ ����� ����
    float m_CycleY = 0.0f;  //������ ���� �ӵ� ����

    //--- �Ѿ� �߻� ���� ���� ����
    public GameObject m_ShootPos  = null;
    public GameObject m_BulletObj = null;
    float shoot_Time = 0.0f;        //�Ѿ� �߻� �ֱ� ���� ����
    float shoot_Delay = 1.5f;       //�Ѿ� �� Ÿ��
    float BulletMySpeed = 10.0f;    //�Ѿ� �̵� �ӵ�
    //--- �Ѿ� �߻� ���� ���� ����

    //--- �̻��� �ൿ���Ͽ� �ʿ��� ����
    Hero_Ctrl m_RefHero = null;
    Vector3 m_DirVec;

    // ������ �ൿ ���� ���� ����
    BossState m_BossState = BossState.BS_Move;
    int m_ShootCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_RefHero = GameObject.FindObjectOfType<Hero_Ctrl>();

        m_SpawnPos = transform.position;
        m_RandY  = Random.Range(0.2f, 2.6f); //Sin �Լ��� ���� ����
        m_CycleY = Random.Range(1.0f, 2.2f);

        if (m_MonType == MonType.MT_Boss)
        {
            m_MaxHP = 3000.0f;
            m_CurHP = m_MaxHP;

            m_BossState = BossState.BS_Move;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (m_MonType == MonType.MT_Zombi)  //���Ͱ� �����϶�
            Zombi_AI_Update();
        else if (m_MonType == MonType.MT_Missile)   //���Ͱ� �̻����϶�
        {
            MissileAIUpdate();
        }
        else if (m_MonType == MonType.MT_Boss)  //���Ͱ� �����϶�
        {
            BossAIUpdate();
        }

        if (this.transform.position.x < CameraResolution.m_ScreenWMin.x - 2.0f)
            Destroy(gameObject);     //���� ȭ�� ���� ����� ��� ����
    }

    void Zombi_AI_Update()
    {
        m_CurPos = transform.position;
        m_CurPos.x += (-1.0f * Time.deltaTime * m_Speed);   //�������� �̵��ϵ���
        m_CacPosY += Time.deltaTime * (m_Speed / m_CycleY); //���Ʒ��� ���ڿ �ϵ���
        m_CurPos.y = m_SpawnPos.y + Mathf.Sin(m_CacPosY) * m_RandY;
        transform.position = m_CurPos;

        //������ ���۵ǰ� 30�� ������ �Ѿ� �߻� x
        if(Time.time - MonsterGenerator.m_StartTime < 30.0f)
        {
            return;
        }

        //--- �Ѿ� �߻� 
        if (m_BulletObj == null)    //�Ѿ��� ������ return
            return;

        //���� �ð����� ���� ������ �Ѿ� �߻�
        shoot_Time += Time.deltaTime;
        if(shoot_Delay <= shoot_Time)
        {
            GameObject a_NewObj = Instantiate(m_BulletObj) as GameObject;
            Bullet_Ctrl a_BulletSc = a_NewObj.GetComponent<Bullet_Ctrl>();
            a_BulletSc.BulletSpawn(m_ShootPos.transform.position,
                                            Vector3.left, BulletMySpeed);
            shoot_Time = 0.0f;
        }
    }

    void MissileAIUpdate()
    {
        m_CurPos = transform.position;
        m_DirVec.x = -1.0f;
        m_DirVec.y = 0.0f;
        m_DirVec.z = 0.0f;

        if(m_RefHero != null)
        {
            //�̻����� ���ΰ��� ���� �����ǵ���
            Vector3 a_calcVal = m_RefHero.transform.position - transform.position;
            m_DirVec = a_calcVal;

            //�̻����� ���ΰ����� �Ÿ��� ������������ 3.5m �̻��̸� ������ ��ȭ����
            //�������θ� �̵���Ű���� �ǵ�
            if(a_calcVal.x < -3.5f)
            {
                m_DirVec.y = 0.0f;
            }
        }

        m_DirVec.Normalize();
        m_DirVec.x = -1.0f; //������ ���� �������� �̵��ϰ� �ϱ� ���ؼ�
        m_DirVec.z = 0.0f;

        m_CurPos += (m_DirVec * Time.deltaTime * m_Speed);
        transform.position = m_CurPos;
    }

    void BossAIUpdate()
    {
        if(m_BossState == BossState.BS_Move)    //���� �̵�����
        {
            m_CurPos = this.transform.position;
            float a_ArrivePos = CameraResolution.m_ScreenWMax.x - 1.9f; //������ġ ����
            if(a_ArrivePos < m_CurPos.x)
            {
                m_CurPos.x += (-1.0f * Time.deltaTime * m_Speed);
                if(m_CurPos.x <= a_ArrivePos)   //������ġ�� ���� Ư������ ���·� ����
                {
                    shoot_Time = 1.28f;
                    m_BossState = BossState.BS_FeverAtt;
                }
            }

            this.transform.position = m_CurPos;
        }
        else if(m_BossState == BossState.BS_NormalAtt)  //���� �⺻���� ����
        {
            shoot_Time -= Time.deltaTime;
            if (shoot_Time <= 0.0f)
            {
                //�⺻ ���� �Ѿ��� ���ΰ��� ���� ���ƿ��� ����
                //���ϴ� ������ ���� �̹����� ȸ���ǵ��� ����
                Vector3 a_TargetV = m_RefHero.transform.position - this.transform.position;
                a_TargetV.Normalize();
                GameObject a_NewObj = (GameObject)Instantiate(m_BulletObj);
                Bullet_Ctrl a_BulletSc = a_NewObj.GetComponent<Bullet_Ctrl>();
                a_BulletSc.BulletSpawn(m_ShootPos.transform.position, a_TargetV, BulletMySpeed);
                float a_CalcAngle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                a_CalcAngle += 180.0f;
                a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, a_CalcAngle);

                //7���� �⺻ ���� �� Ư���������� ��ȯ
                m_ShootCount++;
                if(m_ShootCount < 7)
                {
                    shoot_Time = 0.7f;
                }
                else
                {
                    m_ShootCount = 0;
                    shoot_Time = 2.0f;
                    m_BossState = BossState.BS_FeverAtt;
                }
            }
        }
        else if (m_BossState == BossState.BS_FeverAtt)  //���� Ư������ ����
        {
            shoot_Time -= Time.deltaTime;
            if (shoot_Time <= 0.0f)
            {
                //�������� 15������ �Ѿ��� �߻��ϵ��� ����
                float Radius = 10.0f;
                Vector3 a_TargetV = Vector3.zero;
                GameObject a_NewObj = null;
                Bullet_Ctrl a_BulletSc = null;
                float a_CalcAngle = 0.0f;
                for(float Angle = 0.0f; Angle < 360.0f; Angle += 15.0f)
                {
                    a_TargetV.x = Radius * Mathf.Cos(Angle * Mathf.Deg2Rad);
                    a_TargetV.y = Radius * Mathf.Sin(Angle * Mathf.Deg2Rad);
                    a_TargetV.Normalize();
                    a_NewObj = (GameObject)Instantiate(m_BulletObj);
                    a_BulletSc = a_NewObj.GetComponent<Bullet_Ctrl>();
                    a_BulletSc.BulletSpawn(this.transform.position, a_TargetV, BulletMySpeed);
                    a_CalcAngle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                    a_CalcAngle += 180.0f;
                    a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, a_CalcAngle);
                }

                Sound_Mgr.Instance.PlayEffSound("explosion_large_01", 0.8f);

                //3���� Ư������ �� �⺻�������� ��ȯ
                m_ShootCount++;
                if(m_ShootCount < 3)
                {
                    shoot_Time = 1.0f;
                }
                else
                {
                    m_ShootCount = 0;
                    shoot_Time = 1.5f;
                    m_BossState = BossState.BS_NormalAtt;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        //���� ���忡���� ���ΰ��� �� �Ѿ˸� �������� �߻��ϵ��� ó��
        if(coll.tag == "AllyBullet")
        {
            TakeDamage(80.0f);
            Destroy(coll.gameObject); //���Ϳ� �浹�� �Ѿ� ����
        }
    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHP <= 0.0f)
            return;

        //������ ����
        float a_CacDmg = a_Value;
        if (m_CurHP < a_Value)
            a_CacDmg = m_CurHP;

        Game_Mgr.Inst.DamageText(-a_CacDmg, transform.position, Color.red);

        //������ ��ŭ ���� ü�� ����
        m_CurHP -= a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        //���� ü�� ������ ����
        if (m_HpBar != null)
            m_HpBar.fillAmount = m_CurHP / m_MaxHP;

        if(m_CurHP <= 0.0f)
        {
            Destroy(gameObject); //���� GameObject ����
            //���� ����
            Game_Mgr.Inst.AddScore();

            //��� ����
            Game_Mgr.Inst.SpawnCoin(transform.position);
            //��Ʈ ����
            if(m_MonType == MonType.MT_Boss)
            {
                Game_Mgr.Inst.SpawnHeart(transform.position);
            }

            //����� ���Ͱ� �����ϰ�� ������ �����ֱ� ����
            if(m_MonType == MonType.MT_Boss)
            {
                MonsterGenerator.m_BossSpTimer = Random.Range(8.0f, 10.0f);
            }
        }
    }
}

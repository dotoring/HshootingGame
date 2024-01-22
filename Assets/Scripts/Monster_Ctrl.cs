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

    //--- 몬스터 체력 변수
    float m_MaxHP = 200.0f;
    float m_CurHP = 200.0f;
    public Image m_HpBar = null;
    //--- 몬스터 체력 변수

    float m_Speed = 4.0f;   //이동속도
    Vector3 m_CurPos;       //위치 계산용 변수
    Vector3 m_SpawnPos;     //스폰 위치

    float m_CacPosY = 0.0f; //사인 함수에 들어갈 누적 각도 계산용 변수
    float m_RandY = 0.0f;   //랜덤한 진폭값 저장용 변수
    float m_CycleY = 0.0f;  //랜덤한 진동 속도 변수

    //--- 총알 발사 관련 변수 선언
    public GameObject m_ShootPos  = null;
    public GameObject m_BulletObj = null;
    float shoot_Time = 0.0f;        //총알 발사 주기 계산용 변수
    float shoot_Delay = 1.5f;       //총알 쿨 타임
    float BulletMySpeed = 10.0f;    //총알 이동 속도
    //--- 총알 발사 관련 변수 선언

    //--- 미사일 행동패턴에 필요한 변수
    Hero_Ctrl m_RefHero = null;
    Vector3 m_DirVec;

    // 보스의 행동 패턴 관련 변수
    BossState m_BossState = BossState.BS_Move;
    int m_ShootCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_RefHero = GameObject.FindObjectOfType<Hero_Ctrl>();

        m_SpawnPos = transform.position;
        m_RandY  = Random.Range(0.2f, 2.6f); //Sin 함수의 랜덤 진폭
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
        if (m_MonType == MonType.MT_Zombi)  //몬스터가 좀비일때
            Zombi_AI_Update();
        else if (m_MonType == MonType.MT_Missile)   //몬스터가 미사일일때
        {
            MissileAIUpdate();
        }
        else if (m_MonType == MonType.MT_Boss)  //몬스터가 보스일때
        {
            BossAIUpdate();
        }

        if (this.transform.position.x < CameraResolution.m_ScreenWMin.x - 2.0f)
            Destroy(gameObject);     //왼쪽 화면 끝을 벗어나면 즉시 제거
    }

    void Zombi_AI_Update()
    {
        m_CurPos = transform.position;
        m_CurPos.x += (-1.0f * Time.deltaTime * m_Speed);   //전방으로 이동하도록
        m_CacPosY += Time.deltaTime * (m_Speed / m_CycleY); //위아래로 진자운동 하도록
        m_CurPos.y = m_SpawnPos.y + Mathf.Sin(m_CacPosY) * m_RandY;
        transform.position = m_CurPos;

        //게임이 시작되고 30초 전까지 총알 발사 x
        if(Time.time - MonsterGenerator.m_StartTime < 30.0f)
        {
            return;
        }

        //--- 총알 발사 
        if (m_BulletObj == null)    //총알이 없으면 return
            return;

        //일정 시간마다 좀비 몬스터의 총알 발사
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
            //미사일이 주인공을 향해 유도되도록
            Vector3 a_calcVal = m_RefHero.transform.position - transform.position;
            m_DirVec = a_calcVal;

            //미사일이 주인공과의 거리가 우측방향으로 3.5m 이상이면 높낮이 변화없이
            //좌측으로만 이동시키려는 의도
            if(a_calcVal.x < -3.5f)
            {
                m_DirVec.y = 0.0f;
            }
        }

        m_DirVec.Normalize();
        m_DirVec.x = -1.0f; //무조건 왼쪽 방향으로 이동하게 하기 위해서
        m_DirVec.z = 0.0f;

        m_CurPos += (m_DirVec * Time.deltaTime * m_Speed);
        transform.position = m_CurPos;
    }

    void BossAIUpdate()
    {
        if(m_BossState == BossState.BS_Move)    //보스 이동상태
        {
            m_CurPos = this.transform.position;
            float a_ArrivePos = CameraResolution.m_ScreenWMax.x - 1.9f; //도착위치 설정
            if(a_ArrivePos < m_CurPos.x)
            {
                m_CurPos.x += (-1.0f * Time.deltaTime * m_Speed);
                if(m_CurPos.x <= a_ArrivePos)   //도착위치에 오면 특수공격 상태로 변경
                {
                    shoot_Time = 1.28f;
                    m_BossState = BossState.BS_FeverAtt;
                }
            }

            this.transform.position = m_CurPos;
        }
        else if(m_BossState == BossState.BS_NormalAtt)  //보스 기본공격 상태
        {
            shoot_Time -= Time.deltaTime;
            if (shoot_Time <= 0.0f)
            {
                //기본 공격 총알이 주인공을 향해 날아오게 설정
                //향하는 방향을 향해 이미지가 회전되도록 설정
                Vector3 a_TargetV = m_RefHero.transform.position - this.transform.position;
                a_TargetV.Normalize();
                GameObject a_NewObj = (GameObject)Instantiate(m_BulletObj);
                Bullet_Ctrl a_BulletSc = a_NewObj.GetComponent<Bullet_Ctrl>();
                a_BulletSc.BulletSpawn(m_ShootPos.transform.position, a_TargetV, BulletMySpeed);
                float a_CalcAngle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                a_CalcAngle += 180.0f;
                a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, a_CalcAngle);

                //7번의 기본 공격 후 특수공격으로 전환
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
        else if (m_BossState == BossState.BS_FeverAtt)  //보스 특수공격 상태
        {
            shoot_Time -= Time.deltaTime;
            if (shoot_Time <= 0.0f)
            {
                //전방위로 15도마다 총알을 발사하도록 설정
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

                //3번의 특수공격 후 기본공격으로 전환
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
        //몬스터 입장에서는 주인공이 쏜 총알만 데미지가 발생하도록 처리
        if(coll.tag == "AllyBullet")
        {
            TakeDamage(80.0f);
            Destroy(coll.gameObject); //몬스터에 충돌된 총알 삭제
        }
    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHP <= 0.0f)
            return;

        //데미지 설정
        float a_CacDmg = a_Value;
        if (m_CurHP < a_Value)
            a_CacDmg = m_CurHP;

        Game_Mgr.Inst.DamageText(-a_CacDmg, transform.position, Color.red);

        //데미지 만큼 몬스터 체력 차감
        m_CurHP -= a_Value;
        if (m_CurHP < 0.0f)
            m_CurHP = 0.0f;

        //몬스터 체력 게이지 변경
        if (m_HpBar != null)
            m_HpBar.fillAmount = m_CurHP / m_MaxHP;

        if(m_CurHP <= 0.0f)
        {
            Destroy(gameObject); //몬스터 GameObject 제거
            //점수 보상
            Game_Mgr.Inst.AddScore();

            //골드 보상
            Game_Mgr.Inst.SpawnCoin(transform.position);
            //하트 보상
            if(m_MonType == MonType.MT_Boss)
            {
                Game_Mgr.Inst.SpawnHeart(transform.position);
            }

            //사망한 몬스터가 보스일경우 보스의 스폰주기 설정
            if(m_MonType == MonType.MT_Boss)
            {
                MonsterGenerator.m_BossSpTimer = Random.Range(8.0f, 10.0f);
            }
        }
    }
}

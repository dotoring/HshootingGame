using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Skill_0 = 0,    //50%힐
    Skill_1,        //궁극기
    Skill_2,        //보호막
    Skill_3,        //유도탄
    Skill_4,        //더블샷
    Skill_5,        //소환수
    SkCount,
}

public class Skill_Info  //각 Item 정보
{
    public string m_Name = "";                  //캐릭터 이름
    public SkillType m_SkType = SkillType.Skill_0; //캐릭터 타입
    public Vector2 m_IconSize = Vector2.one;  //아이콘의 가로 사이즈, 세로 사이즈
    public int m_Price = 100;   //아이템 기본 가격 
    public int m_UpPrice = 50; //업그레이드 가격, 타입에 따라서
    public int m_Level = 0;
    //그전엔 Lock, 레벨0 이면 아직 구매 안됨 (구매가 완료되면 레벨 1부터)
    public int m_CurSkillCount = 0;   //사용할 수 있는 스킬 카운트
    //public int m_MaxUsable = 1;     //사용할 수 있는 최대 스킬 카운트는 Level과 같다.
    public string m_SkillExp = "";    //스킬 효과 설명
    public Sprite m_IconImg = null;   //캐릭터 아이템에 사용될 이미지

    public void SetType(SkillType a_SkType)
    {
        m_SkType = a_SkType;

        if (a_SkType == SkillType.Skill_0)
        {
            m_Name = "강아지";
            m_IconSize.x = 0.766f; //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;   //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f = 103 픽셀

            m_Price = 100; //기본가격
            m_UpPrice = 50; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) 가격 필요

            m_SkillExp = "Hp 50% 회복";
            m_IconImg = Resources.Load("IconImg/m0011", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_1)
        {
            m_Name = "울버독";
            m_IconSize.x = 0.81f;    //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 200; //기본가격
            m_UpPrice = 100; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) 가격 필요

            m_SkillExp = "궁극기";
            m_IconImg = Resources.Load("IconImg/m0367", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_2)
        {
            m_Name = "구미호";
            m_IconSize.x = 0.946f;     //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 400; //기본가격
            m_UpPrice = 200; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) 가격 필요

            m_SkillExp = "보호막";
            m_IconImg = Resources.Load("IconImg/m0054", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_3)
        {
            m_Name = "야옹이";
            m_IconSize.x = 0.93f;     //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 800; //기본가격
            m_UpPrice = 400; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) 가격 필요

            m_SkillExp = "유도탄";
            m_IconImg = Resources.Load("IconImg/m0423", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_4)
        {
            m_Name = "드래곤";
            m_IconSize.x = 0.93f;     //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 1600; //기본가격
            m_UpPrice = 800; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) 가격 필요

            m_SkillExp = "더블샷";
            m_IconImg = Resources.Load("IconImg/m0244", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_5)
        {
            m_Name = "팅커벨";
            m_IconSize.x = 0.93f;    //세로에 대한 가로 비율
            m_IconSize.y = 1.0f;     //세로를 기준으로 잡을 것이기 때문에 그냥 1.0f

            m_Price = 3000;   //기본가격
            m_UpPrice = 1600; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) 가격 필요

            m_SkillExp = "소환수 공격";
            m_IconImg = Resources.Load("IconImg/m0172", typeof(Sprite)) as Sprite;
        }

    }//public void SetType(SkillType a_SkType)
}

public class GlobalValue
{
    //소환수 스킬 리스트
    public static List<Skill_Info> m_SkDataList = new List<Skill_Info>();

    public static string g_NickName = "";
    public static int g_BestScore = 0;
    public static int g_UserGold = 0;

    public static void LoadGameData()
    {
        //설정 데이터 로딩
        if(m_SkDataList.Count <= 0)
        {
            Skill_Info a_SkItemNd;
            for(int i = 0; i < (int)SkillType.SkCount; i++)
            {
                a_SkItemNd = new Skill_Info();
                a_SkItemNd.SetType((SkillType)i);
                m_SkDataList.Add(a_SkItemNd);
            }
        }

        //PlayerPrefs.SetInt("UserGold", 11110);

        g_NickName = PlayerPrefs.GetString("NickName", "AAA");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);

        //서버나 로컬에 저장된 보유 상태 로딩
        string a_KeyBuff = "";
        for(int i = 0; i < (int)SkillType.SkCount; i++)
        {
            if(m_SkDataList.Count <= i)
            {
                continue;
            }

            a_KeyBuff = string.Format("Skill_Item_{0}", i);
            m_SkDataList[i].m_Level = PlayerPrefs.GetInt(a_KeyBuff, 0);

            //m_SkDataList[i].m_Level = 3; //테스트를 위해 레벨을 3개씩 채우고 시작
        }
    }
}

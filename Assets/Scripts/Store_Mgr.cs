using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public Button m_BackBtn = null;
    public Text m_UserInfoTxt = null;

    public GameObject m_ItemScContent;  //scrollContent ���ϵ�� ������ parent��ü
    public GameObject m_ItemNodeObj;

    SkProductNode[] m_SkNodeList; // ��ũ�ѿ� �پ��ִ� item ��ϵ�

    SkillType m_BuySkType;
    int m_SvMyGold = 0;
    int m_SvMyLevel = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();
        
        if(m_BackBtn != null)
        {
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });
        }

        if(m_UserInfoTxt != null)
        {
            m_UserInfoTxt.text = "����(" + GlobalValue.g_NickName + ") : �������(" + GlobalValue.g_UserGold + ")";
        }

        //������ ��� �߰�
        GameObject a_ItemObj = null;
        SkProductNode a_SkItemNode = null;
        for(int i = 0; i < GlobalValue.m_SkDataList.Count; i++)
        {
            a_ItemObj = (GameObject)Instantiate(m_ItemNodeObj);
            a_SkItemNode = a_ItemObj.GetComponent<SkProductNode>();
            a_SkItemNode.InitData(GlobalValue.m_SkDataList[i].m_SkType);
            a_ItemObj.transform.SetParent(m_ItemScContent.transform, false);
        }

        RefreshSkItemList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshSkItemList()
    {
        if(m_ItemScContent != null)
        {
            if(m_SkNodeList == null || m_SkNodeList.Length <= 0)
            {
                m_SkNodeList = m_ItemScContent.GetComponentsInChildren<SkProductNode>();
            }
        }

        for(int i = 0; i < GlobalValue.m_SkDataList.Count; i++)
        {
            if(m_SkNodeList[i].m_SkType != GlobalValue.m_SkDataList[i].m_SkType)
            {
                continue;
            }
            if(GlobalValue.m_SkDataList[i].m_Level <= 0) //���� �� ����
            {
                m_SkNodeList[i].SetState(GlobalValue.m_SkDataList[i].m_Price,
                    GlobalValue.m_SkDataList[i].m_Level);
            }
            else //�̹� ���Ե� ����(���׷��̵� �ʿ� ����)
            {
                m_SkNodeList[i].SetState(GlobalValue.m_SkDataList[i].m_UpPrice,
                    GlobalValue.m_SkDataList[i].m_Level);
            }
        }
    }

    public void BuySkillItem(SkillType a_SkType)
    {
        if(a_SkType < SkillType.Skill_0 || SkillType.SkCount <= a_SkType)
        {
            return;
        }

        string a_Mess = "";
        bool a_NeedDelegate = false;
        Skill_Info a_SkInfo = GlobalValue.m_SkDataList[(int)a_SkType];
        int a_Cost = 0;
        //ù���Ž�
        if(a_SkInfo.m_Level <= 0)
        {
            a_Cost = a_SkInfo.m_Price;
            if (GlobalValue.g_UserGold < a_Cost)
            {
                a_Mess = "���� ��尡 �����մϴ�.";
            }
            else
            {
                a_Mess = "���� �����Ͻðڽ��ϱ�?";
                a_NeedDelegate = true;
            }
        }
        else //���׷��̵� ��
        {
            a_Cost = a_SkInfo.m_UpPrice + (a_SkInfo.m_UpPrice * (a_SkInfo.m_Level - 1));
            if(a_SkInfo.m_Level >= 5)
            {
                a_Mess = "�̹� �ְ� �����Դϴ�.";
            }
            else if (GlobalValue.g_UserGold < a_Cost)
            {
                a_Mess = "���� ��尡 �����մϴ�.";
            }
            else
            {
                a_Mess = "���� ���׷��̵��Ͻðڽ��ϱ�?";
                a_NeedDelegate = true;
            }
        }

        m_BuySkType = a_SkType;
        m_SvMyGold = GlobalValue.g_UserGold;
        m_SvMyGold -= a_Cost;
        m_SvMyLevel = GlobalValue.m_SkDataList[(int)a_SkType].m_Level;
        m_SvMyLevel++;

        GameObject a_DlgRsc = Resources.Load("DlgBox") as GameObject;
        GameObject a_DlgBoxObj = (GameObject)Instantiate(a_DlgRsc);
        GameObject a_Canvas = GameObject.Find("Canvas");
        a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);
        DlgBox_Ctrl a_DlgBox = a_DlgBoxObj.GetComponent<DlgBox_Ctrl>();
        if(a_DlgBox != null)
        {
            if (a_NeedDelegate == true)
            {
                a_DlgBox.SetMessage(a_Mess, TryBuySkItem);
            }
            else
            {
                a_DlgBox.SetMessage(a_Mess);
            }
        }
    }

    public void TryBuySkItem() // ���� Ȯ�� �Լ�
    {
        if(m_BuySkType < SkillType.Skill_0 || SkillType.SkCount <= m_BuySkType)
        {
            return;
        }

        GlobalValue.g_UserGold = m_SvMyGold;
        GlobalValue.m_SkDataList[(int)m_BuySkType].m_Level = m_SvMyLevel;

        RefreshSkItemList();
        m_UserInfoTxt.text = "����(" + GlobalValue.g_NickName + ") : �������(" + GlobalValue.g_UserGold + ")";

        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        string a_KeyBuff = string.Format("Skill_Item_{0}", (int)m_BuySkType);
        PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkDataList[(int)m_BuySkType].m_Level);
    }
}

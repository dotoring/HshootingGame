using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkProductNode : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType = SkillType.SkCount;
    public Text m_LvText;
    public Image m_SkIconImg;
    public Text m_HelpText;
    public Text m_BuyText;
    // Start is called before the first frame update
    void Start()
    {
        //리스트뷰에 있는 스킬 가격 버튼을 눌러 구입을 시도 한 경우
        Button m_BtnCom = this.GetComponentInChildren<Button>();
        if(m_BtnCom != null)
        {
            m_BtnCom.onClick.AddListener(() =>
            {
                Store_Mgr a_StoreMgr = null;
                GameObject a_StoreObj = GameObject.Find("StoreMgr");
                if (a_StoreObj != null)
                {
                    a_StoreMgr = a_StoreObj.GetComponent<Store_Mgr>();
                }
                if (a_StoreMgr != null)
                {
                    a_StoreMgr.BuySkillItem(m_SkType);
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitData(SkillType a_SkType)
    {
        if(a_SkType < SkillType.Skill_0 || SkillType.SkCount <= a_SkType)
        {
            return;
        }

        m_SkType = a_SkType;
        m_SkIconImg.sprite = GlobalValue.m_SkDataList[(int)a_SkType].m_IconImg;
        m_SkIconImg.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(GlobalValue.m_SkDataList[(int)a_SkType].m_IconSize.x * 135.0f, 135.0f);

        m_HelpText.text = GlobalValue.m_SkDataList[(int)a_SkType].m_SkillExp;
    }

    public void SetState(int a_Price, int a_Lv = 0)
    {
        m_LvText.text = a_Lv.ToString() + "/5";

        if(a_Lv <= 0)
        {
            m_BuyText.text = a_Price.ToString() + " 골드";
        }
        else
        {
            int a_CalcPrice = a_Price + (a_Price * (a_Lv - 1));
            m_BuyText.text = "Up " + a_CalcPrice.ToString() + " 골드";
        }
    }
}

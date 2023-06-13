using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkInvenNode : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;
    [HideInInspector] public int m_CurSkCount = 0;
    public Image m_RootBtnImg;      //Root를 버튼으로 만들었으므로 버튼 백이미지 접근용 변수
    public Text m_ShortcutText;     //단축키 텍스트
    public Text m_LvText;
    public Text m_SkCountText;
    public Image m_SkIconImg;

    // Start is called before the first frame update
    void Start()
    {
        Button a_BtnCom = this.GetComponent<Button>();

        if(a_BtnCom != null)
        {
            a_BtnCom.onClick.AddListener(() =>
            {
                if (GlobalValue.m_SkDataList[(int)m_SkType].m_CurSkillCount <= 0)
                {
                    return;
                }

                Hero_Ctrl a_Hero = GameObject.FindObjectOfType<Hero_Ctrl>();
                if(a_Hero != null)
                {
                    a_Hero.UseSkill(m_SkType);
                }
                RefreshUI(m_SkType);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitState(Skill_Info a_SkInfo)
    {
        m_SkType = a_SkInfo.m_SkType;
        m_ShortcutText.text = ((int)m_SkType + 1).ToString();
        m_SkIconImg.sprite = a_SkInfo.m_IconImg;
        m_SkIconImg.GetComponent<RectTransform>().sizeDelta =
            new Vector2(a_SkInfo.m_IconSize.x * 103.0f, 103.0f); //스프라이트 사이즈를 이미지 비율에 맞게 조정
        m_CurSkCount = a_SkInfo.m_Level;
        m_LvText.text = "Lv" + a_SkInfo.m_Level.ToString();
        m_SkCountText.text = m_CurSkCount.ToString();
    }

    public void RefreshUI(SkillType a_SkType)
    {
        if (m_SkType != a_SkType)
        {
            return;
        }

        m_CurSkCount = GlobalValue.m_SkDataList[(int)m_SkType].m_CurSkillCount;
        if(m_SkCountText != null)
        {
            m_SkCountText.text = m_CurSkCount.ToString();
        }
        if(m_CurSkCount <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}

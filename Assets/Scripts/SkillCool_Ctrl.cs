using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCool_Ctrl : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;
    public Sprite[] m_IconImg = null;
    float SkillTime = 0.0f;
    float SkillDuration = 0.0f;
    public Image TimeImg = null;
    public Image IconImg = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SkillTime -= Time.deltaTime;
        TimeImg.fillAmount = SkillTime / SkillDuration;

        if(SkillTime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    public void InitState(SkillType a_SkType, float a_Time, float a_During)
    {
        m_SkType = a_SkType;
        IconImg.sprite = m_IconImg[(int)m_SkType];

        SkillTime = a_Time;
        SkillDuration = a_During;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade_Mgr : MonoBehaviour
{
    public bool IsFadeOut = false;
    public bool IsFadeIn = false;

    Image m_FadeImg = null;
    float AniDuring = 0.8f;
    bool m_StartFade = false;
    float m_CalcTime = 0.0f;
    float m_AddTimer = 0.0f;
    Color m_Color;

    float m_StVal = 1.0f;
    float m_EndVal = 0.0f;

    string m_SceneName = "";

    public static Fade_Mgr Inst = null;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject a_Canvas = GameObject.Find("Canvas");
        if(a_Canvas != null)
        {
            Image[] a_ImgList = a_Canvas.transform.GetComponentsInChildren<Image>(true);
            for(int i = 0; i < a_ImgList.Length; i++)
            {
                if(a_ImgList[i].gameObject.name == "FadePanel")
                {
                    m_FadeImg = a_ImgList[i];
                    break;
                }
            }
        }

        if(m_FadeImg != null && IsFadeIn == true)
        {
            m_StVal = 1.0f;
            m_EndVal = 0.0f;
            m_FadeImg.color = new Color32(0, 0, 0, 255);
            m_FadeImg.gameObject.SetActive(true);
            m_StartFade = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_FadeImg == null)
        {
            return;
        }
        FadeUpdate();
    }

    void FadeUpdate()
    {
        if(m_StartFade == false)
        {
            return;
        }
        if(m_CalcTime < 1.0f)
        {
            m_AddTimer += Time.deltaTime;
            m_CalcTime = m_AddTimer / AniDuring;
            m_Color = m_FadeImg.color;
            m_Color.a = Mathf.Lerp(m_StVal, m_EndVal, m_CalcTime);
            m_FadeImg.color = m_Color;

            if(1.0f <= m_CalcTime)
            {
                if (m_StVal == 1.0f && m_EndVal == 0.0f)
                {
                    m_Color.a = 0.0f;
                    m_FadeImg.color = m_Color;
                    m_FadeImg.gameObject.SetActive(false);
                    m_StartFade = false;
                }
                else if(m_StVal == 0.0f && m_EndVal == 1.0f)
                {
                    SceneManager.LoadScene(m_SceneName);
                }
            }
        }
    }

    public void SceneOut(string a_ScName)
    {
        if(m_FadeImg == null)
        {
            return;
        }

        m_SceneName = a_ScName;

        m_CalcTime = 0.0f;
        m_AddTimer = 0.0f;
        m_StVal = 0.0f;
        m_EndVal = 1.0f;
        m_FadeImg.color = new Color32(0, 0, 0, 0);
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    public delegate void CFG_Response();
    public CFG_Response DltMethod = null;

    public Button m_OK_Btn = null;
    public Button m_Close_Btn = null;

    public InputField m_NickInputField = null;

    public Toggle m_Sound_Toggle = null;
    public Slider m_Sound_Slider = null;

    // Start is called before the first frame update
    void Start()
    {
        if(m_OK_Btn != null)
        {
            m_OK_Btn.onClick.AddListener(OkBtnClick);
        }

        if (m_Close_Btn != null)
        {
            m_Close_Btn.onClick.AddListener(CloseBtnClick);
        }

        if(m_Sound_Toggle != null)
        {
            m_Sound_Toggle.onValueChanged.AddListener(SoundOnOff);
        }

        if(m_Sound_Slider != null)
        {
            m_Sound_Slider.onValueChanged.AddListener(SliderChanged);
        }

        if(m_NickInputField != null)
        {
            m_NickInputField.text = PlayerPrefs.GetString("NickName", "AAA");
        }

        //체크 상태, 슬라이드 상태 로딩 후 UI 컨트롤에 적용
        int a_SOundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if(m_Sound_Toggle != null)
        {
            if(a_SOundOnOff == 1)
            {
                m_Sound_Toggle.isOn = true;
            }
            else
            {
                m_Sound_Toggle.isOn = false;
            }
        }

        if(m_Sound_Slider != null)
        {
            m_Sound_Slider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        }
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    void OkBtnClick()
    {
        string a_NickStr = m_NickInputField.text;
        a_NickStr = a_NickStr.Trim();
        if(string.IsNullOrEmpty(a_NickStr) == true)
        {
            Debug.Log("별명이 입력되지 않았습니다.");
            return;
        }

        if(!(2 <= a_NickStr.Length && a_NickStr.Length < 16))
        {
            Debug.Log("별병은 2글자 이상 15글자 이하로 설정해주세요");
            return;
        }

        GlobalValue.g_NickName = a_NickStr;
        PlayerPrefs.SetString("NickName", a_NickStr);

        if(DltMethod != null)
        {
            DltMethod();
        }

        Time.timeScale = 1.0f;
        Destroy(gameObject);
    }

    void CloseBtnClick()
    {

        Time.timeScale = 1.0f;
        Destroy(gameObject);
    }

    void SoundOnOff(bool value)
    {
        if(m_Sound_Toggle != null)
        {
            if(value == true)
            {
                PlayerPrefs.SetInt("SoundOnOff", 1);
            }
            else
            {
                PlayerPrefs.SetInt("SoundOnOff", 0);
            }

            Sound_Mgr.Instance.SoundOnOff(value);
        }
    }

    void SliderChanged(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        Sound_Mgr.Instance.SoundVolume(value);
    }
}

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
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    void OkBtnClick()
    {


        Time.timeScale = 1.0f;
        Destroy(gameObject);
    }

    void CloseBtnClick()
    {

    }

    void SoundOnOff(bool value)
    {

    }

    void SliderChanged(float value)
    {

    }
}

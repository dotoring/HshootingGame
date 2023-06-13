using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DlgBox_Ctrl : MonoBehaviour
{
    public delegate void DLT_Response();    //델리게이트 데이터형 하나 선언
    DLT_Response DltMethod;     // 델리게이트 변수 선언(소켓 역할)

    public Button m_OkBtn = null;
    public Button m_CloseBtn = null;
    public Button m_CancelBtn = null;
    public Text m_ContentsText = null;

    // Start is called before the first frame update
    void Start()
    {
        if(m_OkBtn != null)
        {
            m_OkBtn.onClick.AddListener(() =>
            {
                if (DltMethod != null)
                {
                    DltMethod();
                }

                Destroy(gameObject);
            });
        }

        if(m_CloseBtn != null)
        {
            m_CloseBtn.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            });
        }

        if (m_CancelBtn != null)
        {
            m_CancelBtn.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMessage(string a_Mess, DLT_Response a_DltMtd = null)
    {
        m_ContentsText.text = a_Mess;
        DltMethod = a_DltMtd;
    }
}

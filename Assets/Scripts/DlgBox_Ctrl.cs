using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DlgBox_Ctrl : MonoBehaviour
{
    public delegate void DLT_Response();    //��������Ʈ �������� �ϳ� ����
    DLT_Response DltMethod;     // ��������Ʈ ���� ����(���� ����)

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

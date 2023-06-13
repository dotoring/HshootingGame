using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    public Button m_StartBtn;
    public Button m_ExitGameBtn;
    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();

        if(m_StartBtn != null)
        {
            m_StartBtn.onClick.AddListener(() =>
            {
                if(Fade_Mgr.Inst != null && Fade_Mgr.Inst.IsFadeOut == true)
                {
                    Fade_Mgr.Inst.SceneOut("LobbyScene");
                }
                else
                {
                    SceneManager.LoadScene("LobbyScene");
                }

                Sound_Mgr.Instance.PlayGUISound("Pop", 1.0f);
            });
        }

        if(m_ExitGameBtn != null)
        {
            m_ExitGameBtn.onClick.AddListener(() =>
            {
                Sound_Mgr.Instance.PlayGUISound("Pop", 1.0f);
                Application.Quit();
            });
        }

        //Sound_Mgr.Instance.PlayBGM("sound_bgm_title_001", 1.0f);
        Sound_Mgr.Instance.m_AudioSrc.clip = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

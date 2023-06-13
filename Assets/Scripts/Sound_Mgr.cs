using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioNode : MonoBehaviour
{
    [HideInInspector] public AudioSource m_AudioSrc = null; //각 레이어별 AudioSource 컴포넌트를 저장하기 위한 변수
    [HideInInspector] public float m_EffVolume = 0.2f;      //각 레이어별 사운드 볼륨
    [HideInInspector] public float m_PlayTime = 0.0f;       //각 레이어별 타이머

    void Update()
    {
        if (m_PlayTime > 0.0f)
        {
            m_PlayTime -= Time.deltaTime;
        }
    }
}

public class Sound_Mgr : G_Singleton<Sound_Mgr>
{
    [HideInInspector] public AudioSource m_AudioSrc = null;
    Dictionary<string, AudioClip> m_ADClipList = new Dictionary<string, AudioClip>();

    //효과음 최적화를 위한 버퍼 변수
    int m_EffSdCount = 20;  //지금은 20개의 레이어로 플레이
    List<AudioNode> m_AdNodeList = new List<AudioNode>();

    float m_BgmVolume = 0.2f;
    [HideInInspector] public bool m_SoundOnOff = true;
    [HideInInspector] public float m_SoundVolume = 1.0f;

    protected override void Init() //Awake() 함수 대신 사용
    {
        base.Init(); //부모쪽에 있는 Init() 함수 호출

        LoadChildGameObj();
    }

    // Start is called before the first frame update
    void Start()
    {
        //사운드를 미리 로딩
        AudioClip a_GAudioClip = null;
        object[] temp = Resources.LoadAll("Sounds");
        for(int i = 0; i < temp.Length; i++)
        {
            a_GAudioClip = temp[i] as AudioClip;

            if(m_ADClipList.ContainsKey(a_GAudioClip.name) == true)
            {
                continue;
            }

            m_ADClipList.Add(a_GAudioClip.name, a_GAudioClip);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadChildGameObj()
    {
        m_AudioSrc = this.gameObject.AddComponent<AudioSource>();

        for(int i = 0; i < m_EffSdCount; i++)
        {
            GameObject newSoundObj = new GameObject("SoundEffObj");
            newSoundObj.transform.SetParent(this.transform);
            newSoundObj.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundObj.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            a_AudioSrc.loop = false;
            AudioNode a_AudioNode = newSoundObj.AddComponent<AudioNode>();
            a_AudioNode.m_AudioSrc = a_AudioSrc;
            m_AdNodeList.Add(a_AudioNode);
        }

        //사운드 OnOff, 사운드 볼륨 로컬 로딩 후 적용
        int a_SoundOnfOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if(a_SoundOnfOff == 1)
        {
            SoundOnOff(true);
        }
        else
        {
            SoundOnOff(false);
        }

        float a_Value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        SoundVolume(a_Value);
    }

    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudioClip = null;
        if (m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if(m_AudioSrc == null)
        {
            return;
        }

        if(m_AudioSrc.clip != null && m_AudioSrc.clip.name == a_FileName)
        {
            return;
        }

        m_AudioSrc.clip = a_GAudioClip;
        m_AudioSrc.volume = fVolume * m_SoundVolume;
        m_BgmVolume = fVolume;
        m_AudioSrc.loop = true;
        m_AudioSrc.Play();
    }

    public void PlayGUISound(string a_FileName, float fvolume = 0.2f)
    {
        if(m_SoundOnOff == false)
        {
            return;
        }

        AudioClip a_GAudioClip = null;
        if(m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if(m_AudioSrc == null)
        {
            return;
        }

        m_AudioSrc.PlayOneShot(a_GAudioClip, fvolume * m_SoundVolume);
    }

    public void PlayEffSound(string a_FileName, float fVolume = 0.2f)
    {
        if(m_SoundOnOff == false)
        {
            return;
        }

        AudioClip a_GAudioClip = null;
        if(m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if(a_GAudioClip == null)
        {
            return;
        }

        bool isPlayOk = false;
        AudioSource a_AudSrc = null;
        foreach(AudioNode a_AudNode in m_AdNodeList)
        {
            if(a_AudNode == null)
            {
                continue;
            }

            //이전 사운드가 아직 플레이 중이면 스킵
            if(0.0f < a_AudNode.m_PlayTime)
            {
                continue;
            }

            a_AudSrc = a_AudNode.m_AudioSrc;
            a_AudSrc.volume = fVolume * m_SoundVolume; //m_SoundVolume변수로 게임의 전체적인 볼륨 설정
            a_AudSrc.clip = a_GAudioClip;
            a_AudNode.m_EffVolume = fVolume;
            a_AudNode.m_PlayTime = a_GAudioClip.length + 0.7f;
            a_AudSrc.Play();

            isPlayOk = true;
            break;
        }

        if(isPlayOk == false) //사운드 추가 필요 상태
        {
            GameObject newSoundObj = new GameObject("SoundEffObj");
            newSoundObj.transform.SetParent(this.transform);
            newSoundObj.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundObj.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            a_AudioSrc.loop = false;
            AudioNode a_AudioNode = newSoundObj.AddComponent<AudioNode>();
            a_AudioNode.m_AudioSrc = a_AudioSrc;
            m_AdNodeList.Add(a_AudioNode);

            a_AudioSrc.volume = fVolume * m_SoundVolume; //m_SoundVolume변수로 게임의 전체적인 볼륨 설정
            a_AudioSrc.clip = a_GAudioClip;
            a_AudioNode.m_EffVolume = fVolume;
            a_AudioNode.m_PlayTime = a_GAudioClip.length + 0.7f;
            a_AudioSrc.Play();
        }
    }

    public void SoundOnOff(bool a_OnOff = true)
    {
        bool a_MuteOnOff = !a_OnOff;

        if(m_AudioSrc != null)
        {
            m_AudioSrc.mute = a_MuteOnOff;
            if(a_MuteOnOff == false) //사운드를 다시 켰을 때
            {
                m_AudioSrc.time = 0; //처음부터 다시 플레이
            }
        }

        foreach(AudioNode a_AudNode in m_AdNodeList)
        {
            if(a_AudNode == null)
            {
                continue;
            }

            a_AudNode.m_AudioSrc.mute = a_MuteOnOff;
            if(a_MuteOnOff == false)
            {
                a_AudNode.m_AudioSrc.time = 0;
            }
        }

        m_SoundOnOff = a_OnOff;
    }

    public void SoundVolume(float fVolume)
    {
        if(m_AudioSrc != null)
        {
            m_AudioSrc.volume = m_BgmVolume * fVolume;
        }

        foreach(AudioNode a_AudNode in m_AdNodeList)
        {
            if(a_AudNode == null)
            {
                continue;
            }

            a_AudNode.m_AudioSrc.volume = a_AudNode.m_EffVolume * fVolume;
        }

        m_SoundVolume = fVolume;
    }
}

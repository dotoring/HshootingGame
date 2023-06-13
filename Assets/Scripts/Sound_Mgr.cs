using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioNode : MonoBehaviour
{
    [HideInInspector] public AudioSource m_AudioSrc = null; //�� ���̾ AudioSource ������Ʈ�� �����ϱ� ���� ����
    [HideInInspector] public float m_EffVolume = 0.2f;      //�� ���̾ ���� ����
    [HideInInspector] public float m_PlayTime = 0.0f;       //�� ���̾ Ÿ�̸�

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

    //ȿ���� ����ȭ�� ���� ���� ����
    int m_EffSdCount = 20;  //������ 20���� ���̾�� �÷���
    List<AudioNode> m_AdNodeList = new List<AudioNode>();

    float m_BgmVolume = 0.2f;
    [HideInInspector] public bool m_SoundOnOff = true;
    [HideInInspector] public float m_SoundVolume = 1.0f;

    protected override void Init() //Awake() �Լ� ��� ���
    {
        base.Init(); //�θ��ʿ� �ִ� Init() �Լ� ȣ��

        LoadChildGameObj();
    }

    // Start is called before the first frame update
    void Start()
    {
        //���带 �̸� �ε�
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

        //���� OnOff, ���� ���� ���� �ε� �� ����
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

            //���� ���尡 ���� �÷��� ���̸� ��ŵ
            if(0.0f < a_AudNode.m_PlayTime)
            {
                continue;
            }

            a_AudSrc = a_AudNode.m_AudioSrc;
            a_AudSrc.volume = fVolume * m_SoundVolume; //m_SoundVolume������ ������ ��ü���� ���� ����
            a_AudSrc.clip = a_GAudioClip;
            a_AudNode.m_EffVolume = fVolume;
            a_AudNode.m_PlayTime = a_GAudioClip.length + 0.7f;
            a_AudSrc.Play();

            isPlayOk = true;
            break;
        }

        if(isPlayOk == false) //���� �߰� �ʿ� ����
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

            a_AudioSrc.volume = fVolume * m_SoundVolume; //m_SoundVolume������ ������ ��ü���� ���� ����
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
            if(a_MuteOnOff == false) //���带 �ٽ� ���� ��
            {
                m_AudioSrc.time = 0; //ó������ �ٽ� �÷���
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

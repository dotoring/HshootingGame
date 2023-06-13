using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DmgText_Ctrl : MonoBehaviour
{
    float m_EffTime = 0.0f;     //���� �ð� ���� ����
    public Text m_ChildText = null; //Text UI ���ٿ� ����

    float MvVelocity = 1.1f / 1.05f;    //1.05�� ���ȿ� 1.1m ���ٸ�... �ӵ�
    float ApVelocity = 1.0f / (1.0f - 0.4f);
    //alpha 0.4(0.0)�ʺ��� ���� 1.0��(1.0f)����

    Vector3 m_CurPos;       //��ġ ���� ����
    Color m_Color;          //�÷� ���� ����

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_EffTime += Time.deltaTime;

        if(m_EffTime < 1.05f)
        {
            m_CurPos = m_ChildText.transform.position;
            m_CurPos.y += Time.deltaTime * MvVelocity;
            m_ChildText.transform.position = m_CurPos;
        }

        if(0.4f < m_EffTime)
        {
            m_Color = m_ChildText.color;
            m_Color.a -= (Time.deltaTime * ApVelocity);
            if (m_Color.a < 0.0f)
                m_Color.a = 0.0f;
            m_ChildText.color = m_Color;
        }

        if (1.05f < m_EffTime)
            Destroy(this.gameObject);

    } //void Update()

    public void InitDamage(float a_Damage, Color a_Color)
    {
        if (m_ChildText == null)
            m_ChildText = this.GetComponentInChildren<Text>();

        if(a_Damage <= 0.0f)
        {
            int a_Dmg = (int)Mathf.Abs(a_Damage);   //���밪 �Լ�
            m_ChildText.text = "- " + a_Dmg;
        }
        else
        {
            m_ChildText.text = "+ " + (int)a_Damage;
        }

        a_Color.a = 1.0f;
        m_ChildText.color = a_Color;

    }//public void InitDamage(float a_Damage, Color a_Color)
}

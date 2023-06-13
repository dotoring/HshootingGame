using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart_Ctrl : MonoBehaviour
{
    Vector3 m_DirVecX = Vector3.right;
    Vector3 m_DirVecY = Vector3.up;
    Vector3 m_DirVec;
    float m_MoveSpeed = 7.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_DirVec = m_DirVecX + m_DirVecY;
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.x < CameraResolution.m_ScreenWMin.x + 0.5f ||
            CameraResolution.m_ScreenWMax.x -0.5f < this.transform.position.x)
        {
            m_DirVecX = -m_DirVecX;
        }

        if (this.transform.position.y < CameraResolution.m_ScreenWMin.y + 0.5f ||
            CameraResolution.m_ScreenWMax.y - 0.5f < this.transform.position.y)
        {
            m_DirVecY = -m_DirVecY;
        }

        m_DirVec = m_DirVecX + m_DirVecY;

        transform.position += m_DirVec * Time.deltaTime * m_MoveSpeed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;


    [SerializeField]
    private float m_CamInterpolation = 0.25f;

    [SerializeField]
    private Vector3 m_CameraPosOffset = new Vector3(0.0f, 5.0f, -1.0f);

    // Start is called before the first frame update
    void Start()
    {
        transform.position = m_Player.transform.position + m_CameraPosOffset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos = m_Player.transform.position + m_CameraPosOffset;

        transform.position += (targetPos - transform.position) * m_CamInterpolation;
    }
}

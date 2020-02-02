using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField]
    private Image m_MeterBase;
    [SerializeField]
    private Image m_Meter;
    [SerializeField]
    private Image m_Ouch;
    [SerializeField]
    private Camera m_Camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(0, -90, 0);
        //transform.rotation = Quaternion.AngleAxis(m_Camera.transform.eulerAngles.x, Vector3.right) * Quaternion.AngleAxis(-90.0f, Vector3.up);
    }

    public void ShowMeter()
    {
        m_MeterBase.gameObject.SetActive(true);
        m_Meter.gameObject.SetActive(true);
        m_Ouch.gameObject.SetActive(false);
    }

    public void SetMeterPercentage(float Percent)
    {
        m_Meter.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Percent);
    }

    public void ShowOuch()
    {
        m_MeterBase.gameObject.SetActive(false);
        m_Meter.gameObject.SetActive(false);
        m_Ouch.gameObject.SetActive(true);
    }

    public void HideAll()
    {
        m_MeterBase.gameObject.SetActive(false);
        m_Meter.gameObject.SetActive(false);
        m_Ouch.gameObject.SetActive(false);
    }
}

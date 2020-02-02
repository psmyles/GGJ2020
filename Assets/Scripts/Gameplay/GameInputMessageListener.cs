using UnityEngine;
using System.Collections;

public class GameInputMessageListener : MessageListener 
{
	protected override void Awake()
	{
		base.Awake();
	}
	// Use this for initialization
	protected override void Start () 
	{
	
	}
	
	protected override void Update()
	{
	}
	
	void MoveForward(float fwdValue)
	{
		if( m_TargetBufferedInput != null )
		{
			m_TargetBufferedInput.AddInput("MoveForward",0.15f,fwdValue);
			//Debug.Log("Received mssg 1");
		}
	}

    void MoveBackward(float backValue)
    {
        if (m_TargetBufferedInput != null)
        {
            m_TargetBufferedInput.AddInput("MoveBackward", 0.15f, backValue);
            //Debug.Log("Received mssg 1");
        }
    }

    void MoveRight(float rightValue)
	{
		if( m_TargetBufferedInput != null )
		{
			m_TargetBufferedInput.AddInput("MoveRight",0.15f,rightValue);
			//Debug.Log("Received mssg 2");
		}
	}

    void MoveLeft(float leftValue)
    {
        if (m_TargetBufferedInput != null)
        {
            m_TargetBufferedInput.AddInput("MoveLeft", 0.15f, leftValue);
            //Debug.Log("Received mssg 2");
        }
    }

    void DashBackward(float backVal)
    {
        if (m_TargetBufferedInput != null)
        {
            m_TargetBufferedInput.AddInput("DashBackward", 0.15f, backVal);
            //Debug.Log("Received mssg 2");
        }
    }

    void DashForward(float fwdVal)
    {
        if (m_TargetBufferedInput != null)
        {
            m_TargetBufferedInput.AddInput("DashForward", 0.15f, fwdVal);
            //Debug.Log("Received mssg 2");
        }
    }

    void DashLeft(float leftVal)
    {
        if (m_TargetBufferedInput != null)
        {
            m_TargetBufferedInput.AddInput("DashLeft", 0.15f, leftVal);
            //Debug.Log("Received mssg 2");
        }
    }

    void DashRight(float rghtVal)
    {
        if (m_TargetBufferedInput != null)
        {
            m_TargetBufferedInput.AddInput("DashRight", 0.15f, rghtVal);
            //Debug.Log("Received mssg 2");
        }
    }

    void Dash()
    {
        if (m_TargetBufferedInput != null)
        {
            m_TargetBufferedInput.AddInput("Dash", 0.15f);
            //Debug.Log("Received mssg 2");
        }
    }
}

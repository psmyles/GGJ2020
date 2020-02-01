using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MessageGenerator : MonoBehaviour 
{
	public List<MessageListener>				m_Listeners;
	
	// Use this for initialization
	protected abstract void Start ();
	
	// Update is called once per frame
	protected abstract void Update ();
	
	public abstract MessageGenerator Get();
	
	public void SendMessageToListeners(string message)
	{
		foreach( MessageListener element in m_Listeners)
	    {
			if( element != null )
			{
				element.SendMessage(message);
			}
			else
			{
				//need to check if objects are disabled what happens
				Debug.DebugBreak();
			}
	    }
	}
	
	public void SendMessageToListeners(string message, float fltValue)
	{
		foreach( MessageListener element in m_Listeners)
	    {
			if( element != null )
			{
				element.SendMessage(message,fltValue);
			}
			else
			{
				//need to check if objects are disabled what happens
				Debug.DebugBreak();
			}
	    }
	}
	
	public void SendMessageToListeners(string message, Object obj)
	{
		foreach( MessageListener element in m_Listeners)
	    {
			if( element != null )
			{
				element.SendMessage(message,obj);
			}
			else
			{
				//need to check if objects are disabled what happens
				Debug.DebugBreak();
			}
	    }
	}
}

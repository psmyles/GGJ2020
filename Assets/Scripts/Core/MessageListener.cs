using UnityEngine;
using System.Collections;

public abstract class MessageListener : MonoBehaviour 
{
	public BufferedInput		m_TargetBufferedInput;
	
	protected virtual void Awake()
	{
		if( m_TargetBufferedInput == null )
		{
			Debug.LogError("Message listener '" + gameObject.name + "' doesn't have a 'BufferedInput' attached");
			Debug.Break();
		}
	}
	
	// Use this for initialization
	protected abstract void Start();
	
	// Update is called once per frame
	protected abstract void Update();
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BufferedInput : MonoBehaviour 
{
	// -------------------------------------------------------------------------
	// Start Input
	public class Input
	{
		private float		m_InputTime = 0;
		private float  		m_InputValue = 0;	//used if input has value
		
		public Input(float inputTime, float inputValue)
		{
			m_InputTime = inputTime;
			m_InputValue = inputValue;
		}
		
		public float InputTime
		{
			get
			{
				return m_InputTime;
			}
			set
			{
				m_InputTime = value;
			}
		}
		
		public float InputValue
		{
			get
		    {
		        return m_InputValue;
		    }
			set
			{
				m_InputValue = value;
			}	
		}
	}
	// End Input
	// -------------------------------------------------------------------------
	
	private Dictionary<string,Input>	m_CurrentInputs = new Dictionary<string, Input>();
	private ArrayList					m_ToBeRemoved = new ArrayList();
	
	
	// Use this for initialization
	protected void Start ()
	{
	}
	
	// Update is called once per frame
	protected void Update ()
	{
		foreach(KeyValuePair<string,Input> entry in m_CurrentInputs)
		{
			if( entry.Value.InputTime <= 0.0f )
			{
				m_ToBeRemoved.Add(entry.Key);
			}
			entry.Value.InputTime -= Time.deltaTime;
			
			//Debug.Log("Time " + entry.Value.InputTime + " deltaTime = " + Time.deltaTime);
		}
		
		//Debug.Log("To Be removed " + m_ToBeRemoved.Count);
		for( int i = 0 ; i < m_ToBeRemoved.Count ; i++ )
		{
			m_CurrentInputs.Remove(m_ToBeRemoved[i] as string);
		}
		
		m_ToBeRemoved.Clear();
	}
	
	public void AddInput(string inputName,float inputTime, float inputValue = 0.0f )
	{
		bool contain = m_CurrentInputs.ContainsKey(inputName);
		
		if( contain )
		{
			Input exist = m_CurrentInputs[inputName];
			exist.InputTime = inputTime;
			exist.InputValue = inputValue;
			//Debug.Log("INput exist " + inputName);
		}
		else
		{
			Input newInput = new Input(inputTime,inputValue);
			m_CurrentInputs.Add(inputName,newInput);
			//Debug.Log("Input added = " + inputName + " Time " + inputTime + " value = " + inputValue);
		}
	}
	
	public void RemoveInput(string inputName)
	{
		bool contain = m_CurrentInputs.ContainsKey(inputName);
		if( contain )
		{
			m_CurrentInputs.Remove(inputName);
		}
	}
	
	public void ClearAllInputs()
	{
		m_CurrentInputs.Clear();
	}
	
	public bool CheckInput(string inputName)
	{
		if( m_CurrentInputs.ContainsKey(inputName) )
		{
			return true;
		}
		return false;
	}
	
	public bool CheckInput(string inputName, out float outValue)
	{
		if( m_CurrentInputs.ContainsKey(inputName) )
		{
			Input exist = m_CurrentInputs[inputName];
			outValue = exist.InputValue;
			return true;
		}
		outValue = 0.0f;
		
		return false;
	}
}

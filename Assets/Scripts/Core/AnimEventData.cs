using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class TriggerData
{
	public System.Type		m_TriggerType;
	public string			m_TriggerTypeString;
	public bool				m_Enabled;
}

public class AnimEventData : ScriptableObject
{
	public float			m_TranslationSpeed;
	public bool				m_FinishAnimation = false;
	public TriggerData[]	m_TriggerData;	
	
	// Called when object is loaded
	void OnEnable()	
	{
		List<System.Type> triggerTypes = TypeUtilities.GetInheritedClassesTypes(typeof(ActorTriggerBase));
		
		if( m_TriggerData != null )
		{
			for( int i = 0 ; i < m_TriggerData.Length ; i++ )
			{
				SetTriggerType(m_TriggerData[i],triggerTypes);
			}
		}
	}
				
	private void SetTriggerType(TriggerData triggerData, List<System.Type> allTriggerType)
	{
		foreach (System.Type currType in allTriggerType) 
		{
		    if( currType.ToString() == triggerData.m_TriggerTypeString )
			{
				triggerData.m_TriggerType = currType;
				break;
			}
		}
	}
}

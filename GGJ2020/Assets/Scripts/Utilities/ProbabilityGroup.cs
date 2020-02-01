using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityGroup<T>
{
	private Dictionary<T,float>		m_OriginalData = new Dictionary<T,float>();
	private ArrayList				m_ProbableObjects = null;
	private bool					m_ProbDataGenerated = false;
	private T						m_DefaultT;
	
	public void AddObject(T obj, float probabilityOfOccurance)
	{
		m_OriginalData.Add(obj,probabilityOfOccurance);
		m_ProbDataGenerated = false;
		m_DefaultT = obj;
	}
	
	public void GenerateProbabilityData()
	{
		if( !m_ProbDataGenerated )
		{
			float totalValue = 0.0f;
			int count = 0;
			foreach( KeyValuePair<T,float> entry in m_OriginalData )
			{
			    totalValue += entry.Value;
				count++;
			}
			
			m_ProbableObjects = new ArrayList();
			
			float totalProb = 0.0f;
			int i = 0;
			foreach( KeyValuePair<T,float> entry in m_OriginalData )
			{
				totalProb += entry.Value/totalValue;
				if( i == count)
				{
					totalProb = 1.0f;
				}
			    m_ProbableObjects.Add(new KeyValuePair<T,float>(entry.Key,totalProb) );
				i++;
			}
			
			m_ProbDataGenerated = true;
		}
	}
	
	public T GetRandomObject()
	{
		float randomVal = Random.Range(0.0f,1.0f);
		
		for( int i = 0 ; i < m_ProbableObjects.Count ; i++ )
		{
			KeyValuePair<T,float> currVal = (KeyValuePair<T,float>)m_ProbableObjects[i];// as KeyValuePair<T,float>;
			if( randomVal <= currVal.Value )
			{
				return currVal.Key;
			}
		}
		
		return m_DefaultT;
	}
}

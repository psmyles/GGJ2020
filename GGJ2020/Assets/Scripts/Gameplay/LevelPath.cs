using UnityEngine;
using System.Collections;

public class LevelPath : MonoBehaviour 
{
	private static LevelPath		m_Instance = null;
	
	public GameObject[]				m_Path;
	
	// Use this for initialization
	void Start () 
	{
		if( m_Instance != null )
		{
			Debug.LogError("Only one level path for each level can be created...");
			Debug.Break();
		}
		else
		{
			m_Instance = this;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}

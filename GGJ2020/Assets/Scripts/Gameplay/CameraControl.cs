using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	// All public variables
	public Transform m_Player;
	public float m_PositionInterpolate;
	
	// All private variables
	private Vector3 m_OffsetPos2Player;
	// Use this for initialization
	void Start () 
	{
		if(!m_Player)
		{
			Debug.LogError("No player found or player is not tagged!");
		}
		
		m_OffsetPos2Player = transform.position - m_Player.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 targetPos = m_Player.position + m_OffsetPos2Player;
		transform.position = transform.position + (targetPos - transform.position) * m_PositionInterpolate;
	}
}

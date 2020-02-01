using UnityEngine;
using System;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	// All public variables
	public Transform m_Player;
	public float m_PositionInterpolate;
	
	// All private variables
	private Vector3 m_OffsetPos2Player;
	private Vector2 m_rangeVec;
	// Use this for initialization
	void Start () 
	{
		if(!m_Player)
		{
			Debug.LogError("No player found or player is not tagged!");
		}
		m_OffsetPos2Player = new Vector3(0.0f,10.0f,0.0f);
		m_rangeVec = new Vector2(1.0f, 1.0f);
		//m_OffsetPos2Player = transform.position - m_Player.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float dx =  m_Player.position.x - transform.position.x, dz = m_Player.position.z - transform.position.z;
		Vector3 deltaPosVec = new Vector3(0.0f, 0.0f, 0.0f);

		if( Math.Abs(dx) > m_rangeVec.x  || Math.Abs(dz) > m_rangeVec.y){
			deltaPosVec.x += (dx > 0)? dx-m_rangeVec.x : dx+m_rangeVec.x;
			deltaPosVec.z += (dz > 0)? dz-m_rangeVec.y : dz+m_rangeVec.y;
			transform.position = transform.position + deltaPosVec ;//+ (targetPos - transform.position) * m_PositionInterpolate;
		}
	}
}

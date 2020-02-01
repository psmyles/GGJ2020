using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour 
{
	private static CombatManager				m_Instance = null;
	private List<EnemyController>				m_ActiveEnemies = new List<EnemyController>();
	
	public Actor[]								m_CanAttackActor;
	//private int[]								m_AttackSlotsForActor;
	
	void Awake()
	{
		if( m_Instance != null )
		{
			Debug.LogError("Only one combat manager can be created");
			Debug.Break();
		}
		
		m_Instance = this;
		
		//m_AttackSlotsForActor = new int[m_CanAttackActor.Length];
	}	

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( m_ActiveEnemies.Count > 0 )
		{
			foreach (EnemyController currEnemy in m_ActiveEnemies)
			{
			    ChooseAttackActor(currEnemy);
			} 
		}
	}
	
	// Just temporarily now returns the first actor...
	private void ChooseAttackActor(EnemyController currEnemyController)
	{
		Enemy currEnemy = currEnemyController.Enemy;
		Vector3 enemyPos = currEnemy.gameObject.transform.position;
		Vector3 distVec = enemyPos - m_CanAttackActor[0].gameObject.transform.position;
		float sqrDist = distVec.sqrMagnitude;
		
		if( sqrDist < (currEnemy.Range * currEnemy.Range) )
		{
			//Vector3 dirToEnemy = distVec.normalized;
			Vector3 attackPos = Vector3.zero;//m_CanAttackActor[0].gameObject.transform.position;// + dirToEnemy * currEnemy.AttackRange;
			currEnemyController.AttackActor(m_CanAttackActor[0],attackPos);
		}
		else
		{
			currEnemyController.AttackActor(null,Vector3.zero);
		}	
	}
	
	public void RegisterEnemy(EnemyController enemy)
	{
		if( m_ActiveEnemies.Contains(enemy) == false )
		{
			m_ActiveEnemies.Add(enemy);
		}
	}
	
	public void UnRegisterEnemy(EnemyController enemy)
	{
		if( m_ActiveEnemies.Contains(enemy) == true )
		{
			m_ActiveEnemies.Remove(enemy);
		}
	}
	
	// Get the singleton
	public static CombatManager Get()
	{
		return m_Instance;
	}
}

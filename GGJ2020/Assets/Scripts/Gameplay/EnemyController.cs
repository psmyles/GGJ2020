using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyControllerProperties
{
	public enum PatrolType
	{
		ePT_Random,
		ePT_ZigZag,
		ePT_Clockwise,
		ePT_AntiClockwise,
	}
	
	public float				m_MinIdleTime = 3.0f;
	public float				m_MaxIdleTime = 9.0f;
	public GameObject[]			m_EnemyPatrolPath;
	public PatrolType			m_PatrolType;
	public float				m_MinPatrolMoveTime = 2.0f;
	public float				m_MaxPatrolMoveTime = 5.0f;
	public float				m_WalkProbabilityOnPatrol = 50;
	public float				m_RunProbabilityOnPatrol = 10;
	public float				m_MinAttackInterval = 2.0f;
	public float				m_MaxAttackInterval = 5.0f;
}

public class EnemyController : MonoBehaviour 
{
	public enum EnemyControllerStates
	{
		eECS_Patrol			= 0,
		eECS_Combat,
		
		eECS_Count
	}
	
	public enum EnemyControllerTimers
	{
		eECT_Idle			= 0,
		eECT_PatrolMove,
		eECT_AttackInterval,
		
		eECT_Count
	}
		
	public Enemy							m_Enemy = null;
	public EnemyControllerProperties		m_Properties;
	
	private StateMachine					m_ControllerStateMachine = new StateMachine();
	private State[]							m_AllStates;
	private int								m_TargetPathNodeIndex = -1;
	private bool							m_ZigZagPathIncreasing = true;
	
	private SimpleTimerArray				m_AllTimers = new SimpleTimerArray((uint)EnemyControllerTimers.eECT_Count);
	
	private ProbabilityGroup<bool>			m_WalkProbability = new ProbabilityGroup<bool>();
	
	private Vector3							m_ColliderOffset;
	
	private	Actor							m_AttackActor;
	private Vector3							m_AttackSlotPositionWRTActor;
	
	protected void Awake()
	{
		if( m_Enemy == null )
		{
			Debug.LogError("Empty EnemyController");
			Debug.Break();
		}
		
		m_AllStates = new State[(int)EnemyControllerStates.eECS_Count];
		m_WalkProbability.AddObject(true,m_Properties.m_WalkProbabilityOnPatrol);
		m_WalkProbability.AddObject(false,m_Properties.m_RunProbabilityOnPatrol);
		m_WalkProbability.GenerateProbabilityData();
		
		InitializeAllStates();
	}
	
	// Use this for initialization
	void Start () 
	{
		if( m_Properties.m_EnemyPatrolPath.Length > 0 )
		{
			m_Enemy.transform.position = m_Properties.m_EnemyPatrolPath[0].transform.position;
			m_TargetPathNodeIndex = 0;
			m_ZigZagPathIncreasing = true;
			
			if( m_Properties.m_EnemyPatrolPath.Length > 1 )
			{
				UpdateTargetPathNodeIndex();
			}
		}
		
		m_ColliderOffset = new Vector3(0.0f,transform.localScale.y * 0.5f,0.0f);
		SetColliderToEnemyPos();
		OnDeactivated();
		m_Enemy.gameObject.SetActiveRecursively(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( m_Enemy.gameObject.active == true )
		{
			m_ControllerStateMachine.Update(Time.deltaTime);
			m_AllTimers.Update(Time.deltaTime);
			SetColliderToEnemyPos();
		}
	}
	
	// -------------------------------------------------------------------------
	// Start State functions
	
	public void InitializeAllStates()
	{
		m_ControllerStateMachine.AddNewTransition(CheckStates);
		
		m_AllStates[(int)EnemyControllerStates.eECS_Patrol] = m_ControllerStateMachine.AddNewState("Patrol",BeginPatrolState,UpdatePatrolState,EndPatrolState);
		m_AllStates[(int)EnemyControllerStates.eECS_Combat] = m_ControllerStateMachine.AddNewState("Combat",BeginCombatState,UpdateCombatState,EndCombatState);
	}
	
	public State CheckStates(State currState)
	{
		if( currState == null )
		{
			return m_AllStates[(int)EnemyControllerStates.eECS_Patrol];
		}
		
		if( m_AttackActor == null )
		{
			if( currState.ID != m_AllStates[(int)EnemyControllerStates.eECS_Patrol].ID )
			{
				return m_AllStates[(int)EnemyControllerStates.eECS_Patrol];
			}
		}
		else
		{
			if( currState.ID != m_AllStates[(int)EnemyControllerStates.eECS_Combat].ID )
			{
				return m_AllStates[(int)EnemyControllerStates.eECS_Combat];
			}
		}	
		
		return null;
	}
	
	// -----------------
	// Start Patrol State
	public void BeginPatrolState(State prvState, State newState)
	{
		m_AllTimers.StartTimer((uint)EnemyControllerTimers.eECT_Idle,Random.Range(m_Properties.m_MinIdleTime,m_Properties.m_MaxIdleTime));
		m_Enemy.StayIdle();
	}
	
	public void UpdatePatrolState(State currState, float deltaTime)
	{
		if( m_Properties.m_EnemyPatrolPath.Length > 1 )
		{
			if( m_AllTimers.IsTimerRunning((uint)EnemyControllerTimers.eECT_Idle) )
			{
				if( m_AllTimers.IsTimeEnded((uint)EnemyControllerTimers.eECT_Idle) )
				{
					//Debug.Log("MoveState state");
					m_AllTimers.ResetTimer((uint)EnemyControllerTimers.eECT_Idle);
					m_AllTimers.StartTimer((uint)EnemyControllerTimers.eECT_PatrolMove,Random.Range(m_Properties.m_MinPatrolMoveTime,m_Properties.m_MaxPatrolMoveTime));
					
					m_Enemy.MoveToTarget(m_Properties.m_EnemyPatrolPath[m_TargetPathNodeIndex].transform.position,m_WalkProbability.GetRandomObject());
				}
			}
			else
			{
				if( m_AllTimers.IsTimerRunning((uint)EnemyControllerTimers.eECT_PatrolMove) )
				{
					if( m_AllTimers.IsTimeEnded((uint)EnemyControllerTimers.eECT_PatrolMove) )
					{
						m_AllTimers.ResetTimer((uint)EnemyControllerTimers.eECT_PatrolMove);
						m_AllTimers.StartTimer((uint)EnemyControllerTimers.eECT_Idle,Random.Range(m_Properties.m_MinIdleTime,m_Properties.m_MaxIdleTime));
						m_Enemy.StayIdle();
						//Debug.Log("Idle state");
					}
				}
			}
			
			if( m_AllTimers.IsTimerRunning((uint)EnemyControllerTimers.eECT_PatrolMove) )
			{
				if( m_Enemy.TargetReached() )
				{
					m_Enemy.transform.position = m_Properties.m_EnemyPatrolPath[m_TargetPathNodeIndex].transform.position;
					UpdateTargetPathNodeIndex();
					m_Enemy.MoveToTarget(m_Properties.m_EnemyPatrolPath[m_TargetPathNodeIndex].transform.position,m_WalkProbability.GetRandomObject());
					//Debug.Log("MoveState state 2");
				}
			}

		}
		
		for( int i = 0 ; i < m_Properties.m_EnemyPatrolPath.Length ; i++ )
		{
			DebugDraw.DrawCircle(m_Properties.m_EnemyPatrolPath[i].transform.position,0.1f,Vector3.up,9,Color.red);
		}
	}
	
	public void EndPatrolState(State endingState, State newState)
	{
	
	}
	
	// End Patrol State
	// -----------------
	
	
	// -----------------
	// Start Combat State
	public void BeginCombatState(State prvState, State newState)
	{
	}
	
	public void UpdateCombatState(State currState, float deltaTime)
	{
		if( m_AttackActor != null )
		{
			if( m_AllTimers.IsTimeEnded((uint)EnemyControllerTimers.eECT_AttackInterval) == true )
			{
				Vector3 targetPos = m_AttackActor.transform.position + m_AttackSlotPositionWRTActor;
				Vector3 distVec = m_Enemy.transform.position - targetPos;
				float sqrLength = distVec.sqrMagnitude;
				
				if( sqrLength <= (m_Enemy.AttackRange * m_Enemy.AttackRange) )
				{
					m_Enemy.Attack();
					m_AllTimers.StartTimer((uint)EnemyControllerTimers.eECT_AttackInterval,Random.Range(m_Properties.m_MinAttackInterval,m_Properties.m_MaxAttackInterval));
				}
				else
				{
					m_Enemy.MoveToTarget(targetPos,false);
				}
				//Debug.DrawLine(m_AttackActor.transform.position,m_AttackSlotPositionWRTActor);
			}
			else
			{
				m_Enemy.StayIdle();
			}
		}
	}
	
	public void EndCombatState(State endingState, State newState)
	{
	
	}
	
	// End Combat State
	// -----------------
	
	// End State functions
	// -------------------------------------------------------------------------
	
	private void UpdateTargetPathNodeIndex()
	{
		switch(m_Properties.m_PatrolType)
		{
		case EnemyControllerProperties.PatrolType.ePT_Random:
			int count = m_Properties.m_EnemyPatrolPath.Length;
			m_TargetPathNodeIndex = Random.Range(0,count);
			//Debug.Log("Curr path : " + m_TargetPathNodeIndex + ", game object : " + gameObject.name);
			break;
			
		case EnemyControllerProperties.PatrolType.ePT_ZigZag:
			if( m_ZigZagPathIncreasing == true )
			{
				m_TargetPathNodeIndex++;
				if( m_TargetPathNodeIndex == ( m_Properties.m_EnemyPatrolPath.Length - 1 ) )
				{
					m_ZigZagPathIncreasing = false;
				}
			}
			else
			{
				m_TargetPathNodeIndex--;
				if( m_TargetPathNodeIndex == 0 )
				{
					m_ZigZagPathIncreasing = true;
				}
			}	
			break;
			
		case EnemyControllerProperties.PatrolType.ePT_Clockwise:
			m_TargetPathNodeIndex++;
			if( m_TargetPathNodeIndex >= m_Properties.m_EnemyPatrolPath.Length )
			{
				m_TargetPathNodeIndex = 0;
			}
			break;
			
		case EnemyControllerProperties.PatrolType.ePT_AntiClockwise:
			m_TargetPathNodeIndex--;
			if( m_TargetPathNodeIndex < 0 )
			{
				m_TargetPathNodeIndex = m_Properties.m_EnemyPatrolPath.Length - 1;
			}
			break;
		}
	}
	
	private void SetColliderToEnemyPos()
	{
		transform.position = m_Enemy.transform.position + m_ColliderOffset;
		transform.rotation = m_Enemy.transform.rotation;
	}
	
	void OnTriggerEnter( Collider other ) 
	{
    	if( other.gameObject.tag == "Player" )
		{
			if( m_Enemy.gameObject.active == false )
			{
				m_Enemy.gameObject.SetActiveRecursively(true);
				OnActivated();
			}
		}
	}
	
	void OnTriggerExit( Collider other ) 
	{
    	if( other.gameObject.tag == "Player" )
		{
			if( m_Enemy.gameObject.active == true )
			{
				OnDeactivated();
				m_Enemy.gameObject.SetActiveRecursively(false);
			}
		}
	}
	
	private void OnActivated()
	{
		CombatManager.Get().RegisterEnemy(this);
		m_Enemy.OnActivated();
		m_AttackActor = null;
	}
	
	private void OnDeactivated()
	{
		CombatManager.Get().UnRegisterEnemy(this);
		m_Enemy.OnDeactivated();
		m_AttackActor = null;
	}
	
	public Enemy Enemy
	{
		get
		{
			return m_Enemy;
		}
	}
	
	public void AttackActor(Actor actorToAttack, Vector3 slotPosWRTActor )
	{
		m_AttackActor = actorToAttack;
		m_AttackSlotPositionWRTActor = slotPosWRTActor;
	}
}

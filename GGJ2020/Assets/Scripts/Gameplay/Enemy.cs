using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemyProperties
{
	public float								m_Health = 100.0f;
	public float								m_WalkTurnAngle = 90.0f;
	public float								m_RunTurnAngle = 180.0f;
	public float								m_Range = 5.0f;
	public float								m_AttackRange = 1.25f;
}

public class Enemy : Character 
{
	public enum EnemyStates
	{
		eES_Idle		= 0,
		eES_Walk,
		eES_Run,
		eES_Attack,
		eES_Attacked,
		eES_Dead,
		
		eES_Count
	}
	
	public enum EnemyCommands : uint
	{
		eEC_None			= 0,
		eEC_Idle			= 1 << 0,
		eEC_Walk		 	= 1 << 1,
		eEC_Run				= 1 << 2,
		eEC_Attack			= 1 << 3,
		eEC_Attacked		= 1 << 4,
	}
	
	public EnemyProperties						m_Properties;
	
	private StateMachine						m_EnemyStateMachine = new StateMachine();
	private State[]								m_AllStates;
	private Animation							m_Animation;
	private Flag								m_EnemyCommands = new Flag();
	private Vector3								m_TargetPos;
	private Vector3								m_LastFrameDir;
	
	private Body								m_EnemyBody = null;
	private float								m_CurrHealth;
	
	protected void Awake()
	{
		m_Animation = GetComponent<Animation>();
		m_AllStates = new State[(int)EnemyStates.eES_Count];
		m_TargetPos = transform.position;
		InitializeAllStates();
		m_CurrHealth = m_Properties.m_Health;
	}
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		m_LastFrameDir = transform.forward;
		
		//temporary
		if( m_AllActorTriggerBaseAttached.ContainsKey(typeof(Body)) == true )
		{
			m_EnemyBody = (Body)m_AllActorTriggerBaseAttached[typeof(Body)];
			m_EnemyBody.Enabled = true;
		//	Debug.Log("Body found");
		}
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		
		m_EnemyStateMachine.Update(Time.deltaTime);
		
		DebugDraw.DrawCircle(transform.position,m_Properties.m_Range,Vector3.up,9,Color.red);
		DebugDraw.DrawCircle(transform.position,m_Properties.m_AttackRange,Vector3.up,9,Color.green);
	}
	
	public override bool CanAttack(Actor attacker)
	{
		if( m_EnemyStateMachine.CurrentState == m_AllStates[(int)EnemyStates.eES_Dead].ID )
		{
			return false;
		}
		
		if( attacker.tag == "Player" )
		{
			return true;
		}
		return false;
	}

	// Called when any types weapon collides with this, returns true if it is successful attack
	// returns false if attack is blocked
	public override bool Attacked(Actor attacker, GameObject bodyPartGotHit , float damage)
	{
		//if( m_EnemyStateMachine.CurrentState != m_AllStates[(int)EnemyStates.eES_Attacked].ID )
		{
			m_EnemyCommands.SetFlag((uint)EnemyCommands.eEC_Attacked);
		}
		//if( m_EnemyStateMachine.CurrentState == m_AllStates[(int)EnemyStates.eES_Attack].ID )
		{
			m_EnemyStateMachine.CanChangeState = true;
		}
		
		m_CurrHealth -= damage * m_EnemyBody.m_PercentOfDamageReceive/100.0f;
		Debug.Log("Curr health.." + m_CurrHealth);
		//return false if attack is blocked..
		
		//Debug.Log("Enemy attacked..");
		return true;
	}
	
	// Called when attack is blocked
	public override void CurrentAttackBlocked(Actor defender)
	{
	}
	
	public void OnActivated()
	{
		
	}
	
	public void OnDeactivated()
	{
		
	}
	
	public Vector3 TargetPosition
	{
		get
		{
			return m_TargetPos;
		}
		set
		{
			m_TargetPos = value;
		}
	}
	
	public bool CanWalk
	{
		get
		{
			if( m_Animation["Walk"] == null )
			{
				return false;
			}
			return true;
		}
	}
	
	public bool CanRun
	{
		get
		{
			if( m_Animation["Run"] == null )
			{
				return false;
			}
			return true;
		}
	}
	
	
	public float Range
	{
		get
		{
			return m_Properties.m_Range;
		}
	}
	
	public float AttackRange
	{
		get
		{
			return m_Properties.m_AttackRange;
		}
	}
	
	//if it doesn't have walk animation it always goes to run
	//if it doesn't have run aniation it always goes to walk
	//if it doesn't have any of the animations it stays idle
	public void MoveToTarget(Vector3 targetPos, bool walk)
	{
		TargetPosition = targetPos;
		bool canWalk = CanWalk;
		bool canRun = CanRun;
		
		if( walk == true )
		{
			if( canWalk == true )
			{
				m_EnemyCommands.SetFlag((uint)EnemyCommands.eEC_Walk);
			}
			else
			{
				if( canRun == true )
				{
					m_EnemyCommands.SetFlag((uint)EnemyCommands.eEC_Run);
				}
			}
		}
		else
		{
			if( canRun == true )
			{
				m_EnemyCommands.SetFlag((uint)EnemyCommands.eEC_Run);
			}
			else
			{
				if( canWalk == true )
				{
					m_EnemyCommands.SetFlag((uint)EnemyCommands.eEC_Walk);
				}
			}
		}
		
		Vector3 dirVec = m_TargetPos - transform.position;
		dirVec.y = 0.0f;
		m_LastFrameDir = dirVec.normalized;
	}
	
	public void StayIdle()
	{
		m_EnemyCommands.SetFlag((uint)EnemyCommands.eEC_Idle);
	}
	
	public void Attack()
	{
		m_EnemyCommands.SetFlag((uint)EnemyCommands.eEC_Attack);
	}
	
	// -------------------------------------------------------------------------
	// Start State functions
	
	public void InitializeAllStates()
	{
		m_EnemyStateMachine.AddNewTransition(CheckAttackedState);
		m_EnemyStateMachine.AddNewTransition(CheckAttackState);
		m_EnemyStateMachine.AddNewTransition(CheckMoveStates);
		
		m_AllStates[(int)EnemyStates.eES_Idle] = m_EnemyStateMachine.AddNewState("Idle",BeginIdleState,UpdateIdleState,EndIdleState);
		m_AllStates[(int)EnemyStates.eES_Walk] = m_EnemyStateMachine.AddNewState("Walk",BeginMoveState,UpdateMoveState,EndMoveState);
		m_AllStates[(int)EnemyStates.eES_Run] = m_EnemyStateMachine.AddNewState("Run",BeginMoveState,UpdateMoveState,EndMoveState);
		m_AllStates[(int)EnemyStates.eES_Attack] = m_EnemyStateMachine.AddNewState("Attack",BeginAttackState,UpdateAttackState,null);
		m_AllStates[(int)EnemyStates.eES_Attacked] = m_EnemyStateMachine.AddNewState("Attacked",BeginAttackedState,UpdateAttackedState,null);
		m_AllStates[(int)EnemyStates.eES_Dead] = m_EnemyStateMachine.AddNewState("Dead",BeginDeadState,UpdateDeadState,null);
	}
	
	public State CheckMoveStates(State currState)
	{
		if( currState == null || m_EnemyCommands.CheckFlag((int)EnemyCommands.eEC_Idle) 
			|| currState.ID == m_AllStates[(int)EnemyStates.eES_Attacked].ID )
		{
			m_EnemyCommands.ClearFlag((int)EnemyCommands.eEC_Idle);
			return m_AllStates[(int)EnemyStates.eES_Idle];
		}
		
		if( m_EnemyCommands.CheckFlag((int)EnemyCommands.eEC_Run) )
		{
			m_EnemyCommands.ClearFlag((int)EnemyCommands.eEC_Run);
			return m_AllStates[(int)EnemyStates.eES_Run];
		}
		else if( m_EnemyCommands.CheckFlag((int)EnemyCommands.eEC_Walk) )
		{
			m_EnemyCommands.ClearFlag((int)EnemyCommands.eEC_Walk);
			return m_AllStates[(int)EnemyStates.eES_Walk];
		}
		
		if( 	currState.ID == m_AllStates[(int)EnemyStates.eES_Walk].ID 
			||	currState.ID == m_AllStates[(int)EnemyStates.eES_Run].ID )
		{
			if( TargetReached() )
			{
				return m_AllStates[(int)EnemyStates.eES_Idle];
			}
		}
		return null;
	}
	
	public State CheckAttackState(State currState)
	{
		if( m_EnemyCommands.CheckFlag((int)EnemyCommands.eEC_Attack) )
		{
			m_EnemyCommands.ClearFlag((int)EnemyCommands.eEC_Attack);
			return m_AllStates[(int)EnemyStates.eES_Attack];
		}
		return null;
	}
	
	public State CheckAttackedState(State currState)
	{
		if( m_CurrHealth <= 0.0f )
		{
			return m_AllStates[(int)EnemyStates.eES_Dead];
		}
		if( currState != null && currState.ID != m_AllStates[(int)EnemyStates.eES_Attacked].ID && m_EnemyCommands.CheckFlag((int)EnemyCommands.eEC_Attacked) )
		{
			m_EnemyCommands.ClearFlag((int)EnemyCommands.eEC_Attacked);
			return m_AllStates[(int)EnemyStates.eES_Attacked];
		}
		
		return null;	
	}
	
	// -----------------
	// Start Idle State
	public void BeginIdleState(State prvState, State newState)
	{
		m_AnimEventData = null;
		m_Animation.CrossFade("Idle");
		SetCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
	}
	
	public void UpdateIdleState(State currState, float deltaTime)
	{
		m_AnimEventData = null;
	}
	
	public void EndIdleState(State endingState, State newState)
	{
	
	}
	
	// End Idle State
	// -----------------
	
	// -----------------
	// Start Walk, Run State
	public void BeginMoveState(State prvState, State newState)
	{
		m_AnimEventData = null;
		SetCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
	}
	
	public void UpdateMoveState(State currState, float deltaTime)
	{
		if( currState.ID == m_AllStates[(int)EnemyStates.eES_Walk].ID )
		{
			m_Animation.CrossFade("Walk");
			Rotate(m_Properties.m_WalkTurnAngle, deltaTime);
		}
		else
		{
			m_Animation.CrossFade("Run");
			Rotate(m_Properties.m_RunTurnAngle, deltaTime);
		}
	}
	
	public void EndMoveState(State endingState, State newState)
	{
	
	}
	// End Walk, Run State
	// -----------------
	
	// -----------------
	// Start Attack State
	public void BeginAttackState(State prvState, State newState)
	{
		m_AnimEventData = null;
		m_Animation.CrossFade("Attack0");
		m_EnemyStateMachine.CanChangeState = false;
		//Debug.Break();
	}
	
	public void UpdateAttackState(State currState, float deltaTime)
	{
		if( m_Animation.IsPlaying("Attack0") == false )
		{
			m_EnemyStateMachine.CanChangeState = true;
		}
	}
	
	// End Attack State
	// -----------------
	
	// -----------------
	// Start Attacked State
	public void BeginAttackedState(State prvState, State newState)
	{
		m_AnimEventData = null;
		m_EnemyStateMachine.CanChangeState = false;
		if( m_Animation.IsPlaying("AttackedReaction0") == true )
		{
			m_Animation.Rewind("AttackedReaction0");
		}
		else
		{
			m_Animation.CrossFade("AttackedReaction0");
		}
	}
	
	public void UpdateAttackedState(State currState, float deltaTime)
	{
		if( m_EnemyCommands.CheckFlag((int)EnemyCommands.eEC_Attacked) )
		{
			m_EnemyCommands.ClearFlag((int)EnemyCommands.eEC_Attacked);
			if( m_Animation.IsPlaying("AttackedReaction0") == true )
			{
				//Debug.Log("Rewind..");
				m_EnemyStateMachine.CanChangeState = false;
				m_Animation.Rewind("AttackedReaction0");
			}
			else
			{
				m_Animation.CrossFade("AttackedReaction0");
			}
		}
		else if( m_Animation.IsPlaying("AttackedReaction0") == false )
		{
			//Debug.Log("Animation finished..");
			m_EnemyStateMachine.CanChangeState = true;
		}
	}
	
	// End Attacked State
	// -----------------
	
	// -----------------
	// Start Dead State
	public void BeginDeadState(State prvState, State newState)
	{
		m_AnimEventData = null;
		m_EnemyStateMachine.CanChangeState = false;
		m_Animation.CrossFade("Death");
	}
	
	public void UpdateDeadState(State currState, float deltaTime)
	{
		
	}
	
	// End Dead State
	// -----------------
	
	// End State functions
	// -------------------------------------------------------------------------
	
	private void Rotate(float turnAngleSpeed, float deltaTime)
	{
		Vector3 targetDir = TargetPosition - transform.position;
		targetDir.y = 0.0f;
		targetDir.Normalize();
		
		const float EPSILON = 0.95f;
		float dot = Vector3.Dot(targetDir,transform.forward);
			
		if( dot <= EPSILON )
		{
			float floatDir = Vector3.Dot(transform.up,Vector3.Cross(targetDir,transform.forward));
			
			if( floatDir >= 0.0f )
			{
				floatDir = 1.0f;
			}
			else
			{
				floatDir = -1.0f;
			}
				
			Quaternion newRot = Quaternion.AngleAxis(turnAngleSpeed * deltaTime,Vector3.up * floatDir * -1.0f );
			Vector3 newDir = newRot * transform.forward;
			newDir.y = 0.0f;
			newDir.Normalize();
			transform.forward = newDir;
			
			float newfloatDir = Vector3.Dot(transform.up,Vector3.Cross(targetDir,newDir));
			
			if( Mathf.Sign(floatDir) != Mathf.Sign(newfloatDir) )
			{
				transform.forward = targetDir;
			}
		}
		else
		{
			transform.forward = targetDir;
		}	
	}
	
	public bool TargetReached()
	{
		Vector3 dirVec = m_TargetPos - transform.position;
		dirVec.y = 0.0f;
		const float SQRDEPSILON = 0.01f;
		if( dirVec.sqrMagnitude < SQRDEPSILON || Vector3.Dot(m_LastFrameDir,dirVec) <= 0.0f )
		{
			//Debug.Log("Target reached...");
			return true;
		}
		
		m_LastFrameDir = dirVec.normalized;
		return false;
	}
}

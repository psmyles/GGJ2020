using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerProperties
{
	public float								m_WalkTurnAngle = 45.0f;
	public float								m_RunTurnAngle = 90.0f;
	public string[]								m_AttackComboAnims;
	public float								m_MaxComboAttackTimer = 0.5f;	// if attack button pressed after the first aniation finished,																			
																				// within this time limit it will play next animation
}

public class Player : Character 
{
	public enum PlayerStates
	{
		ePS_Idle = 0,		// Idle
		ePS_Run,			// Run
        ePS_Dash,           // Dash
        ePS_Attacked,		// Attacked
		
		ePS_Count
	}

	
	public PlayerProperties						m_Properties;

    [SerializeField]
    private DecalsManager                       m_DecalsManaer;

	private int									m_CurrAttackComboAnimIndex = 0;
	private float								m_AttackComboTimer;
	private BufferedInput						m_BufferedInput;
	private StateMachine						m_PlayerStateMachine = new StateMachine();
	private State[]								m_AllStates;
	private Animation							m_Animation;
	private	Vector3								m_RunVelocityAppliedToJump = Vector3.zero;

    [SerializeField]
    private float                               m_DefaultMoveSpeed = 0.0f;
    [SerializeField]
    private float                               m_MoveAcceleration = 10.0f;

    private bool                                m_bUseAnimEventDataForSpeed = false;

    private Vector2                             m_MoveDir = new Vector2(0.0f, 0.0f);
    private Vector2                             m_DashDir = new Vector2(0.0f, 0.0f);
    private Vector2                             m_CurrDashDir = new Vector2(0.0f, 0.0f);
    private float                               m_CurrentDashSpeed = 0.0f;
    [SerializeField]
    private float                               m_DashDeAcceleration = 10.0f;


    private Decal                               m_CurrentDecal = null;
    private Vector3                             m_LastDashDecalPointAdded;
    [SerializeField]
    private float                               m_DecalPointAddDist = 1.0f;

    private static readonly float               TURN_DIR_EPSILON = 0.00001f;

	protected void Awake()
	{
		m_BufferedInput = GetComponent<BufferedInput>();
		if( m_BufferedInput == null )
		{
			Debug.LogError("Buffered input is not attached to game object '" + gameObject.name + "'");
			Debug.Break();
		}
		//m_Animation = GetComponent<Animation>();
		m_AllStates = new State[(int)PlayerStates.ePS_Count];
		InitializeAllStates();
        //CurrentGravityScale = 0.15f;
        CurrentVelocity = new Vector3(0.0f, m_OnGroundYVelocity, 0.0f);
    }
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		m_AttackComboTimer = m_Properties.m_MaxComboAttackTimer;
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();
		
		m_PlayerStateMachine.Update(Time.deltaTime);
		//Debug.Log("CurrState outside : " + m_PlayerStateMachine.CurrentState.ToString());
		
		m_AttackComboTimer -= Time.deltaTime;
		if( m_AttackComboTimer < 0.0f )
		{
			m_AttackComboTimer = 0.0f;
		}
	}
	
	// Called before called Attacked if returns false Attacked will get called even if weapon
	// Collides with this actor
	public override bool CanAttack(Actor attacker)
	{
		if( attacker.tag == "Enemy" )
		{
			return true;
		}
		return false;
	}

	// Called when any types weapon collides with this, returns true if it is successful attack
	// returns false if attack is blocked
	public override bool Attacked(Actor attacker, GameObject bodyPartGotHit , float damage)
	{
	//	if( m_PlayerStateMachine.CurrentState != m_AllStates[(int)PlayerStates.ePS_Attacked].ID )
		{
			m_BufferedInput.AddInput("Attacked",0.05f);
			m_PlayerStateMachine.CanChangeState = true;
		}
		return true;
	}
	
	// Called when attack is blocked
	public override void CurrentAttackBlocked(Actor defender)
	{
	}
	
	// Called when an animation event happens
	public override void OnAnimEvent(AnimEventData data)
	{
		base.OnAnimEvent(data);
		if( data.GetType() == typeof(PlayerAnimEventData) )
		{

		}
		else
		{
			Debug.Log("AnimEventData not supported in Player");
		}
	}

    override protected bool CanUseAnimEventDataForSpeed() 
    {
        return m_bUseAnimEventDataForSpeed;
    }

    // -------------------------------------------------------------------------
    // Start State functions

    public void InitializeAllStates()
	{
        m_PlayerStateMachine.AddNewTransition(CheckAttackedState);
        m_PlayerStateMachine.AddNewTransition(CheckDashState);
        m_PlayerStateMachine.AddNewTransition(CheckMoveStates);
		
		m_AllStates[(int)PlayerStates.ePS_Idle] = m_PlayerStateMachine.AddNewState("Idle",BeginIdleState,UpdateIdleState,EndIdleState);
		m_AllStates[(int)PlayerStates.ePS_Run] = m_PlayerStateMachine.AddNewState("Run",BeginMoveState,UpdateMoveState,EndMoveState);
        m_AllStates[(int)PlayerStates.ePS_Dash] = m_PlayerStateMachine.AddNewState("Dash", BeginDashState, UpdateDashState, EndDashState);
        m_AllStates[(int)PlayerStates.ePS_Attacked] = m_PlayerStateMachine.AddNewState("Attacked", BeginAttackedState, UpdateAttackedState, null);
    }
	
	public State CheckMoveStates(State currState)
	{
        if (m_BufferedInput != null)
        {
            float forwardValue = 0.0f;
            float backwardValue = 0.0f;
            float turnLeftValue = 0.0f;
            float turnRightValue = 0.0f;
            bool forward = m_BufferedInput.CheckInput("MoveForward", out forwardValue);
            bool backward = m_BufferedInput.CheckInput("MoveBackward", out backwardValue);
            bool turnLeft = m_BufferedInput.CheckInput("MoveLeft", out turnLeftValue);
            bool turnRight = m_BufferedInput.CheckInput("MoveRight", out turnRightValue);

            //Debug.Log("Trun left : " + turnLeftValue + ", Turn RIght : " + turnRightValue);

            m_BufferedInput.RemoveInput("MoveForward");
            m_BufferedInput.RemoveInput("MoveBackward");
            m_BufferedInput.RemoveInput("MoveLeft");
            m_BufferedInput.RemoveInput("MoveRight");
            m_MoveDir = Vector2.zero;

            if ((forward && forwardValue > 0.0f) | (backward && backwardValue > 0.0f)
                || (turnLeft && turnLeftValue > 0.0f) || (turnRight && turnRightValue > 0.0f))
            {
                if (forward)
                {
                    m_MoveDir.y = forwardValue;
                }
                else if (backward) 
                {
                    m_MoveDir.y = -backwardValue;
                }

                if (turnRight)
                {
                    m_MoveDir.x = turnRightValue;
                }
                else if (turnLeft)
                {
                    m_MoveDir.x = -turnLeftValue;
                }

                return m_AllStates[(int)PlayerStates.ePS_Run];
            }
            else
            {
                return m_AllStates[(int)PlayerStates.ePS_Idle];
            }
        }

        return m_AllStates[(int)PlayerStates.ePS_Idle];
	}

    public State CheckDashState(State currState)
    {
        if (m_BufferedInput != null)
        {
            float forwardValue = 0.0f;
            float backwardValue = 0.0f;
            float turnLeftValue = 0.0f;
            float turnRightValue = 0.0f;
            bool forward = m_BufferedInput.CheckInput("DashForward", out forwardValue);
            bool backward = m_BufferedInput.CheckInput("DashBackward", out backwardValue);
            bool turnLeft = m_BufferedInput.CheckInput("DashLeft", out turnLeftValue);
            bool turnRight = m_BufferedInput.CheckInput("DashRight", out turnRightValue);

            //Debug.Log("Trun left : " + turnLeftValue + ", Turn RIght : " + turnRightValue);

            m_BufferedInput.RemoveInput("DashForward");
            m_BufferedInput.RemoveInput("DashBackward");
            m_BufferedInput.RemoveInput("DashLeft");
            m_BufferedInput.RemoveInput("DashRight");
            m_DashDir = Vector2.zero;

            if ((forward && forwardValue > 0.0f) | (backward && backwardValue > 0.0f)
                || (turnLeft && turnLeftValue > 0.0f) || (turnRight && turnRightValue > 0.0f))
            {
                if (forward)
                {
                    m_DashDir.y = forwardValue;
                }
                else if (backward)
                {
                    m_DashDir.y = -backwardValue;
                }

                if (turnRight)
                {
                    m_DashDir.x = turnRightValue;
                }
                else if (turnLeft)
                {
                    m_DashDir.x = -turnLeftValue;
                }

                return m_AllStates[(int)PlayerStates.ePS_Dash];
            }
        }

        return null;
    }

    public State CheckAttackedState(State currState)
	{
		if( currState == null )
		{
			return m_AllStates[(int)PlayerStates.ePS_Idle];
		}
		if( m_BufferedInput.CheckInput("Attacked") )
		{
			m_BufferedInput.RemoveInput("Attacked");
			return m_AllStates[(int)PlayerStates.ePS_Attacked];
		}
		return null;
	}
	
	// -----------------
	// Start Idle State
	public void BeginIdleState(State prvState, State newState)
	{
		//Debug.Log("Begin state : " + newState.Name);
		//m_Animation.Stop();
		m_AnimEventData = null;
		SetCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
	}
	
	public void UpdateIdleState(State currState, float deltaTime)
	{
        //CurrentVelocity = new Vector3(0.0f, m_OnGroundYVelocity, 0.0f);
    }
	
	public void EndIdleState(State endingState, State newState)
	{
		ClearCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
	}
	
	// End Idle State
	// -----------------
	
	// -----------------
	// Start Walk, Run State
	public void BeginMoveState(State prvState, State newState)
	{
        //m_Animation.Play();
        //SetCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
        CurrentVelocity = new Vector3(0.0f, m_OnGroundYVelocity, 0.0f);
    }
	
	public void UpdateMoveState(State currState, float deltaTime)
	{
        CurrentMoveSpeed = m_DefaultMoveSpeed;
			//m_Animation.CrossFade("Run");

        Vector3 turnDir = new Vector3(m_MoveDir.x, 0.0f, m_MoveDir.y);
        if (turnDir.sqrMagnitude >= TURN_DIR_EPSILON)
        {
            turnDir.Normalize();
            transform.forward = Vector3.Slerp(transform.forward, turnDir, 0.1f);
        }
    }
	
	public void EndMoveState(State endingState, State newState)
	{
        CurrentMoveSpeed = 0;
        ClearCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
	}
    // End Walk, Run State
    // -----------------

    // -----------------
    // Start Dash State
    public void BeginDashState(State prvState, State newState)
    {
        //m_Animation.Play();
        //SetCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
        CurrentVelocity = new Vector3(0.0f, m_OnGroundYVelocity, 0.0f);
        m_CurrentDashSpeed = m_DefaultMoveSpeed;// Can be current speed.
        m_CurrDashDir = m_DashDir;
        m_PlayerStateMachine.CanChangeState = false;

        m_CurrentDecal = m_DecalsManaer.CreateDecal(DecalsManager.DecalsType.DT_Tape);
        m_CurrentDecal.AddPoint(transform.position);
        m_CurrentDecal.UpVector = transform.up;
        m_LastDashDecalPointAdded = transform.position;
    }

    public void UpdateDashState(State currState, float deltaTime)
    {
        CurrentMoveSpeed = m_CurrentDashSpeed;
        m_CurrentDashSpeed -= m_DashDeAcceleration * deltaTime;

        if (m_CurrentDashSpeed <= 0.0f)
        {
            m_PlayerStateMachine.CanChangeState = true;
        }

        Vector3 turnDir = new Vector3(m_CurrDashDir.x, 0.0f, m_CurrDashDir.y);
        if (turnDir.sqrMagnitude >= TURN_DIR_EPSILON)
        {
            turnDir.Normalize();
            // Rotate(turnDir, m_Properties.m_RunTurnAngle, deltaTime);
            transform.forward = Vector3.Slerp(transform.forward, turnDir, 0.1f);
        }

        if (Vector3.SqrMagnitude(m_LastDashDecalPointAdded - transform.position) >= (m_DecalPointAddDist * m_DecalPointAddDist))
        {
            m_CurrentDecal.AddPoint(transform.position);
            m_LastDashDecalPointAdded = transform.position;
        }
    }

    public void EndDashState(State endingState, State newState)
    {
        CurrentMoveSpeed = 0;
        ClearCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
    }
    // End Dash State
    // -----------------

	// -----------------
	// Start Attack State
	public void BeginAttackedState(State prvState, State newState)
	{
		//m_Animation.CrossFade("AttackedReaction0");
		m_PlayerStateMachine.CanChangeState = false;
	}
	
	public void UpdateAttackedState(State currState, float deltaTime)
	{
		if( m_Animation.IsPlaying("AttackedReaction") == false )
		{
			//Debug.Log("Animation finished..");
			m_PlayerStateMachine.CanChangeState = true;
		}
	}
	
	// End Attacked State
	// -----------------
	
	// End State functions
	// -------------------------------------------------------------------------
}

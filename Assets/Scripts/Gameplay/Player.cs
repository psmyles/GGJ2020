using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player : Character 
{
	public enum PlayerStates
	{
		ePS_Idle = 0,		// Idle
		ePS_Run,			// Run
        ePS_Dash,           // Dash
        ePS_Attacked,		// Attacked
        ePS_Dead,           // Death
		
		ePS_Count
	}

    [SerializeField]
    private DecalsManager                       m_DecalsManaer;

	private BufferedInput						m_BufferedInput;
	private StateMachine						m_PlayerStateMachine = new StateMachine();
	private State[]								m_AllStates;
	private Animation							m_Animation;
	private	Vector3								m_RunVelocityAppliedToJump = Vector3.zero;

    [SerializeField]
    private float                               m_DefaultMoveSpeed = 10.0f;

    [SerializeField]
    private float                               m_PlayerTurnAngSpeed = 360.0f;

    private bool                                m_bUseAnimEventDataForSpeed = false;

    private Vector2                             m_MoveDir = new Vector2(0.0f, 0.0f);
    private Vector2                             m_DashDir = new Vector2(0.0f, 0.0f);
    private float                               m_CurrentDashSpeed = 0.0f;


    private Decal                               m_CurrentDecal = null;
    private Vector3                             m_LastDashDecalPointAdded;
    [SerializeField]
    private float                               m_DecalPointAddDist = 1.0f;
    [SerializeField]
    private float                               m_DecalThickness = 0.25f;

    private static readonly float               TURN_DIR_EPSILON = 0.00001f;

    [SerializeField]
    private float                               m_DecalUpOffset = 0.01f;

    private Vector3                             m_StartDashInputDir;
    private Vector3                             m_CurrDashInputDir;

    [SerializeField]
    private float                               m_RunTimeToDash = 0.25f;
    private float                               m_CurrRunTime = 0.0f;

    [SerializeField]
    private float                               m_InputRotation = -90.0f;

    [SerializeField]
    private float                               m_MaxDashTime = 0.5f;
    private float                               m_CurrDashTime = 0.0f;
    [SerializeField]
    private float                               m_DashSpeedMultiplier = 1.25f;

    [SerializeField]
    private float                               m_PlayerMaxHealth = 400.0f;
    private float                               m_PlayerHealth;

    [SerializeField]
    private PlayerHUD                           m_PlayerHUD;

    [SerializeField]
    private AudioSource                         m_GettingHitAudio;

    private bool                                m_DeathStateCompleted = false;

    public bool DeathStateCompleted
    {
        get { return m_DeathStateCompleted; }
    }


    protected void Awake()
	{
		m_BufferedInput = GetComponent<BufferedInput>();
		if( m_BufferedInput == null )
		{
			Debug.LogError("Buffered input is not attached to game object '" + gameObject.name + "'");
			Debug.Break();
		}
		m_Animation = GetComponent<Animation>();
		m_AllStates = new State[(int)PlayerStates.ePS_Count];
		InitializeAllStates();
        //CurrentGravityScale = 0.15f;
        CurrentVelocity = new Vector3(0.0f, m_OnGroundYVelocity, 0.0f);
        m_PlayerHealth = m_PlayerMaxHealth;

        //Application.targetFrameRate = 120;
    }
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();

        m_PlayerHUD.ShowMeter();
    }
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();
		
		/*m_PlayerStateMachine.Update(Time.deltaTime);

        float Percent = Mathf.Clamp((m_RunTimeToDash - m_CurrRunTime) / m_RunTimeToDash, 0.0f, 1.0f);
        m_PlayerHUD.SetMeterPercentage(Percent);*/
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        m_PlayerStateMachine.Update(Time.deltaTime);

        float Percent = Mathf.Clamp((m_RunTimeToDash - m_CurrRunTime) / m_RunTimeToDash, 0.0f, 1.0f);
        m_PlayerHUD.SetMeterPercentage(Percent);
    }
	
	// Called before called Attacked if returns false Attacked will get called even if weapon
	// Collides with this actor
	public override bool CanAttack(Actor attacker)
	{
        if (attacker.GetType().IsSubclassOf(typeof(Projectile)))
        {
            return true;
        }

        return false;
    }

	// Called when any types weapon collides with this, returns true if it is successful attack
	// returns false if attack is blocked
	public override bool Attacked(Actor attacker, GameObject bodyPartGotHit , float damage)
	{
        if (attacker.GetType().IsSubclassOf(typeof(Projectile)))
        {
            Projectile proj = (Projectile)attacker;
            if (proj != null)
            {
                proj.DestroyWithoutDecals();
            }
            if (m_PlayerStateMachine.CurrentState != (int)PlayerStates.ePS_Dead)
            {
                m_PlayerHealth -= damage;
                GameScoreManager.Get().PlayerHealth = Mathf.Clamp(m_PlayerHealth / m_PlayerMaxHealth, 0.0f, 1.0f);
                m_BufferedInput.AddInput("Attacked", 0.05f);
                m_PlayerStateMachine.CanChangeState = true;
            }
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
        m_AllStates[(int)PlayerStates.ePS_Dead] = m_PlayerStateMachine.AddNewState("Dead", BeginDead, UpdateDead, null); 
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

                Vector3 NewMoveDir = Quaternion.AngleAxis(m_InputRotation, Vector3.up) * (new Vector3(m_MoveDir.x, 0.0f, m_MoveDir.y));
                m_MoveDir = new Vector2(NewMoveDir.x, NewMoveDir.z);

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
        if (m_BufferedInput != null && m_CurrRunTime <= 0.0f)
        {
            float forwardValue = 0.0f;
            float backwardValue = 0.0f;
            float turnLeftValue = 0.0f;
            float turnRightValue = 0.0f;
            bool forward = m_BufferedInput.CheckInput("DashForward", out forwardValue);
            bool backward = m_BufferedInput.CheckInput("DashBackward", out backwardValue);
            bool turnLeft = m_BufferedInput.CheckInput("DashLeft", out turnLeftValue);
            bool turnRight = m_BufferedInput.CheckInput("DashRight", out turnRightValue);
            bool dash = m_BufferedInput.CheckInput("Dash");

            //Debug.Log("Trun left : " + turnLeftValue + ", Turn RIght : " + turnRightValue);

            m_BufferedInput.RemoveInput("DashForward");
            m_BufferedInput.RemoveInput("DashBackward");
            m_BufferedInput.RemoveInput("DashLeft");
            m_BufferedInput.RemoveInput("DashRight");
            m_DashDir = Vector2.zero;

            if (dash || (forward && forwardValue > 0.0f) | (backward && backwardValue > 0.0f)
                || (turnLeft && turnLeftValue > 0.0f) || (turnRight && turnRightValue > 0.0f))
            {
                if (dash)
                {
                    m_DashDir = transform.forward;
                }
                else
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

                    Vector3 NewMoveDir = Quaternion.AngleAxis(m_InputRotation, Vector3.up) * (new Vector3(m_DashDir.x, 0.0f, m_DashDir.y));
                    m_DashDir = new Vector2(NewMoveDir.x, NewMoveDir.z);
                }

                return m_AllStates[(int)PlayerStates.ePS_Dash];
            }
        }

        return null;
    }

    public State CheckAttackedState(State currState)
	{
        if (m_BufferedInput.CheckInput("Death"))
        {
            m_BufferedInput.RemoveInput("Death");
            return m_AllStates[(int)PlayerStates.ePS_Dead];
        }
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
		m_AnimEventData = null;
        CurrentMoveSpeed = 0.0f;
        SetCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
        m_CurrRunTime = m_RunTimeToDash;
        m_PlayerHUD.ShowMeter();
    }
	
	public void UpdateIdleState(State currState, float deltaTime)
	{
        //CurrentVelocity = new Vector3(0.0f, m_OnGroundYVelocity, 0.0f);
        m_Animation.CrossFade("Idle");
    }
	
	public void EndIdleState(State endingState, State newState)
	{
		ClearCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
        m_CurrRunTime = m_RunTimeToDash;
    }
	
	// End Idle State
	// -----------------
	
	// -----------------
	// Start Walk, Run State
	public void BeginMoveState(State prvState, State newState)
	{
        CurrentVelocity = new Vector3(0.0f, m_OnGroundYVelocity, 0.0f);
        m_CurrRunTime = m_RunTimeToDash;
        m_PlayerHUD.ShowMeter();
    }
	
	public void UpdateMoveState(State currState, float deltaTime)
	{
        CurrentMoveSpeed = m_DefaultMoveSpeed;
		m_Animation.CrossFade("Run");

        Vector3 turnDir = new Vector3(m_MoveDir.x, 0.0f, m_MoveDir.y);
        if (turnDir.sqrMagnitude >= TURN_DIR_EPSILON)
        {
            turnDir.Normalize();
            Vector3 forward = Rotate(transform.forward, turnDir, Mathf.Deg2Rad * m_PlayerTurnAngSpeed, deltaTime);//Vector3.Slerp(transform.forward, turnDir, 0.2f);
            //transform.forward = forward.normalized;
            transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        m_CurrRunTime -= Time.deltaTime;
    }
	
	public void EndMoveState(State endingState, State newState)
	{
        CurrentMoveSpeed = 0;
        ClearCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);
	}

    public Vector3 Rotate(Vector3 currDir, Vector3 targetDir, float angleSpeedRad, float deltaTime)
    {
        float radAngle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(currDir, targetDir), 0.0f, 1.0f));
        float angleToRotate = angleSpeedRad * deltaTime;

        float targetAngle = Mathf.Clamp(radAngle - angleToRotate, 0.0f, Mathf.PI);

        //Debug.Log("## Target angle : " + targetAngle);

        Vector3 rotAxis = Vector3.Cross(targetDir, currDir);
        const float EPSILON = 0.000001f;
        if (rotAxis.sqrMagnitude <= EPSILON)
        {
            rotAxis = Vector3.up;
        }
        else
        {
            rotAxis.Normalize();
        }

        return Quaternion.AngleAxis(Mathf.Rad2Deg * targetAngle, rotAxis) * targetDir;
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
        m_CurrentDashSpeed = m_DefaultMoveSpeed * m_DashSpeedMultiplier;// Can be current speed.
        CurrentMoveSpeed = m_CurrentDashSpeed;
        m_PlayerStateMachine.CanChangeState = false;
        m_CurrRunTime = m_RunTimeToDash;

        m_CurrentDecal = m_DecalsManaer.CreateDecal(DecalsManager.DecalsType.DT_Tape);

        RaycastHit rayHit;
        if (PhysicsUtils.RaycastToGround(transform.position, out rayHit))
        {
            m_LastDashDecalPointAdded = rayHit.point;
        }
        else
        {
            m_LastDashDecalPointAdded = transform.position;
        }

        m_CurrentDecal.Thickness = m_DecalThickness;
        m_CurrentDecal.AddPoint(m_LastDashDecalPointAdded + Vector3.up * m_DecalUpOffset);
        m_CurrentDecal.UpVector = transform.up;

        m_CurrDashTime = m_MaxDashTime;
        m_Animation.CrossFade("Run");

        m_PlayerHUD.ShowMeter();
    }

    public void UpdateDashState(State currState, float deltaTime)
    {
        m_CurrDashTime -= deltaTime;
        if (m_CurrDashTime <= 0.0f)
        {
            m_PlayerStateMachine.CanChangeState = true;
        }

        m_Animation.CrossFade("Run");

        if (Vector3.SqrMagnitude(m_LastDashDecalPointAdded - transform.position) >= (m_DecalPointAddDist * m_DecalPointAddDist))
        {
            RaycastHit rayHit;
            if (PhysicsUtils.RaycastToGround(transform.position, out rayHit))
            {
                m_LastDashDecalPointAdded = rayHit.point;
            }
            else
            {
                m_LastDashDecalPointAdded = transform.position;
            }

            m_CurrentDecal.AddPoint(m_LastDashDecalPointAdded + Vector3.up * m_DecalUpOffset);
        }
        m_CurrRunTime = m_RunTimeToDash;
    }

    public void EndDashState(State endingState, State newState)
    {
        ClearCharacterFlag(CharacterFlag.eCF_ResetMoveSpeedAfterUse);

        if (m_CurrentDecal != null)
        {
            m_CurrentDecal.SetToDestroy();
        }
    }
    // End Dash State
    // -----------------

	// -----------------
	// Start Attack State
	public void BeginAttackedState(State prvState, State newState)
	{
		m_Animation.CrossFade("HitReaction");
		m_PlayerStateMachine.CanChangeState = false;
        CurrentMoveSpeed = 0.0f;
        m_PlayerHUD.ShowOuch();
        m_GettingHitAudio.Play();
    }
	
	public void UpdateAttackedState(State currState, float deltaTime)
	{
		if( m_Animation.IsPlaying("HitReaction") == false )
		{
			//Debug.Log("Animation finished..");
			m_PlayerStateMachine.CanChangeState = true;
            if (GameScoreManager.Get().PlayerHealth == 0.0f)
            {
                m_BufferedInput.AddInput("Death", 0.1f);
            }
        }
	}

    // End Attacked State
    // -----------------

    // -----------------
    // Start Attack State
    public void BeginDead(State prvState, State newState)
    {
        m_Animation.CrossFade("Death");
        m_PlayerStateMachine.CanChangeState = false;
        CurrentMoveSpeed = 0.0f;
        m_PlayerHUD.HideAll();
        m_GettingHitAudio.Play();
    }

    public void UpdateDead(State currState, float deltaTime)
    {
        if (m_Animation.IsPlaying("Death") == false)
        {
            m_DeathStateCompleted = true;
        }
    }

    // End Attacked State
    // -----------------

    // End State functions
    // -------------------------------------------------------------------------

    public void MakePlayerDie()
    {
        m_BufferedInput.AddInput("Death", 0.1f);
    }
}

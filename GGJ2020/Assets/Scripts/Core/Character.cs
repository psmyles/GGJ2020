using UnityEngine;
using System.Collections;

public class Character : Actor
{	
	public enum CharacterFlag : uint
	{
		eCF_None							= 0,
		eCF_ResetMoveSpeedAfterUse			= 1 << 0,
		eCF_Jumping							= 1 << 1,
	}
	
	private CollisionFlags				m_CollisionFlags; 
	private CharacterController 		m_CharacterController = null;
	private Vector3						m_CurrVelocity = Vector3.zero;
	private float						m_CurrentMoveSpeed = 0.0f;
	private float						m_CurrentGravityScale = 1.0f;
	private Flag						m_Flag = new Flag();
	protected AnimEventData				m_AnimEventData = null;
	private Vector3						m_AccelerationForce = Vector3.zero;
	public int							m_MaxFrameToNotGrounded = 4;
	private int							m_FrameNotGrounded = 0;
	private	Vector3						m_LastFrameMoveVelocity = Vector3.zero;
	protected float						m_OnGroundYVelocity = -0.025f;
			
	// Use this for initialization
	protected override void Start () 
	{
		m_CharacterController = GetComponent<CharacterController>();
		if( m_CharacterController == null )
		{
			Debug.LogError("Character doesn't have a 'Character controller'");
			Debug.Break();
		}
		m_CollisionFlags = m_CharacterController.Move(m_CurrVelocity);
		
		m_FrameNotGrounded = 0;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		if(IsPhysicsGrounded () != true)
		{
			m_FrameNotGrounded++;
		}
		else
		{
			m_FrameNotGrounded = 0;
		}
		
		ApplyAnimEventData(m_AnimEventData);
		ApplyGravityAndForce(Time.deltaTime);
	
		Vector3 currTotalVel = m_CurrVelocity;
		if( IsGrounded() )
		{
			currTotalVel += transform.forward * m_CurrentMoveSpeed;
		}	
		
		m_LastFrameMoveVelocity = currTotalVel;
		
		m_CollisionFlags = m_CharacterController.Move(currTotalVel * Time.deltaTime);
        
		if( m_Flag.CheckFlag((uint)CharacterFlag.eCF_ResetMoveSpeedAfterUse) == true )
		{
			CurrentMoveSpeed = 0.0f;
		}
	}
	
	protected void ApplyGravityAndForce (float deltaTime)
	{
		m_CurrVelocity += (Physics.gravity * m_CurrentGravityScale + m_AccelerationForce) * deltaTime;	
		if( IsGrounded() == true && m_Flag.CheckFlag((uint)CharacterFlag.eCF_Jumping) == false )
		{
			m_CurrVelocity.y = m_OnGroundYVelocity;
			//Debug.Log("Grounded....");
		}
		//Debug.Log("Curr Speed.." + m_CurrVelocity);
		m_AccelerationForce = Vector3.zero;
	}
	
	public bool IsGrounded () 
	{
		return (m_FrameNotGrounded < m_MaxFrameToNotGrounded) && !m_Flag.CheckFlag((uint)CharacterFlag.eCF_Jumping);
	}
	
	public bool IsPhysicsGrounded ()
	{
		return m_CharacterController.isGrounded;
	}
	
	public void Jump(Vector3 velocity)
	{
		if( m_Flag.CheckFlag((uint)CharacterFlag.eCF_Jumping) == false )
		{
			m_CurrVelocity += velocity;
			m_Flag.SetFlag((uint)CharacterFlag.eCF_Jumping);
		}
	}
	
	public void StopJump()
	{
		m_Flag.ClearFlag((uint)CharacterFlag.eCF_Jumping);
	}
	
	// Called before called Attacked if returns false Attacked will get called even if weapon
	// Collides with this actor
	public override bool CanAttack(Actor attacker)
	{
		return true;
	}

	// Called when any types weapon collides with this, returns true if it is successful attack
	// returns false if attack is blocked
	public override bool Attacked(Actor attacker, GameObject bodyPartGotHit , float damage)
	{
		return true;
	}
	
	// Called when attack is blocked
	public override void CurrentAttackBlocked(Actor defender)
	{
	}
	
	// Called when an animation event happens
	public override void OnAnimEvent(AnimEventData data)
	{
		m_AnimEventData = data;
		// Because this needs to called only once
		// thats why its not inside function ApplyAnimEventData;
		if( data.m_TriggerData != null )
		{
			//SetToDefaultEnabledAllActorTriggerBase();
			for( int i = 0 ; i < data.m_TriggerData.Length ; i++ )
			{
				if( data.m_TriggerData[i].m_TriggerType != null )
				{
					//Debug.Log("Enabled trigger " + data.m_TriggerData[i].m_TriggerType + " Enabled " + data.m_TriggerData[i].m_Enabled);
					SetEnableActorTriggerBase(data.m_TriggerData[i].m_TriggerType,data.m_TriggerData[i].m_Enabled);
				}
				else
				{
					Debug.LogError("Wrong trigger type is assigned to AnimEventData for game object '" + gameObject.name + "'");
				}
			}
		}
	}
	
	protected void ApplyAnimEventData(AnimEventData data)
	{
		if( data != null )
		{
            if (CanUseAnimEventDataForSpeed())
			    CurrentMoveSpeed = data.m_TranslationSpeed;
			/*if( GetType() == typeof(Enemy) )
			{
				Debug.Log("Transform speed.. " + m_CurrentMoveSpeed);
			}*/
		}
		else
		{
			SetToDefaultEnabledAllActorTriggerBase();
            if (CanUseAnimEventDataForSpeed())
                CurrentMoveSpeed = 0.0f;
		}
	}
	
	public void SetCharacterFlag(CharacterFlag flag)
	{
		m_Flag.SetFlag((uint)flag);
	}
	
	public void ClearCharacterFlag(CharacterFlag flag)
	{
		m_Flag.ClearFlag((uint)flag);
	}
	
	public void ResetCharacterFlag()
	{
		m_Flag.Reset();
	}
	
	public float CurrentMoveSpeed
	{
		get
		{
			return m_CurrentMoveSpeed;
		}
		set 
		{
			m_CurrentMoveSpeed = value;
		}
	}
	
	public float CurrentGravityScale
	{
		get
		{
			return m_CurrentGravityScale;
		}
		set 
		{
			m_CurrentGravityScale = value;
		}
	}
	
	public Vector3 AccelerationForce
	{
		get
		{
			return m_AccelerationForce;
		}
		set
		{
			m_AccelerationForce = value;
		}
	}
	
	public Vector3 CurrentVelocity
	{
		get
		{
			return m_CurrVelocity;
		}
		set
		{
			m_CurrVelocity = value;
		}
	}
	
	public Vector3 LastFrameMoveVelocity
	{
		get
		{
			return m_LastFrameMoveVelocity;
		}
	}

    virtual protected bool CanUseAnimEventDataForSpeed() { return true; }
}

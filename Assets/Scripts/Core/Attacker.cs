using UnityEngine;
using System.Collections;

public class Attacker : ActorTriggerBase 
{
	// How much damage this attacker will deal when it hits a defender
	public float					m_AttackDamage;
	
	private Rigidbody				m_RigidBody;
	
	private bool					m_ThisFrameAttacked;

    public bool                     m_DisableAfterAttack = true;
	
	protected override void Awake()
	{
		base.Awake();
	}
	
	// Use this for initialization
	protected override void Start () 
	{	
		base.Start();
		m_RigidBody = gameObject.GetComponent<Rigidbody>();
		
		if( m_RigidBody == null )
		{
			Debug.LogError("Attacker doesn't have a RigidBody");
			Debug.Break();
		}
		m_RigidBody.isKinematic = true;
		m_ThisFrameAttacked = false;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
        base.Update();

        if ( m_ThisFrameAttacked == true && m_DisableAfterAttack)
		{
			//Debug.Log("Enabled false");
			Enabled = false;
			m_ThisFrameAttacked = false;
		}
	}
	
	protected virtual void OnTriggerEnter (Collider other) 
	{	
        if (m_ParentActorAttachedTo.GetComponent<Patch>() != null)
        {
            //Debug.Log("############ Patched ....");
        }
		if(Enabled == true && m_ParentActorAttachedTo != null && !JustSpawned)
		{
			Defender defender = other.gameObject.GetComponent<Defender>();
			if( defender != null && !defender.JustSpawned)
			{	
				Actor defenderActor = defender.ParentActorAttachedTo;

                if (m_ParentActorAttachedTo.GetComponent<Patch>() != null)
                {
                    //Debug.Log("############ Patched ....");
                }

                if ( defenderActor != null && defenderActor != m_ParentActorAttachedTo && defender.CanAttack(m_ParentActorAttachedTo) == true )
				{
                    if (m_ParentActorAttachedTo.GetComponent<Patch>() != null)
                    {
                        //Debug.Log("############ Patched Inside.......");
                    }
                    //Debug.Log("Try attack..");
                    if ( defender.Attacked(m_ParentActorAttachedTo,m_AttackDamage) == false )
					{
						CurrentAttackBlocked(defenderActor);
					}
					else 
					{
						m_ThisFrameAttacked = true;
                        CurrentAttackSuccedded(defenderActor);

                    }
				}
			}
		}
	}
	
	protected void CurrentAttackBlocked(Actor defender)
	{
		if( Enabled == true )
		{
			m_ParentActorAttachedTo.CurrentAttackBlocked(defender);
		}
	}

    protected void CurrentAttackSuccedded(Actor defender)
    {
        //if (Enabled == true)
        {
            m_ParentActorAttachedTo.CurrentAttackSuccedded(defender);
        }
    }
}

using UnityEngine;
using System.Collections;

public class Defender : ActorTriggerBase 
{
	// The percentage of damage to receive from Attacker
	public float 				m_PercentOfDamageReceive;
	
	protected override void Awake()
	{
		base.Awake();
	}
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
        base.Update();
	}
	
	// Called before called Attacked if returns false Attacked will get called even if weapon
	// Collides with this actor
	public virtual bool CanAttack(Actor attacker)
	{
		if( Enabled == true && m_ParentActorAttachedTo != null )
		{
			return m_ParentActorAttachedTo.CanAttack(attacker);
		}
		return false;
	}
	
	// Called when any types weapon collides with this, returns true if it is successful attack
	// returns false if attack is blocked
	public virtual bool Attacked(Actor attacker, float damage)
	{
		if( Enabled == true && m_ParentActorAttachedTo != null )
		{
			float finalDamage = damage * m_PercentOfDamageReceive/100.0f;
			return m_ParentActorAttachedTo.Attacked(attacker,transform.gameObject,finalDamage);
		}
		
		return false;
	}
}

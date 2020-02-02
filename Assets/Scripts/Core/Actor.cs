using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Actor : MonoBehaviour 
{	
	protected Dictionary<System.Type,ActorTriggerBase>	m_AllActorTriggerBaseAttached = new Dictionary<System.Type, ActorTriggerBase>(); 
	
	private bool										m_AllActorTriggerBaseSetToDefault = false;
	// Use this for initialization
	protected abstract void Start ();
	
	// Update is called once per frame
	protected abstract void Update ();

	// Called before called Attacked if returns false Attacked will get called even if weapon
	// Collides with this actor
	public abstract bool CanAttack(Actor attacker);

	// Called when any types weapon collides with this, returns true if it is successful attack
	// returns false if attack is blocked
	public abstract bool Attacked(Actor attacker, GameObject bodyPartGotHit , float damage);
	
	// Called when attack is blocked
	public abstract void CurrentAttackBlocked(Actor defender);

    public abstract void CurrentAttackSuccedded(Actor defender);

    // Called when animation event happens
    public abstract void OnAnimEvent(AnimEventData data);
	
	
	// Called if actor trigger is attached
	// TODO: if i need to add multiple triggers of same type then try with Multiple dictionaries
	// Ex. Dictionary<System.Type,List<ActorTriggerBase>>
	public virtual void ActorTriggerAttached(ActorTriggerBase triggerBase)
	{
		if( m_AllActorTriggerBaseAttached.ContainsKey(triggerBase.GetType()) != true )
		{
			m_AllActorTriggerBaseAttached.Add(triggerBase.GetType(),triggerBase);
			triggerBase.Enabled = true;
		}
		else
		{
			triggerBase.gameObject.SetActive(false);
			Debug.LogError("Already 'ActorTriggerBase' of type '" + triggerBase.GetType() + "' attached to game object '" + gameObject.name + "'");
			Debug.Break();
		}
	}
	
	// Called when actor trigger is detached
	public virtual void ActorTriggerDetached(ActorTriggerBase triggerBase)
	{
		if( m_AllActorTriggerBaseAttached.ContainsKey(triggerBase.GetType()) == true )
		{
			if( m_AllActorTriggerBaseAttached.ContainsValue(triggerBase) == true )
			{
				triggerBase.Enabled = false;
				if( m_AllActorTriggerBaseAttached.Remove(triggerBase.GetType()) == false )
				{
					triggerBase.gameObject.SetActive(false);
					Debug.LogError("Unable to remove 'ActorTriggerBase' of type '" + triggerBase.GetType() + "' attached to game object '" + gameObject.name + "'");
					Debug.Break();	
				}
			}
		}
	}
	
	public void SetEnableActorTriggerBase(System.Type triggerType, bool enabled)
	{
		if( m_AllActorTriggerBaseAttached.ContainsKey(triggerType) )
		{
			m_AllActorTriggerBaseAttached[triggerType].Enabled = enabled;
			//m_AllActorTriggerBaseSetToDefault = enabled;
		}
	}

    public void SetEnableAllActorTriggerBase(bool enabled)
    {
        foreach (var val in m_AllActorTriggerBaseAttached)
        {
            val.Value.Enabled = enabled;
            //m_AllActorTriggerBaseSetToDefault = enabled;
        }
    }
}

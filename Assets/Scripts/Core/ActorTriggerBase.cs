using UnityEngine;
using System.Collections;

public abstract class ActorTriggerBase : MonoBehaviour 
{
	// Nearest top most parent actor of the attacker
	// If its not attached to any parent it checks for its uses own game object
	protected Actor 			m_ParentActorAttachedTo;
	protected bool				m_Enabled;
    protected bool              m_JustSpawned = true;

    public bool JustSpawned
    {
        get { return m_JustSpawned; }
    }
	
	protected virtual void Awake()
	{
		Transform itrParent = transform;
		Transform parent = null;
		
		while(itrParent != null )
		{
			parent = itrParent;
			m_ParentActorAttachedTo = parent.gameObject.GetComponent<Actor>();
			if( m_ParentActorAttachedTo != null )
			{
				break;
			}
			
			itrParent = itrParent.parent;
		}
		
		if( m_ParentActorAttachedTo != null )
		{
			m_ParentActorAttachedTo.ActorTriggerAttached(this);
		}
	}
	
	// Use this for initialization
	protected virtual void Start () 
	{
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        m_JustSpawned = false;
    }
	
	public Actor ParentActorAttachedTo
    {
		get
	    {
	        return m_ParentActorAttachedTo;
	    }
	}
	
	public bool Enabled
	{
		get 
		{
			return m_Enabled;
		}
		set
		{
			//Debug.Log("Set enabled for " + this.GetType().ToString() + " " + value);
			gameObject.active = value;
			m_Enabled = value;
		}
	}
	
	public bool IsType(string typeName)
	{
		return (this.GetType().ToString() == typeName);
	}
}

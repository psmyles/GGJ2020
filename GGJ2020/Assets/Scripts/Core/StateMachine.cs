using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class State
{
	private string				m_Name;
	private	uint				m_UniqueID; // this will be only unique to current state machine owner
	
	protected State(string name, uint uniqueID)
	{
		m_Name = name;
		m_UniqueID = uniqueID;
	}
	
	//returns the name of the State
	public string Name
    {
		get
	    {
	        return m_Name;
	    }
	}
	
	public uint ID
	{
		get
		{
			return m_UniqueID;
		}
	}
}

public class StateMachine
{
	public delegate State TransitionFun(State currState);
	public delegate void BeginStateFun(State prvState, State newState);
	public delegate void UpdateStateFun(State currState, float deltaTime);
	public delegate void EndStateFun(State endingState, State newState);
	
	// -------------------------------------------------------------------------
	// Start State implementation
	private class StateImpl : State
	{
		private BeginStateFun 		m_BeginStateFunction;
		private UpdateStateFun	 	m_UpdateStateFunction;
		private EndStateFun 		m_EndStateFunction;
		
		public StateImpl(string name, uint uniqueID, BeginStateFun beginState, UpdateStateFun updateState, EndStateFun endState)
			: base(name,uniqueID)
		{
			m_BeginStateFunction = beginState;
			m_UpdateStateFunction = updateState;
			m_EndStateFunction = endState;
			
			if( m_BeginStateFunction == null )
			{
				Debug.LogError("Begin state is null in constructor of " + Name);
				Debug.Break();
			}
			
			if( m_UpdateStateFunction == null )
			{
				Debug.LogError("Update state is null in constructor of " + Name);
				Debug.Break();
			}
			
		}
		
		public void BeginState(State prvState, State newState)
		{
			if( m_BeginStateFunction == null )
			{
				Debug.LogError("Begin state is null in " + Name);
				Debug.Break();
			}
			
			m_BeginStateFunction(prvState,newState);
		}
		
		public void UpdateState(State currState, float deltaTime)
		{
			if( m_UpdateStateFunction == null )
			{
				Debug.LogError("Update state is null in " + Name);
				Debug.Break();
			}
			
			m_UpdateStateFunction(currState,deltaTime);
		}
		
		public void EndState(State endingState, State newState)
		{
			if( m_EndStateFunction != null )
			{
				m_EndStateFunction(endingState,newState);
			}
		}
	}
	// End State implementation
	// -------------------------------------------------------------------------
	
	
	private ArrayList 						m_AllTransitionFuns = new ArrayList();
	private Dictionary<string,StateImpl>	m_AllStates = new Dictionary<string,StateImpl>();
	private Dictionary<uint,StateImpl>		m_AllStatesID = new Dictionary<uint,StateImpl>();
	private StateImpl						m_CurrState = null;
	private uint							m_CurrId = 0;
	private bool 							m_CanChangeState = true;
	
	public StateMachine()
	{
	}
	
	// Optimization tip, Try to keep the state name as short as possible
	public State AddNewState(string name, BeginStateFun beginState, UpdateStateFun updateState, EndStateFun endState)
	{
		if( m_AllStates.ContainsKey(name) )
		{
			Debug.LogError("State named " + name + " already exist");
		}
		else
		{
			uint currID = m_CurrId;
			StateImpl newState = new StateImpl(name,currID,beginState, updateState, endState);
			m_AllStates[name] = newState;
			m_AllStatesID[currID] = newState;
			m_CurrId++;
			return newState;
		}
		return null;
	}
	
	public void AddNewTransition(TransitionFun transition)
	{
		m_AllTransitionFuns.Add(transition);
	}
		
	public void Update(float deltaTime)
	{
		if( CanChangeState == true )
		{
			int tranFunCount = m_AllTransitionFuns.Count;
			
			StateImpl newStateImpl = null;
			
			for( int i = 0 ; i < tranFunCount ; i++ )
			{
				State newState = ((m_AllTransitionFuns[i] as TransitionFun)(m_CurrState));
				
				if( newState != null )
				{
					if( newState.GetType() == typeof( StateImpl ) )
					{
						newStateImpl = (StateImpl)newState;
						break;
					}
				}
			}
			
			if( newStateImpl != null && m_CurrState != newStateImpl )
			{
				SetState(newStateImpl);
			}
		}
	
		if( m_CurrState != null )
		{
			m_CurrState.UpdateState(m_CurrState,deltaTime);
		}
	}
			
	public bool CanChangeState
	{
		get
		{
			return m_CanChangeState;
		}
		set
		{
			m_CanChangeState = value;
		}
	}
	
	public uint CurrentState
	{
		get
		{
			return m_CurrState.ID;
		}
		set
		{
			StateImpl newState = null;
			
			if( m_AllStatesID.ContainsKey(value) )
			{
				newState = m_AllStatesID[value];
			}
			
			if( newState != m_CurrState && CanChangeState == true )
			{
				SetState(newState);
			}
		}
	}
	
	private void SetState(StateImpl state)
	{
		if( m_CurrState != null )
		{
			m_CurrState.EndState(m_CurrState,state);
		}
		
		if( state != null )
		{
			state.BeginState(m_CurrState,state);
		}
		
		m_CurrState = state;
	}
}

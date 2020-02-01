using UnityEngine;
using System.Collections;

public class SimpleTimerArray
{
	private enum TimerState
	{
		eTS_Running,
		eTS_Paused,
		eTS_Stopped,
	}
	
	private float[]			m_Timers = null;
	private TimerState[]	m_TimerState = null;
		
	public SimpleTimerArray(uint timerCount)
	{
		m_Timers = new float[timerCount];
		m_TimerState = new TimerState[timerCount];
		ResetAllTimers();
	}
	
	public void ResetAllTimers()
	{
		for( int i = 0 ; i < m_Timers.Length ; i++ )
		{
			m_Timers[i] = 0.0f;
			m_TimerState[i] = TimerState.eTS_Stopped;
		}
	}
	
	public bool IsTimeEnded(uint timerIndex)
	{
		if( m_Timers[timerIndex] <= 0.0f )
		{
			return true;
		}
		return false;
	}
	
	public bool IsTimerRunning(uint timerIndex)
	{
		if( m_TimerState[timerIndex] == TimerState.eTS_Running )
		{
			return true;
		}
		
		return false;
	}
	
	public bool IsTimerPaused(uint timerIndex)
	{
		if( m_TimerState[timerIndex] == TimerState.eTS_Paused )
		{
			return true;
		}
		
		return false;
	}
	
	public bool IsTimerStopped(uint timerIndex)
	{
		if( m_TimerState[timerIndex] == TimerState.eTS_Stopped )
		{
			return true;
		}
		
		return false;
	}
	
	public void PauseTimer(uint timerIndex)
	{
		m_TimerState[timerIndex] = TimerState.eTS_Paused;
	}
	
	public void ResumeTimer(uint timerIndex)
	{
		m_TimerState[timerIndex] = TimerState.eTS_Running;
	}
	
	public void ResetTimer(uint timerIndex)
	{
		m_Timers[timerIndex] = 0.0f;
		m_TimerState[timerIndex] = TimerState.eTS_Stopped;
	}
	
	public void StartTimer(uint timerIndex, float time)
	{
		m_Timers[timerIndex] = time;
		m_TimerState[timerIndex] = TimerState.eTS_Running;
	}
	
	public void Update(float deltaTime)
	{
		for( int i = 0 ; i < m_Timers.Length ; i++ )
		{
			if( m_TimerState[i] == TimerState.eTS_Running )
			{
				m_Timers[i] -= deltaTime;
				
				if( m_Timers[i] <= 0.0f )
				{
					m_Timers[i] = 0.0f;
				}
			}
		}
	}
}

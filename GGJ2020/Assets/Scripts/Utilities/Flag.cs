using UnityEngine;
using System;
using System.Collections;

public class Flag
{
	private uint			m_Flag = 0;
	
	public Flag()
	{
	}
	
	public void SetFlag(uint flag)
	{
		m_Flag = m_Flag | flag;
	}
	
	public void ClearFlag(uint flag)
	{
		m_Flag = m_Flag & ~flag;
	}
	
	public bool CheckFlag(uint flag)
	{
		return (m_Flag & flag) == flag;
	}
	
	public void Reset()
	{
		m_Flag = 0;
	}
}

using UnityEngine;
using System.Collections;

public class Weapon : Attacker 
{
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
		if( Enabled == true )
		{
 			DebugDraw.DrawBound(GetComponent<Collider>().bounds,Color.blue);
		}
	}
}

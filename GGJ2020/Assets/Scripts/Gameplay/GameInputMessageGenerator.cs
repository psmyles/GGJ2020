using UnityEngine;
using System.Collections;

public class GameInputMessageGenerator : MessageGenerator 
{
	private static GameInputMessageGenerator m_MessageGenerator = null;
	
	protected void Awake()
	{
		if( m_MessageGenerator != null )
		{
			Debug.LogError("Only one instance of 'GameInputMessageGenerator' can be created");
			Debug.Break();
		}
		m_MessageGenerator = this;
	}
		
	// Use this for initialization
	protected override void Start () 
	{
	
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		float vertValue = Input.GetAxisRaw("Vertical");
		float horzValue = Input.GetAxisRaw("Horizontal");

        float dashVertValue = Input.GetAxisRaw("DashVertical");
        float dashHorzValue = Input.GetAxisRaw("DashHorizontal");

        //Debug.Log("Left = " + vertValue + ", Forward = " + horzValue ); 

        const float epsilon = 0.0001f;

        //Debug.Log("Vertical : " + vertValue + ", HorzValue : " + horzValue);
		
		if( Mathf.Abs(vertValue) > epsilon )
		{
            if (vertValue < 0.0f)
            {
                SendMessageToListeners("MoveBackward", Mathf.Abs(vertValue));
            }
            else
            {
                SendMessageToListeners("MoveForward", Mathf.Abs(vertValue));
            }
			//Debug.Log("Send mssg 1");
		}
		if( Mathf.Abs(horzValue) > epsilon )
		{
			//Debug.Log("Send mssg 2");
			if( horzValue < 0.0f )
			{
				SendMessageToListeners("MoveLeft", Mathf.Abs(horzValue));
			}
			else
			{
				SendMessageToListeners("MoveRight", Mathf.Abs(horzValue));
			}
		}

        if (Mathf.Abs(dashVertValue) > epsilon)
        {
            if (dashVertValue < 0.0f)
            {
                SendMessageToListeners("DashBackward", Mathf.Abs(dashVertValue));
            }
            else
            {
                SendMessageToListeners("DashForward", Mathf.Abs(dashVertValue));
            }
            //Debug.Log("Send mssg 1");
        }
        if (Mathf.Abs(dashHorzValue) > epsilon)
        {
            //Debug.Log("Send mssg 2");
            if (dashHorzValue < 0.0f)
            {
                SendMessageToListeners("DashLeft", Mathf.Abs(dashHorzValue));
            }
            else
            {
                SendMessageToListeners("DashRight", Mathf.Abs(dashHorzValue));
            }
        }
	}
	
	public override MessageGenerator Get()
	{
		return m_MessageGenerator;
	}

    public void OnInputMove(Vector2 val)
    {
        
    }
}

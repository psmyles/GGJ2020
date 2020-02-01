using UnityEngine;
using System.Collections;

public class DebugDraw 
{
	public static void DrawCircle(Vector3 center, float radius, Vector3 upAxis, uint numberOfSubdivisions , Color color)
	{
		Quaternion rotate = Quaternion.FromToRotation(Vector3.up,upAxis);
		
		
		float angleInc = Mathf.PI * 2.0f / numberOfSubdivisions;
		
		Vector3 prvPos = new Vector3(radius,0.0f,0.0f);
		prvPos = rotate * prvPos + center;
		Vector3 startPos = prvPos;
		
		for( float currAngle = angleInc ; currAngle < 2.0f * Mathf.PI ; currAngle += angleInc )
		{
			float x = radius * Mathf.Cos(currAngle);
			float z = radius * Mathf.Sin(currAngle);
			
			Vector3 newPos = new Vector3(x,0.0f,z);
			newPos = rotate * newPos + center;
			
			Debug.DrawLine(prvPos,newPos,color);
			prvPos = newPos;
		}
		Debug.DrawLine(prvPos,startPos,color);
	}
	
	public static void DrawBound(Bounds bound, Color color)
	{
		Vector3 backBottomLeft = new Vector3(-bound.extents.x,-bound.extents.y,-bound.extents.z) + bound.center;
		Vector3 backTopLeft = new Vector3(-bound.extents.x,bound.extents.y,-bound.extents.z) + bound.center;
		Vector3 backBottomRight = new Vector3(bound.extents.x,-bound.extents.y,-bound.extents.z) + bound.center;
		Vector3 backTopRight = new Vector3(bound.extents.x,bound.extents.y,-bound.extents.z) + bound.center;
		
		Vector3 frontBottomLeft = new Vector3(-bound.extents.x,-bound.extents.y,bound.extents.z) + bound.center;
		Vector3 frontTopLeft = new Vector3(-bound.extents.x,bound.extents.y,bound.extents.z) + bound.center;
		Vector3 frontBottomRight = new Vector3(bound.extents.x,-bound.extents.y,bound.extents.z) + bound.center;
		Vector3 frontTopRight = new Vector3(bound.extents.x,bound.extents.y,bound.extents.z) + bound.center;
		
		Debug.DrawLine(backBottomLeft,backTopLeft,color);
		Debug.DrawLine(backTopLeft,backTopRight,color);
		Debug.DrawLine(backTopRight,backBottomRight,color);
		Debug.DrawLine(backBottomRight,backBottomLeft,color);
		
		Debug.DrawLine(frontBottomLeft,frontTopLeft,color);
		Debug.DrawLine(frontTopLeft,frontTopRight,color);
		Debug.DrawLine(frontTopRight,frontBottomRight,color);
		Debug.DrawLine(frontBottomRight,frontBottomLeft,color);
		
		Debug.DrawLine(backBottomLeft,frontBottomLeft,color);
		Debug.DrawLine(backTopLeft,frontTopLeft,color);
		Debug.DrawLine(backBottomRight,frontBottomRight,color);
		Debug.DrawLine(backTopRight,frontTopRight,color);
	}
}

using UnityEngine;
using System.Collections;
using UnityEditor;

public class NewBehaviourScript
{
	[MenuItem("Assets/Create/AnimEventData")]
    public static void CreateAnimEventData()
    {
		//string path =;//.Split(char.Parse("/"));
		//Debug.Log("App path " + path);
        AnimEventData newAnimEventData = ScriptableObject.CreateInstance<AnimEventData>();
        AssetDatabase.CreateAsset(newAnimEventData, "Assets/AnimEventData/AnimEventData.asset");
		//AssetDatabase.SaveAssets();
        Selection.activeObject = newAnimEventData; 
    }
	
	[MenuItem("Assets/Create/PlayerAnimEventData")]
    public static void CreatePlayerAnimEventData()
    {
        PlayerAnimEventData newAnimEventData = ScriptableObject.CreateInstance<PlayerAnimEventData>();// new PlayerAnimEventData();  //scriptable object
        AssetDatabase.CreateAsset(newAnimEventData, "Assets/AnimEventData/PlayerAnimEventData.asset");
		//AssetDatabase.SaveAssets();
        Selection.activeObject = newAnimEventData; 
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelObjectMap : MonoBehaviour {
	
	[System.Serializable]
	public class ObjectPair 
	{
		public int objectID;
		public LevelObject obj;
	}
	private static LevelObjectMap instance;
	public static LevelObjectMap Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(LevelObjectMap)) as LevelObjectMap;

			return instance;
		}
	}

	public ObjectPair[] objectMap;
	private Dictionary<int, LevelObject> objectDict;

	public void Awake() 
	{
		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}

		objectDict = new Dictionary<int, LevelObject>();

		for(int i = 0; i < objectMap.Length; i++) 
		{
			objectDict.Add(objectMap[i].objectID, objectMap[i].obj);
		}

	}

	public LevelObject GetPrefab(int objId)
	{
		if (!objectDict.ContainsKey(objId))
		{
			Debug.LogError("LevelObjectMap.GetPrefab Error: objectDict does not contain key: " + objId);
			return null;
		}
		
		return objectDict[objId];
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System;

public class LevelLoader : MonoBehaviour {

	public string currentLevelName;
	public int currentLevelId;
	private static LevelLoader instance;
	public static LevelLoader Instance
	{
		get 
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(LevelLoader)) as LevelLoader;

			return instance;
		}
	}

	public event Action<Dictionary<int, List<LevelObject>>> OnLevelLoaded;
	public Dictionary<int, List<LevelObject>> objectLists;
	public bool sceneLoaded = false;

	void Awake () 
	{
		objectLists = new Dictionary<int, List<LevelObject>>();

		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
	{
		if(scene.name == "LevelLoader")
		{
			StartCoroutine(LoadLevelObjects());
			sceneLoaded = true;
		}
	}

	public void LoadLevel(string levelName)
	{
		currentLevelName = levelName;
		if (!sceneLoaded)
			SceneManager.LoadScene("LevelLoader");
		else
		{
			foreach (var levelObjectList in objectLists)
			{
				for (int i = 0; i < levelObjectList.Value.Count; i++)
				{
					var obj = levelObjectList.Value[i];
					if(obj != null) 
					{
						Destroy(obj.gameObject);
					}
				}
			}
			objectLists.Clear();
			StartCoroutine(LoadLevelObjects());
		}

	}

	IEnumerator LoadLevelObjects()
	{
		var path = Application.streamingAssetsPath + "/Levels/" + currentLevelName + ".txt";;
		HelperFunctions.Load(path, id => 
		{
			var prefab = LevelObjectMap.Instance.GetPrefab(id);

			var obj = Instantiate(prefab);

			//insert elements into dictionary indexed by objectID
			if(objectLists.ContainsKey(obj.objectID) == false) 
			{
				objectLists.Add(obj.objectID, new List<LevelObject>());
			}
			//insert each object into its approriate list
			objectLists[obj.objectID].Add(obj);

			return obj;

		});

		yield return 1f;
		yield return 1f;
		 
		if(OnLevelLoaded != null)
		{
			OnLevelLoaded(objectLists);
		}
	}
}

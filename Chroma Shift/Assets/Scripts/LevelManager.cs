using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DG.DemiLib;

public class LevelManager : Photon.MonoBehaviour {

	[System.Serializable]
	public class Levels
	{
		public int id;
		public string name;
		public float[] rankTimes;
	}
	public Levels[] levels;
	public Dictionary<string,float> levelTimeDict;
	private int levelIndex;
	public float levelTimer;
	[SerializeField] Transform[] spawnPoints;
	public const int LEVEL_BOTTOM = -15;
	public const int LEVEL_TOP = 15;
	public SpawnPoint currentSpawnPoint;
	public SpawnPoint startingPoint;
	private int spawnPointIndex;

	public Hero[] heroes;
	public bool restart;
	public bool startTimer;
	public event Action<Hero> OnHeroSpawned;
	public event Action<Hero> OnHeroDeath;
	public delegate void RestartLevel();
	public delegate void LoadEvent();
	public event RestartLevel Restart;
	public event LoadEvent LoadTimes;
	public List<GameObject> droppedGear;


	private bool inMenu;

	private static LevelManager instance;
	public static LevelManager Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(LevelManager)) as LevelManager;

			return instance;
		}
	}
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
		droppedGear = new List<GameObject>();
		levelTimeDict = new Dictionary<string, float>();

	}
	private void Start()
	{
		LevelLoader.Instance.OnLevelLoaded += OnLevelLoaded;
		inMenu = true;
		LoadLevelTimes();
		LevelSelectScreen.Instance.SetLevelButtons();
		heroes = new Hero[HeroManager.Instance.numPlayers];
	}

	void OnLevelLoaded(Dictionary<int, List<LevelObject>> objectLists) 
	{
		inMenu = false;
		levelIndex = LevelLoader.Instance.currentLevelId;
		levelTimer = levels[levelIndex].rankTimes[3];
		currentSpawnPoint = null;

		var playerSpawners = objectLists[LevelObject.PLAYER_SPAWNER];
		foreach(SpawnPoint ps in playerSpawners)
		{
			if(currentSpawnPoint == null || currentSpawnPoint.transform.position.x > ps.transform.position.x)
			{
				currentSpawnPoint = ps;
				startingPoint = ps;
			}
		}
		currentSpawnPoint.PlayHeroEntry();
		//get the name of the colorwheels current gradient
		//colourWheelFaceColours[0].name;
		for(int i = 0; i < HeroManager.Instance.numPlayers; i++) 
		{
			GameObject go;

			if(heroes[i] == null)
			{

				go = Instantiate(HeroManager.Instance.GetCurrentHeroPrefab(i), currentSpawnPoint.transform.position, Quaternion.identity) as GameObject;
				heroes[i] = go.GetComponentInChildren<Hero>();

				heroes[i].playerID = i;
				heroes[i].colour.currentColourType = HeroManager.Instance.currentColorType[i];
				heroes[i].colour.shadeIndex = HeroManager.Instance.currentShadeIndex[i];
			}
			else 
			{
				heroes[i].transform.position = currentSpawnPoint.transform.position;
			}

			if(OnHeroSpawned != null) 
			{
				OnHeroSpawned(heroes[i]);
			}
			//PlayerUI.Instance.SetHero(hero);
			//TODO play player animation
			heroes[i].OnHeroSpawn();
		}
		ColourWheel.Instance.Shift();


		startTimer = true;
		//var h = go.GetComponent<Hero>();
	}

	public void NextLevel()
	{
		foreach (GameObject gear in droppedGear)
		{
			Destroy(gear);
		}
		droppedGear.Clear();
		CameraBehaviour.Instance.isSetup = false;
		SaveLevelTimes();
		levelIndex++;
		LevelLoader.Instance.currentLevelId = levelIndex;
		LevelLoader.Instance.LoadLevel(levels[levelIndex].name);

		for(int i = 0; i < HeroManager.Instance.numPlayers; i++) {
			if(heroes[i] == null)
				continue;
			
			transform.DOScale(Vector3.one, 1.0f).OnComplete(() => {
				PlayerUI.Instance.timerAnim.SetBool("finish", false);
				heroes[i].stats.currentHealth = heroes[i].stats.maxHealth;
				heroes[i].grounded = false;
				heroes[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			});
		}

	}
	public void FinishedLevel()
	{
		var levelName = levels[levelIndex].name;
		var levelTime = levels[levelIndex].rankTimes[3] - levelTimer;

		if(levelTimeDict.ContainsKey(levelName)) {
			if(levelTime < levelTimeDict[levelName])
				levelTimeDict[levelName] = levelTime;
		} else 
			levelTimeDict.Add(levelName, levelTime);

		//PlayerPrefs.SetFloat("Level_" + levels[levelIndex], levelTime);
		startTimer = false;
	}
	public void SaveLevelTimes()
	{
		var path = Application.streamingAssetsPath + "/LevelTimes/levelTimes.txt";

		var sb = new System.Text.StringBuilder();

		foreach(var level in levelTimeDict)
			sb.AppendLine(level.Key + "_" + level.Value);
		
		System.IO.File.WriteAllText(path, sb.ToString());
	}

	public void LoadLevelTimes()
	{
		var path = Application.streamingAssetsPath + "/LevelTimes/levelTimes.txt";

		levelTimeDict.Clear();

		var file = System.IO.File.ReadAllLines(path);
		foreach(var line in file)
		{
			if (string.IsNullOrEmpty(line)) continue;
			var data = line.Split('_');
			levelTimeDict.Add(data[0], float.Parse(data[1]));
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (inMenu){
			return;
		}
		if (startTimer)
			levelTimer -= Time.deltaTime;

		if (levelTimer < 10.0f)
			PlayerUI.Instance.TimerFlash();
		
		if (levelTimer < 0.0f)
		{
			if (Restart != null)
			{
				currentSpawnPoint = startingPoint;
				currentSpawnPoint.PlayHeroEntry();
				ColourWheel.Instance.Shift();
				Restart();
				levelTimer = GetLevelTime();
			}
		}
		if (restart && Restart != null)
		{

			currentSpawnPoint = startingPoint;
			currentSpawnPoint.PlayHeroEntry();
			ColourWheel.Instance.Shift();
			Restart();
			levelTimer = GetLevelTime();
			restart = false;
		}
	}
	public float GetLevelTime()
	{
		return levels[levelIndex].rankTimes[3];
	}
	public Hero GetAliveHero()
	{
		foreach(Hero h in heroes)
		{
			if (h.alive)
			{
				return h;
			}
		}
		return null;
	}
	public void InvokeHeroDeath(Hero hero)
	{
		if (OnHeroDeath != null)
		{
			OnHeroDeath(hero);
		}
	}
}

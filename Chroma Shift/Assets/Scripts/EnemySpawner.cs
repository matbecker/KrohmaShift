using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using DG.DemiLib;
using DG.Tweening;

public class EnemySpawner : LevelObject, IProjectileIgnore {

	[SerializeField] int minEnemies;
	[SerializeField] int maxEnemies;
	public static List<Enemy> enemyWave;
	[SerializeField] GameObject[] barriers;
	public bool hasWaveStarted;
	private bool isWaveOver;
	private bool startLerp;
	private float startLerpTimer;
	private float endLerpTimer;
	[SerializeField] float lerpDuration;
	[SerializeField] int spawnCircleWidth;
	[SerializeField] int spawnCircleHeight;
	[SerializeField] Enemy[] enemyTypes;
	[SerializeField] GameObject spawnerManager;
	[SerializeField] BoxCollider2D trigger;
	[SerializeField] ParticleSystem[] enemyParticles;
	private Hero h;

	// Use this for initialization
	void Awake()
	{
		if (!inEditor)
		{
			spawnerManager = GameObject.FindGameObjectWithTag("EnemySpawnerManager");
		}
			
	}
	void Start () 
	{
		hasWaveStarted = false;

		enemyWave = new List<Enemy>();
		playersEntered = new HashSet<Hero>();

		barriers[0].GetComponent<SpriteRenderer>().color = Color.clear;
		barriers[0].GetComponent<BoxCollider2D>().enabled = false;


		if (!inEditor)
			transform.parent = spawnerManager.transform;

		LevelManager.Instance.Restart += RestartLevel;
	}

	void OnDestroy() 
	{
		if (LevelManager.Instance)
			LevelManager.Instance.Restart -= RestartLevel;
	}

	private HashSet<Hero> playersEntered;
	void OnTriggerEnter2D(Collider2D other)
	{
		var hero = other.GetComponent<Hero>();
		if (hero != null && !hasWaveStarted)
		{
			if(!playersEntered.Contains(hero))
				playersEntered.Add(hero);
			else
				return;
			
			if(playersEntered.Count < HeroManager.Instance.numPlayers)
				return;
			
			h = hero;
			for (int i = 0; i < barriers.Length; i++)
			{
				var s = barriers[i].GetComponent<SpriteRenderer>();
				s.DOColor(Color.black, 2.0f);
				barriers[i].GetComponent<BoxCollider2D>().enabled = true;
				var t = barriers [i].transform;
				t.DOScale (new Vector3 (t.localScale.x, t.localScale.y * 1.5f, t.localScale.z), 0.5f);
			}
			PlayerUI.Instance.currentEnemiesBottom.DOColor(Color.black, 2.0f);
			PlayerUI.Instance.currentEnemiesTop.DOColor(Color.white, 2.0f);

			//get a random number of enemies to spawn based on the min and max provided
			int enemyCount = Random.Range(minEnemies, maxEnemies);

			//loop through as many times as the enemy count returned
			for (int i = 0; i <= enemyCount; i++)
			{
				//index
				int enemyType = Random.Range(0,enemyTypes.Length);

				//instantiate a random enemy type at the enemyspawner location
				Enemy enemyObj = Instantiate(enemyTypes[enemyType], transform.position + new Vector3(Random.insideUnitCircle.x * spawnCircleWidth, spawnCircleHeight, transform.position.z), Quaternion.identity) as Enemy;
				//disable box collider
				enemyObj.isSpawning = true;
				enemyObj.DisableColliders();
				enemyObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
				enemyObj.sprite.gameObject.transform.localScale = Vector3.zero;
				enemyObj.sprite.transform.DOScale(Vector3.one, 1.0f).OnComplete(() => 
				{
					enemyObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
					enemyObj.EnableColliders();
					enemyObj.isSpawning = false;
				});
				//add the enemy to the wave list
				enemyWave.Add(enemyObj);
			}
			hasWaveStarted = true;
			//cannot pause while inside a wave
			PauseOverlay.Instance.canPause = false;
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();
		if(hero != null && playersEntered.Contains(hero))
			playersEntered.Remove(hero);
	}
	void Update()
	{
		if (hasWaveStarted)
		{
			Vector3 dist = transform.position - h.transform.position;
			
			PlayerUI.Instance.currentEnemiesBottom.text = enemyWave.Count.ToString();
			PlayerUI.Instance.currentEnemiesTop.text = enemyWave.Count.ToString();
		}
		if (hasWaveStarted && enemyWave.Count == 0)
		{
			for (int i = 0; i < barriers.Length; i++)
			{
				//barriers[i].GetComponent<SpriteRenderer>().DOColor(Color.clear, 2.0f);
				var s = barriers[i].GetComponent<SpriteRenderer>();
				s.DOColor(Color.black, 2.0f);
				barriers[i].GetComponent<BoxCollider2D>().enabled = false;
			}
			PlayerUI.Instance.currentEnemiesBottom.DOColor(Color.clear, 2.0f);
			PlayerUI.Instance.currentEnemiesTop.DOColor(Color.clear, 2.0f);

			float rand = Random.value;

			if (rand > 0.5f)
				ColourWheel.Instance.Shift();

			gameObject.SetActive(false);
			hasWaveStarted = false;
			PauseOverlay.Instance.canPause = true;
		} 
	}
	public static void ClearEnemies()
	{
		if(enemyWave != null) 
		{
			foreach (Enemy enemy in enemyWave)
			{
				Destroy(enemy.gameObject);
			}
			enemyWave.Clear();
		}
	}

	public override string GetSaveString ()
	{
		//int differentEnemyTypes = enemyTypes.Count;

		var saveString = string.Join(SPLIT_CHAR.ToString(), new []{objectID, transform.position.x, transform.position.y, transform.position.z, 
														barriers[0].transform.position.x, barriers[0].transform.position.y, barriers[0].transform.position.z,
														barriers[1].transform.position.x, barriers[1].transform.position.y, barriers[1].transform.position.z,
														minEnemies, maxEnemies, spawnCircleWidth, spawnCircleHeight, barriers[0].transform.localScale.y,
				/*enemyTypes[0]*/}.Select(s => s.ToString()).ToArray());

		if(enemyTypes == null || enemyTypes.Length == 0)
		{
			saveString += SPLIT_CHAR.ToString() + "0";

		} else {
			saveString += SPLIT_CHAR.ToString() + enemyTypes.Length;
			for(int i = 0; i < enemyTypes.Length; i++)
			{
				saveString += SPLIT_CHAR.ToString() + enemyTypes[i].objectID;
			}
		}
		saveString += SPLIT_CHAR.ToString() + trigger.size.x;
		return saveString;
	}

	public override void LoadSaveData (string input)
	{
		var data = input.Split(SPLIT_CHAR);

		transform.position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
		barriers[0].transform.position = new Vector3(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]));
		barriers[1].transform.position = new Vector3(float.Parse(data[7]), float.Parse(data[8]), float.Parse(data[9]));
		minEnemies = int.Parse(data[10]);
		maxEnemies = int.Parse(data[11]);
		spawnCircleWidth = int.Parse(data[12]);
		spawnCircleHeight = int.Parse(data[13]);
		barriers[0].transform.localScale = new Vector3(barriers[0].transform.localScale.x, float.Parse(data[14]), barriers[0].transform.localScale.z);
		barriers[1].transform.localScale = new Vector3(barriers[1].transform.localScale.x, float.Parse(data[14]), barriers[1].transform.localScale.z);

		var enemys = new List<Enemy>();
		var num = int.Parse(data[15]);

		for(int i = 0; i < num; i++){
			var objectID = int.Parse(data[16 + i]);
			var enemyPrefab = (Enemy)LevelObjectMap.Instance.GetPrefab(objectID);
			enemys.Add(enemyPrefab);
		}

		var index = 16 + num;
		trigger.size = new Vector2(index < data.Length ? float.Parse(data[index]) : 10.0f, trigger.size.y);

		enemyTypes = enemys.ToArray();
		
	}
	public override Vector3 GetOffset ()
	{
		return Vector3.zero;
	}
	private void RestartLevel()
	{
		ClearEnemies();

		barriers[0].GetComponent<SpriteRenderer>().color = Color.clear;
		barriers[0].GetComponent<BoxCollider2D>().enabled = false;
		gameObject.SetActive(true);
	}
}

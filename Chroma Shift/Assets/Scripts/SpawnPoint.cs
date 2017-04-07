using UnityEngine;
using System.Collections;
using System.Linq;

public class SpawnPoint : LevelObject, IProjectileIgnore {

	[SerializeField] GameObject spawnerManager;
	[SerializeField] Animator anim;
	public bool finishPoint;
	private bool jumpPushed;
	// Use this for initialization
	void Awake()
	{
		if (!inEditor)
			spawnerManager = GameObject.FindGameObjectWithTag("SpawnerManager");
	}
	void Start () 
	{
		if (!inEditor)
			transform.parent = spawnerManager.transform;

		anim.SetBool("editor", inEditor);
	}

	public void PlayHeroEntry() 
	{
		anim.SetTrigger("heroEntry");
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		var hero = other.GetComponent<Hero>();

		if (hero != null)
		{
			hero.stats.currentHealth = hero.stats.maxHealth;
			PlayerUI.Instance.SetHealthBarColour(hero, Color.black, true);

			if (finishPoint)
			{
				anim.SetBool("heroAtEnd", true);
				LevelManager.Instance.FinishedLevel();
				PlayerUI.Instance.TimerStop();
				return;
			}

			if (hero.GetComponent<Rigidbody2D>().gravityScale == 0.0f)
			{
				PlayHeroEntry();
			}

			anim.SetBool("heroInTrigger", true);

			LevelManager.Instance.currentSpawnPoint = this;

			foreach (Hero h in LevelManager.Instance.heroes)
			{
				if (h != null)
				{
					if (!h.alive)
					{
						h.Respawn();
					}
				}

			}
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		var hero = other.GetComponent<Hero>();

		if (hero != null)
		{
			if (finishPoint)
			{
				anim.SetBool("heroAtEnd", false);
				return;
			}

			anim.SetBool("heroInTrigger", false);	

			hero.isSpawning = false;
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			if (finishPoint)
			{
				if (Input.GetButtonDown("Jump") && !jumpPushed)
				{
					other.GetComponent<Animator>().SetTrigger("onHeroExit");
					jumpPushed = true;
				}
			}
		}
	}
	public override string GetSaveString ()
	{
		return string.Join(SPLIT_CHAR.ToString(), new []{objectID, transform.position.x, transform.position.y, transform.position.z, finishPoint ? 1 : 0}.Select(s => s.ToString()).ToArray());
	}
	public override void LoadSaveData (string input)
	{
		var data = input.Split(SPLIT_CHAR);

		transform.position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));

		var fp = int.Parse(data[4]);

		finishPoint = (fp == 1) ? true : false;
	}
	public void RespawnPlayer()
	{
		anim.SetTrigger("heroEntry");
	}
	void OnDrawGizmos()
	{
		if (inEditor)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(bc.bounds.center, bc.bounds.size);
		}
	}
}

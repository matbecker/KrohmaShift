﻿using UnityEngine;
using System.Collections;
using DG.DemiLib;
using DG.Tweening;

public class Enemy : LevelObject, IDamageable {

	public enum EnemyType {Buzzer, Bomber, Bouncer };
	public EnemyType type;

	public static int PURPLE_INDEX = 0;
	public static int BLUE_INDEX = 1;
	public static int GREEN_INDEX = 2;
	public static int YELLOW_INDEX = 3;
	public static int ORANGE_INDEX = 4;
	public static int RED_INDEX = 5;

	[System.Serializable]
	public class Stats
	{
		public int health;
		public int attackPower;
		public int movementSpeed;
	}

	public ColourManager colour;
	[SerializeField] GameObject[] powerUps;
	[SerializeField] protected ParticleSystem[] enemyParticles;
	[SerializeField] protected ParticleSystem fx;
	[SerializeField] protected Stats stats;
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] protected BoxCollider2D col;
	[SerializeField] protected SpriteRenderer sprite;
	[SerializeField] protected GameObject target;
	protected Vector2 direction;
	[SerializeField] protected float distance;

	protected virtual void Awake()
	{
		target = GameObject.FindGameObjectWithTag("Player");

		//disable particle systems
		foreach (ParticleSystem ps in enemyParticles)
		{
			ps.gameObject.SetActive(false);
		}
	}
	// Use this for initialization
	protected virtual void Start () 
	{
		rb = gameObject.GetComponent<Rigidbody2D>();
		//colour = gameObject.GetComponent<ColourManager>();
		//top or bottom colour
		int rand = Random.Range(0,2);
		//each shade
		int randShade = Random.Range(0,6);

		if (rand == 0)
		{
			var indexT = (int)ColourWheel.Instance.currentColourTop;
			var topColour = colour.colors[indexT].color[randShade];
			enemyParticles[indexT].gameObject.SetActive(true);
			fx = enemyParticles[indexT];
			topColour.a = 1;
			sprite.color = topColour;
			colour.currentColourType = (ColourManager.ColourType)ColourWheel.Instance.currentColourTop;

		}
		else
		{
			var indexB = (int)ColourWheel.Instance.currentColourBottom;
			var bottomColour = colour.colors[indexB].color[randShade];
			enemyParticles[indexB].gameObject.SetActive(true);
			fx = enemyParticles[indexB];
			bottomColour.a = 1;
			sprite.color = bottomColour;
			colour.currentColourType = (ColourManager.ColourType)ColourWheel.Instance.currentColourBottom;
		}
			
	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		if (transform.position.y < LevelManager.LEVEL_BOTTOM)
			Death();

		distance = (target.transform.position - transform.position).magnitude;

		if (distance > 20)
			Death();
	}
	protected virtual void FixedUpdate(){}

	protected virtual void OnCollisionEnter2D(Collision2D other)
	{
		var damage = stats.attackPower;
		var hero = other.gameObject.GetComponent<Hero>();

		if (hero != null)
		{
			if (HelperFunctions.IsContrastingColour(colour.currentColourType, hero.colour.currentColourType))
				damage = 10;

			hero.Damage(damage);
		}
	}
	protected virtual void Death()
	{
		int rand = Random.Range(0,5);

		if (rand == 0)
		{
			GameObject powerUp = Instantiate(powerUps[0], col.bounds.center, Quaternion.identity) as GameObject;
		}
		EnemySpawner.enemyWave.Remove(this);
	}
	public virtual void Damage(int damageAmount)
	{
		stats.health -= damageAmount;

		if (stats.health <= 0)
		{
			Death();
		}
	}
	protected virtual void Move(){}

	protected virtual void SetSize(Vector3 sizeValue, float duration)
	{
		transform.DOScale(sizeValue, duration).SetEase(Ease.Linear);
	}
}

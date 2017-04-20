using UnityEngine;
using System.Collections;
using DG.DemiLib;
using DG.Tweening;

public class Bomber : Enemy {

	private bool grounded;
	protected override void Awake()
	{
		base.Awake();
		//low health
		stats.health = 1;
		//medium attack
		stats.attackPower = Random.Range(1,4);
		//fast movement
		stats.movementSpeed = Random.Range(4,7);
		desiredSize = new Vector3 (1.0f, 0.75f, 1.0f);
	}
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();

		type = EnemyType.Bomber;

	}
	protected override void Update()
	{
		base.Update ();
	}	
	protected override void FixedUpdate()
	{
		//start moving when on the ground
		if (grounded)
			Move();
	}
	protected override void Move()
	{
		direction = target.transform.position - transform.position;
		direction.Normalize();
		direction *= stats.movementSpeed;
		rb.velocity = new Vector2(direction.x, rb.velocity.y);
	}
	protected override void OnCollisionEnter2D(Collision2D other)
	{
		var ground = other.gameObject.GetComponent<GroundBlock>();
		var hero = other.gameObject.GetComponent<Hero>();
		var enemy = other.gameObject.GetComponent<Enemy>();

		//bomber touched the ground
		if (ground != null)
		{
			grounded = true;
		}
		if (hero != null)
		{
			var force = (hero.transform.position.x < transform.position.x) ? Vector2.left * 100.0f : Vector2.right * 100.0f;
			hero.GetComponent<Rigidbody2D>().AddForce(force);

			transform.DOScale(Vector3.one * 2f, 0.2f).OnComplete(() => 
			{
				Death();
			});
		}
		if(other.gameObject.CompareTag("Barricade"))
		{
			transform.DOScale(Vector3.one * 2f, 0.2f).OnComplete(() => 
			{
				Death();
			});
		}
		base.OnCollisionEnter2D(other);

	}
	protected override void Death ()
	{
		fx.Play();
		fx.transform.parent = null;
		Destroy(gameObject);
		base.Death ();
	}
}

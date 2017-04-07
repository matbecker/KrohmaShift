using UnityEngine;
using System.Collections;

public class Buzzer : Enemy {

	public enum State { Move, Swoop, Avoid };
	public State state;

	private float timer;

	protected override void Awake ()
	{
		base.Awake ();
		//medium health
		stats.health = Random.Range(1,3);
		//weak attack
		stats.attackPower = Random.Range(1,3);
		//medium speed
		stats.movementSpeed = Random.Range(2,4);
	}
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();

		type = EnemyType.Buzzer;

		state = State.Move;
	}
	protected override void Update ()
	{
		base.Update ();
	}
	protected override void FixedUpdate ()
	{
		switch (state)
		{
		case State.Move:
			Move();
			break;
		case State.Swoop:
			Swoop();
			break;
		case State.Avoid:
			Avoid();
			break;
		default:
			break;
		}
	}
	protected override void Move ()
	{
		if (distance > 2)
		{
			state = State.Swoop;
			return;
		}

		direction = target.transform.position - transform.position;
		direction.Normalize();
		direction *= stats.movementSpeed;
		rb.velocity = new Vector2(direction.x, direction.y);


	}
	private void Swoop()
	{
		transform.right = target.transform.position - transform.position;

		timer += Time.deltaTime;

		if (timer > 2.0f)
		{
			direction = transform.right;

			direction.Normalize();

			direction *= (stats.movementSpeed * 1.25f);

			rb.velocity = direction;
		}
	}
	private void Avoid()
	{
		timer += Time.deltaTime;

		if (timer > 2.0f)
		{
			state = State.Move;
			return;
		}

		direction = transform.position - target.transform.position;
		direction.Normalize();
		direction *= stats.movementSpeed;
		rb.velocity = new Vector2(direction.x, rb.velocity.y);
	}
	protected override void OnCollisionEnter2D(Collision2D other)
	{
		
		//if the hero lands on top of me
		if (other.collider.CompareTag("Player") && other.transform.position.y > transform.position.y + sprite.sprite.bounds.extents.y)
		{
			other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 200);
			Damage(2);
			return;
		}
		base.OnCollisionEnter2D(other);
//		//if i swoop at the hero 
//		if (other.collider.CompareTag("Player") && other.transform.position.y < transform.position.y)
//		{				
//			other.gameObject.SendMessage("Damage", stats.attackPower, SendMessageOptions.DontRequireReceiver);
//
//			timer = 0.0f;
//
//			state = State.Avoid;
//			}
//		}
	}
}

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Buzzer : Enemy {

	public enum State { None, Move, Swoop, Avoid };
	public State state;
	[SerializeField] Transform childTransform;
	[SerializeField] float frequency;
	[SerializeField] float amplitude;
	[SerializeField] float movementSpeedRangeMin;
	[SerializeField] float movementSpeedRangeMax;

	private float timer;

	protected override void Awake ()
	{
		base.Awake ();
		//medium health
		stats.health = Random.Range(1,3);
		//weak attack
		stats.attackPower = Random.Range(1,3);
		//medium speed
		stats.movementSpeed = Random.Range(movementSpeedRangeMin,movementSpeedRangeMax);

		desiredSize = new Vector3 (1.0f, 0.25f, 1.0f);
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
		if (distance > 2.0f)
		{
			state = State.None;
			//childTransform.GetComponent<SpriteRenderer>().transform.DOScale(new Vector3 (childTransform.localScale.x, childTransform.localScale.y + 0.05f, childTransform.localScale.z), 0.25f).SetLoops (2, LoopType.Yoyo).OnComplete (() => 
			childTransform.DORotate(new Vector3(0.0f,0.0f,360.0f), 0.25f, RotateMode.FastBeyond360).OnComplete(() => 
			{
				state = State.Swoop;
				return;	
			});
		}
		direction = target.transform.position - transform.position;
		direction.Normalize();
		rb.velocity = new Vector2(direction.x * stats.movementSpeed, 0f);
		rb.position = new Vector2 (rb.position.x, amplitude * Mathf.Sin (Time.time * frequency));
		childTransform.eulerAngles = new Vector3 (0f, 0f, -Mathf.Rad2Deg * Mathf.Cos (Time.time * frequency));
	}

	private void Swoop()
	{
		transform.right = target.transform.position - transform.position;

		direction = transform.right;

		direction.Normalize ();

		direction *= (stats.movementSpeed * 1.25f);

		rb.velocity += (direction * Time.fixedDeltaTime);

	}
	private void Avoid()
	{
//		if (frequency < 20.0f && amplitude < 5.0f) 
//		{
//			frequency = 20.0f;
//			amplitude = 5.0f;
//			state = State.Move;
//			return;
//		}
		direction = transform.position - target.transform.position;
		direction.Normalize();
//		frequency = 30.0f;
//		frequency -= 0.1f;
//		amplitude = 30.0f;
//		amplitude -= 0.5f;
		rb.velocity = new Vector2(direction.x * stats.movementSpeed, 0f);
	}
	protected override void Death ()
	{
		base.Death ();
		Destroy (gameObject);
	}
	protected override void OnCollisionEnter2D(Collision2D other)
	{
		var hero = other.gameObject.GetComponent<Hero> ();
		//if the hero lands on top of me
		if (hero != null)
		{
			if (hero.transform.position.y > transform.position.y + sprite.sprite.bounds.extents.y) {
				other.gameObject.GetComponent<Rigidbody2D> ().AddForce (Vector2.up * 200);
				Death ();
				return;
			} 
			else 
			{
				base.OnCollisionEnter2D(other);
				state = State.Avoid;
			}
		}
	}
}

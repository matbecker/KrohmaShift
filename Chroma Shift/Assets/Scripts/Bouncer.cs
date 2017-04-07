using UnityEngine;
using System.Collections;
using DG.DemiLib;
using DG.Tweening;

public class Bouncer : Enemy {

	[SerializeField] Vector2 verticalForce;
	[SerializeField] Vector2 horizontalForce;
	[SerializeField] Vector2 velocity;
	[SerializeField] Vector2 maxVelocity;
	[SerializeField] EdgeCollider2D edgeCol;

	public bool grounded;
	private float airTimer;
	private bool facingRight;
	private float timer;
	private bool spinning;
	private bool isDropping;
	private float dropTimer;
	private RaycastHit2D rayLeft;
	private RaycastHit2D rayRight;

	protected override void Awake ()
	{
		base.Awake ();
		//lots of health
		stats.health = Random.Range(4,7);
		//strong attack
		stats.attackPower = Random.Range(4,8);
		//slow movement
		stats.movementSpeed = Random.Range(1,3);
	}
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		type = EnemyType.Bouncer;
		direction = (Random.value > 0.5f) ? Vector2.right : Vector2.left;
		SetBouncerSize();
	}
	protected override void Update ()
	{
		base.Update();

		var rayDown = Physics2D.Raycast((Vector2)edgeCol.bounds.center + edgeCol.offset, Vector2.down, 10.0f);

		if (rayDown.collider != null)
		{
			if (!spinning && rayDown.collider.gameObject.GetComponent<Hero>() != null)
			{
				rb.constraints = RigidbodyConstraints2D.FreezeAll;
				transform.DORotate(new Vector3(0.0f, 0.0f,720.0f), 0.3f,RotateMode.FastBeyond360).OnComplete(Drop);
				spinning = true;
			}
		}

		rayLeft = Physics2D.Raycast(new Vector2(transform.position.x - sprite.bounds.extents.x - 0.05f, transform.position.y), Vector2.left, 0.1f);
		rayRight = Physics2D.Raycast(new Vector2(transform.position.x + sprite.bounds.extents.x + 0.05f, transform.position.y), Vector2.right, 0.1f);

		if (rayLeft.collider != null && rayLeft.collider.gameObject.layer != 14)
		{
			direction *= -1;
		}
		if (rayRight.collider != null && rayRight.collider.gameObject.layer != 14)
		{
			direction *= -1;
		}
	}
	public override void Damage (int damageAmount)
	{
		Shrink(new Vector3(0.2f,0.2f,0.0f));
		base.Damage (damageAmount);
	}
	protected override void Death ()
	{
		base.Death ();
		transform.DORotate(new Vector3(0.0f,0.0f,180.0f), 0.5f, RotateMode.Fast);
		transform.DOScale(Vector3.zero,0.5f).SetEase(Ease.Linear).OnComplete(() => {
			Destroy(gameObject);
		});
	}
	private void SetBouncerSize()
	{
		var size = Vector3.one;

		switch (stats.health)
		{
		case 7:
			size = new Vector3(2.0f,2.0f,1.0f);
			break;
		case 6:
			size = new Vector3(1.8f,1.8f,1.0f);
			break;
		case 5:
			size = new Vector3(1.6f,1.6f,1.0f);
			break;
		case 4:
			size = new Vector3(1.4f,1.4f,1.0f);
			break;
		case 3:
			size = new Vector3(1.2f,1.2f,1.0f);
			break;
		case 2:
			size = Vector3.one;
			break;
		default:
			size = Vector3.one;
			break;
		}
		transform.localScale = size;
	}
	private void Shrink(Vector3 amount)
	{
		transform.DOScale(transform.localScale - amount, 0.5f);
	}
	// Update is called once per frame
	protected override void FixedUpdate () 
	{
		Move();

		if (velocity.x > maxVelocity.x)
			velocity.x = maxVelocity.x;
		else if (velocity.x < -maxVelocity.x)
			velocity.x = -maxVelocity.x;
			

		if (velocity.y > maxVelocity.y)
			velocity.y = maxVelocity.y;
		else if (velocity.y < -maxVelocity.y)
			velocity.y = -maxVelocity.y;

		velocity = rb.velocity;
	}

	Tween scaleTween;
	protected override void OnCollisionEnter2D(Collision2D other)
	{

	
		var hero = other.gameObject.GetComponent<Hero>();
		if  (hero != null && other.gameObject.transform.position.y > transform.position.y)
		{
			return;
		}
		base.OnCollisionEnter2D(other);

		if (rb.gravityScale == 10.0f)
		{
			rb.constraints = RigidbodyConstraints2D.FreezeAll;

			var weight = (stats.health > 4) ? 0.5f : 0.2f;
			CameraBehaviour.Instance.Shake(0.2f,0.0f,weight,false);

			transform.DOShakeScale(0.3f,0.5f,3,0.0f).OnComplete(() => {
				rb.constraints = RigidbodyConstraints2D.FreezeRotation;
				rb.gravityScale = 1.0f;
			});
			fx.Play();
		}
		if(other.gameObject.layer == HelperFunctions.collidableLayerId)
		{
			spinning = false;
			rb.gravityScale = 1;
			grounded = true;
			if(scaleTween == null)
			{
				scaleTween = sprite.transform.DOScale(sprite.transform.localScale * 1.4f, 0.15f)
						.SetLoops(2, LoopType.Yoyo)
						.OnComplete(() => {
							Jump();
							scaleTween = null;
					});
			}
		}
	}
	private void Jump() 
	{
		if (grounded)
		{
			rb.velocity = new Vector2(rb.velocity.x, 0.0f);
			rb.AddForce(verticalForce);
			grounded = false;
		}
	}
	private void Drop()
	{
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		rb.gravityScale = 10.0f;
	}
	protected override void Move ()
	{
		direction.Normalize();
		direction *= stats.movementSpeed;
		rb.velocity = new Vector2(direction.x, rb.velocity.y);
	}
}

using UnityEngine;
using System.Collections;
using DG.DemiLib;
using DG.Tweening;

public class Ninja : Hero {

	[SerializeField] private bool canDoubleJump;
	[SerializeField] GameObject dagger;
	[SerializeField] SpriteRenderer headband;
	[SerializeField] BoxCollider2D swordTrigger;
	[SerializeField] ParticleSystem ps;
	private float blockTimer;
	private bool freezeBlock;
	private bool onFire;
	private bool shake;
	private float shakeTimer;


	protected override void Start ()
	{	
		base.Start ();

		if (photonView.isMine)
			InputManager.Instance.DoubleJump += DoubleJump;
	}

	protected override void OnDestroy ()
	{
		base.OnDestroy ();

		if (InputManager.Instance)
			if (photonView.isMine)
				InputManager.Instance.DoubleJump -= DoubleJump;
	}
	

	protected override void Update ()
	{
		base.Update ();

		if (!startShieldTimer)
		{
			onFire = false;
			ps.Stop();
			sprite.DOColor(colour.GetCurrentColor(), 0.2f);
			headband.DOColor(Color.black, 0.2f);
			dagger.SetActive(true);
		}
		if (freezeBlock)
		{
			blockTimer += Time.deltaTime;
			stats.shieldCapacity = 4.0f;

			if (blockTimer > stats.shieldCapacity)
				freezeBlock = false;
		}
		else
		{
			stats.shieldCapacity = 0.2f;
		}

		swordTrigger.enabled = (isAttacking) ? true : false;
	}

	protected override void Attack ()
	{
		base.Attack ();
		disableInput = true;

		PlayAttackAnimation();
		//call the play attack animation over the network
		if (photonView.isMine)
			photonView.RPC("PlayAttackAnimation", PhotonTargets.Others);
	}
	//method for playing the attack animation
	[PunRPC] private void PlayAttackAnimation()
	{
		weaponAnim.SetBool("isAttacking", true);
	}

	[PunRPC] protected override void Block ()
	{
		base.Block ();

		//dont allow the player to reset the LerpTimer if they are currently blocking
		if (!freezeBlock && canBlock)
		{
			stats.shieldCapacity = 0.5f;
			ps.Play();
			sprite.DOColor(Color.clear, 0.2f);
			headband.DOColor(Color.clear, 0.2f);
			dagger.SetActive(false);
			onFire = true;
			freezeBlock = true;
			blockTimer = 0.0f;
		}
	}
	protected override void OnCollisionExit2D (Collision2D other)
	{
		base.OnCollisionExit2D(other);

		if (grounded)
			canDoubleJump = true;
	}
	protected override void OnCollisionEnter2D (Collision2D other)
	{
		base.OnCollisionEnter2D(other);
		canDoubleJump = false;

		var enemy = other.gameObject.GetComponent<Enemy>();

		if (enemy != null && onFire)
		{
			CameraBehaviour.Instance.Shake(0.2f,0.2f,0.2f,true);
			other.gameObject.SendMessage("Damage", 10, SendMessageOptions.DontRequireReceiver);
			onFire = false;
		}
	}
	private void DoubleJump(int playerID)
	{
		if (playerID != this.playerID)
			return;
		
		//if the player can double jump
		if (!grounded && canDoubleJump)
		{
			transform.DORotate(new Vector3(0.0f, 0.0f,360.0f), 0.5f,RotateMode.FastBeyond360);

			rb.velocity = new Vector2(rb.velocity.x, 0.0f);

			var force = (rb.gravityScale > 0) ? stats.jumpForce : -stats.jumpForce;
			//apply a force in the y direction once again
			rb.AddForce(force);
			//set the canDoubleJump variable to false to ensure the player cannot jump a third time
			canDoubleJump = false;
		}
	}
}

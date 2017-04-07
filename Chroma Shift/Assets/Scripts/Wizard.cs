using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;

public class Wizard : Hero {


	[SerializeField] private bool canHover;
	[SerializeField] private bool isHovering;
	[SerializeField] private float hoverTimer;
	[SerializeField] float desiredHoverDuration;
	[SerializeField] private bool isChargingShot;
	[SerializeField] private float chargingShotTimer;
	[SerializeField] ParticleSystem ps;
	[SerializeField] BoxCollider2D shieldCol;
	private bool freezeBlock;
	private Tween hoverTween;

	protected override void Start ()
	{
		base.Start ();

		if(photonView.isMine)
		{
			InputManager.Instance.Hover += Hover;
			InputManager.Instance.UnAttack += UnAttack;
		}
		freezeBlock = false;

	}

	protected override void OnDestroy ()
	{
		base.OnDestroy ();

		if (InputManager.Instance)
		{
			if(photonView.isMine)
			{
				InputManager.Instance.Hover -= Hover;
				InputManager.Instance.UnAttack -= UnAttack;
			}
		}
	}

	protected override void Update()
	{
		base.Update();

		//if the player is hovering start timing their hover
		if (isHovering)
			hoverTimer += Time.deltaTime;
		else
			hoverTimer = 0.0f;

		//if the timer reaches the hover duration
		if (hoverTimer >= desiredHoverDuration)
		{
			//call the StopHovering method
			StopHovering();
		}
		if (isChargingShot)
		{
			if (!tmpProjectile)
				return;
			
			chargingShotTimer += Time.deltaTime;

			//if the attackbutton has been held for under 1 second
			if (chargingShotTimer < 1.0f)
			{
				//increase attack power
				stats.attackPower = 2;

				//expand the size of the projectile
				tmpProjectile.GetComponent<Transform>().localScale = new Vector3(1.2f,1.2f,1.2f);
			}
			//if the attack button has been held bwteen 1 and 2 seconds
			else if (chargingShotTimer > 1.0f && chargingShotTimer < 2.0f)
			{
				//increase attack power
				stats.attackPower = 3;

				//expand the size of the projectile
				tmpProjectile.GetComponent<Transform>().localScale = new Vector3(1.4f,1.4f,1.4f);
			}
			//if the attack button has been held between 2 and 3 seconds
			else if (chargingShotTimer > 2.0f && chargingShotTimer < 3.0f)
			{
				//increase attack power
				stats.attackPower = 5;

				//expand the size of the projectile
				tmpProjectile.GetComponent<Transform>().localScale = new Vector3(1.6f,1.6f,1.6f);
			}
			//if the attack button has been held for over 3 seconds
			else if (chargingShotTimer > 3.0f)
			{
				//increase attack power
				stats.attackPower = 7;

				//expand the size of the projectile
				tmpProjectile.GetComponent<Transform>().localScale = new Vector3(1.8f,1.8f,1.8f);
			}
		}
		else
		{
			//reset the charge timer
			chargingShotTimer = 0.0f;
		}


		if (startShieldTimer)
		{
			shieldCol.enabled = true;
			ps.Play();
		}
		else
		{
			shieldCol.enabled = false;
			ps.Stop();
		}
			
		

		if (isHovering && Input.GetButtonUp("Jump"))
		{
			StopHovering();
		}
	}

	protected override void Attack ()
	{
		base.Attack ();

		Vector2 spawnProjectilePoint;

		//if the wizard is facing right spawn a projectile at the right side of their sprite
		if(facingRight)
			spawnProjectilePoint = new Vector3(transform.position.x + edgeCol.bounds.extents.x + projectile.GetComponent<SpriteRenderer>().sprite.bounds.extents.x, transform.position.y + 0.1f, transform.position.z);
		//if the player is facing left spawn a projectile at the left side of their sprite
		else 
			spawnProjectilePoint = new Vector3(transform.position.x - edgeCol.bounds.extents.x - projectile.GetComponent<SpriteRenderer>().sprite.bounds.extents.x, transform.position.y + 0.1f, transform.position.z);


		StartChargeShot(spawnProjectilePoint);

		//call teh start charge shot method over the network
		if(photonView.isMine)
			photonView.RPC("StartChargeShot", PhotonTargets.Others, spawnProjectilePoint);
	}
	//method for spawning the wizards projectile
	[PunRPC] void StartChargeShot(Vector2 spawnProjectilePoint)
	{
		//spawn the projectile
		tmpProjectile = Instantiate(projectile, spawnProjectilePoint, Quaternion.identity) as GameObject;
		//Freeze the y position of the projectile so it doesnt fall
		tmpProjectile.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;

		tmpProjectile.GetComponent<Projectile>().hero = this;
		//wizard is now charging their shot
		isChargingShot = true;
	}
	//method for shooting the wizards projectile
	[PunRPC] void ShootChargeShot(Vector2 velocity)
	{
		if(!isChargingShot)
			return;
		//set the projectiles velocity
		tmpProjectile.GetComponent<Rigidbody2D>().velocity = velocity;
		//wizard is no longer charging their shot
		isChargingShot = false;
	}

	private void UnAttack(int playerID)
	{
		//give the projectile a velocity in the x direction once the attack button is released
		var velocity = new Vector2(stats.attackSpeed, 0.0f);
		//negate the velocity if the player is facing left
		if(!facingRight) 
			velocity.x *= -1;
		
		ShootChargeShot(velocity);
		//call the shoot charge shot method over the network
		if(photonView.isMine)
			photonView.RPC("ShootChargeShot", PhotonTargets.Others, velocity);
	}

	[PunRPC] protected override void Block ()
	{
		base.Block ();
	}
	protected override void OnCollisionEnter2D (Collision2D other)
	{
		base.OnCollisionEnter2D (other);

		if (other == null)
			return;

		var enemy = other.gameObject.GetComponent<Enemy>();

		if (enemy != null && startShieldTimer)
		{
			Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

			Vector2 force = Vector2.zero;

			//if the enemy lands on top of me
			if (enemyRb != null && enemy.transform.position.y  - enemy.GetComponent<BoxCollider2D>().bounds.extents.y > transform.position.y + boxCol.bounds.extents.y)
			{
				force = Vector2.up * 50.0f;
			}

			if (enemyRb != null)
				force = (enemy.transform.position.x < transform.position.x) ? Vector2.left * 100.0f : Vector2.right * 100.0f;
				
			enemyRb.AddForce(force);
		}

		if (HelperFunctions.GroundCheck(edgeCol, rb))
		{
			isHovering = false;
			canHover = false;
		}
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	private void Hover(int playerID)
	{
		if (playerID != this.playerID)	
			return;
		
		//if the player can hover freeze their y position so they begin hovering
		if (!grounded && !isHovering)
		{
			rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
			//player is now hovering 
			isHovering = true;
		}

	}

	private void StopHovering()
	{
		//unfreeze the players position
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		//player is no longer hovering
		isHovering = false;
		//and cannot hover again
		canHover = false;
	}
}

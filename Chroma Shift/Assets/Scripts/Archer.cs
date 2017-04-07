using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;

public class Archer : Hero {

	[SerializeField] GameObject bow;
	[SerializeField] GameObject shield;
	float currentBowRot;

	protected override void Start ()
	{
		base.Start ();

		if (InputManager.Instance)
		{
			InputManager.Instance.TrackMouseEvent += TrackMouseEvent;
			InputManager.Instance.RotateBow += RotateBow;
		}
		shield.GetComponent<SpriteRenderer>().color = Color.clear;
		bow.GetComponent<SpriteRenderer>().color = Color.white;
	}
	protected override void OnDestroy ()
	{
		base.OnDestroy ();

		if (InputManager.Instance)
		{
			InputManager.Instance.TrackMouseEvent -= TrackMouseEvent;
			InputManager.Instance.RotateBow -= RotateBow;
		}
			
	}
	//method for the archers attack
	protected override void Attack ()
	{
		Vector2 arrowSpawnPoint;

		//if the hero is facing right spawn the arrow at the right side of their sprite
		if(facingRight)
			arrowSpawnPoint = new Vector3(transform.position.x + edgeCol.bounds.extents.x + projectile.GetComponent<SpriteRenderer>().sprite.bounds.extents.x, transform.position.y, transform.position.z);
		//if the hero is facing left spawn an arrow at the left side of their sprite
		else 
			arrowSpawnPoint = new Vector3(transform.position.x - edgeCol.bounds.extents.x - projectile.GetComponent<SpriteRenderer>().sprite.bounds.extents.x, transform.position.y, transform.position.z);

		//call the arc method to get the velocity for the arrow
		Vector2 arrowVelocity = HelperFunctions.Arc(bow.GetComponent<Transform>()) * stats.attackSpeed;

		//negate the velocity if the hero is facing left
		if(!facingRight)
			arrowVelocity.x *= -1;

		//call the shoot arrow method
		ShootArrow(arrowSpawnPoint, arrowVelocity);

		//call the shoot arrow method over the network
		if(photonView.isMine)
			photonView.RPC("ShootArrow", PhotonTargets.Others, arrowSpawnPoint, arrowVelocity);
	}
	//method for shooting an arrow 
	[PunRPC] void ShootArrow(Vector2 spawn, Vector2 velocity)
	{
		//instantiate the arrow
		GameObject arrow = Instantiate(projectile, spawn, Quaternion.identity) as GameObject;
		//set the projectiles velocity
		arrow.GetComponent<Rigidbody2D>().velocity = velocity;

		arrow.GetComponent<Projectile>().hero = this;
		//get the scale of the arrow
		Vector3 scale = arrow.transform.localScale;

		//arrow.transform.localEulerAngles = new Vector3(bow.transform.rotation.eulerAngles.x, bow.transform.rotation.y, bow.transform.rotation.eulerAngles.z);
		//flip the scale of the arrow if the velocity has been negated 
		arrow.transform.localScale = new Vector3(scale.x * (velocity.x < 0 ? -1 : 1), scale.y, scale.z);

	}

	[PunRPC] protected override void Block ()
	{
		base.Block ();

		if (canBlock)
		{
			bow.GetComponent<SpriteRenderer>().DOColor(Color.clear,0.1f);
			shield.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f);
		}
	}

	[PunRPC] protected override void FinishedBlocking()
	{
		base.FinishedBlocking();
		bow.GetComponent<SpriteRenderer>().DOColor(Color.white,0.1f);
		shield.GetComponent<SpriteRenderer>().DOColor(Color.clear, 0.1f);
	}

	private void TrackMouseEvent(int playerID, float x, float y)
	{
		if (playerID != this.playerID)
			return;

		if (Input.GetJoystickNames().Length == 0)
		{
			//track the mouse cursor at the position of the player
			y -= CameraBehaviour.Instance.GetComponentInChildren<Camera>().WorldToScreenPoint(transform.position).y;
			//clamp the rotation of the bow between 0 and 90 degrees
			y = Mathf.Clamp(y, 0, 90);
			//rotate the bow zround the z axis based on the mouse position
			bow.transform.localEulerAngles = new Vector3(bow.transform.localRotation.x, bow.transform.localRotation.y, y);
		}

	}

	protected override void Update ()
	{
		base.Update ();

		var block = (canBlock) ? true : false;
		shield.SetActive(block);
	}
	private void RotateBow(int playerID, float axis)
	{
		if (axis == 0)
			return;
		
		if (playerID != this.playerID)
			return;

		if (axis > 0)
			currentBowRot+=5;
		if (axis < 0)
			currentBowRot-=5;

		currentBowRot = Mathf.Clamp(currentBowRot, 0, 90);
	
		bow.transform.localEulerAngles = new Vector3(0.0f, 0.0f, currentBowRot);

		Debug.Log(currentBowRot);
	}
}

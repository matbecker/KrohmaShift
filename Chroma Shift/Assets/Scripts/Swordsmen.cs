using UnityEngine;
using System.Collections;

public class Swordsmen : Hero {

	[SerializeField] GameObject sword;

	protected override void Start ()
	{
		base.Start ();
	}

	protected override void OnDestroy ()
	{
		base.OnDestroy ();
	}

	protected override void Attack ()
	{
		base.Attack ();
		//dont allow the swordsmen to do anything else while hes swinging his giant sword
		disableInput = true;

		PlayAttackAnimation();
		//call the play attack animation over the network
		if (photonView.isMine)
			photonView.RPC("PlayAttackAnimation", PhotonTargets.Others);
	}
	//method for playing the ninjas attack animation
	[PunRPC] private void PlayAttackAnimation()
	{
		weaponAnim.SetBool("isAttacking", true);
	}
	//swordsmens block consists of a block animation
	[PunRPC] protected override void Block ()
	{
		base.Block ();
		weaponAnim.SetBool("isBlocking", true);
	}
	//swordsmens finished blocking consists of stopping the block animation
	[PunRPC] protected override void FinishedBlocking()
	{
		base.FinishedBlocking();
		weaponAnim.SetBool("isBlocking", false);
	}
		
	protected override void Update ()
	{
		base.Update ();

		var block = (canBlock) ? true : false;
		sword.SetActive(block);
	}

}

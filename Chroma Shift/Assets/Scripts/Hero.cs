using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.DemiLib;
using DG.Tweening;

public class Hero : Photon.MonoBehaviour, IProjectileIgnore {

	public enum Type { Archer, Ninja, Swordsmen, Wizard };

	[System.Serializable]
	public class Stats
	{
		public int currentHealth;
		public int maxHealth;
		public int attackPower;
		public float attackRange;
		public float attackSpeed;
		public float attackCooldownRate;
		public float currentShieldStrength;
		public float shieldCapacity;
		public Vector2 maxVelocity;
		public Vector2 jumpForce;
		public Vector2 movementForce;
		public int colourShifts;
		public float damageCooldownTime;
	}
	public Stats stats;
	public Type type;
	public ColourManager colour;
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] protected EdgeCollider2D edgeCol;
	[SerializeField] protected BoxCollider2D boxCol;
	[SerializeField] protected Image shieldBar;
	[SerializeField] public SpriteRenderer sprite;
	[SerializeField] protected GameObject projectile;
	[SerializeField] protected GameObject tmpProjectile;
	[SerializeField] protected Animator anim;
	[SerializeField] protected Animator weaponAnim;
	[SerializeField] protected bool depleteOnHit;
	[SerializeField] protected bool rangedAttack;
	[SerializeField] SpriteRenderer[] glowRenderers;
	[SerializeField] TrailRenderer tr;
	[SerializeField] Gradient[] gradients;
	[SerializeField] GameObject[] gear;
	[SerializeField] Transform[] gearPos;
	protected bool startShieldTimer;
	protected bool canBlock;
	protected bool isBlocking;
	public bool grounded;
	private bool canAttack;
	public bool isAttacking;
	private bool rechargeShield;
	public bool disableInput;
	public bool facingRight;
	protected bool isInvinsible;
	protected bool isDamaged;
	public bool isFollowTarget;
	private float timer;
	private float CooldownTimer;
	private Coroutine transparencyCor;
	private bool canSlide;
	public bool isSpawning;
	public int playerID;
	public bool alive;



	//Online Synchronization variables
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;


	protected virtual void Awake()
	{
		gameObject.tag = "Player";

		alive = true;
	}
	protected virtual void Start () 
	{
		if(photonView.isMine)
		{
			//register events
			InputManager.Instance.Jump += Jump;
			InputManager.Instance.Run += Run;
			InputManager.Instance.Attack += TryAttack;
			InputManager.Instance.Block += TryBlock;
			InputManager.Instance.UnBlock += UnBlock;
			InputManager.Instance.SwitchColour += SwitchColour;
			InputManager.Instance.SwitchShade += SwitchShade;
			InputManager.Instance.Slide += Slide;
			LevelManager.Instance.Restart += RestartLevel;
		}
		if (PauseOverlay.Instance)
		{
			PauseOverlay.Instance.Freeze += Freeze;
			PauseOverlay.Instance.UnFreeze += UnFreeze;
		}
		isDamaged = true;
		//get the rigidbody of the gameobject
		rb = gameObject.GetComponent<Rigidbody2D>();
		//get the edge collider of the gameobject
		edgeCol = gameObject.GetComponent<EdgeCollider2D>();
		//get the box trigger of the gameObject
		boxCol = gameObject.GetComponent<BoxCollider2D>();


		//ensure the player starts with max health
		stats.currentHealth = stats.maxHealth;
		//ensure the players shield is at max capacity
		stats.currentShieldStrength = stats.shieldCapacity;
		//get the shield health bar from the canvas object attached to the player
		shieldBar = gameObject.GetComponentInChildren<Image>();
		//the player can block when they start
		canBlock = true;
		//the player can attack when they start
		canAttack = true;
		//the player beigins facing right
		facingRight = true;
		//ensure the players shieldbar is not showing
		shieldBar.canvasRenderer.SetAlpha(0.01f);
		//null the co-routine
		transparencyCor = null;

		if(!photonView.isMine) 
		{
			//Get the Photon Data
			var data = photonView.instantiationData;
			//sync other players colour data with photon
			colour.currentColourType = (ColourManager.ColourType)data[0];
			//sync other players shade data with photon
			colour.shadeIndex = (int)data[1];
			//dont worry about the other players gravity
			rb.gravityScale = 0;
		}
		SetupSprite();
	}

	public void SetupSprite()
	{
		//set the color to the players colour 
		//sprite.color = Color.clear;
		sprite.DOColor(colour.GetCurrentColor(), 1.0f);
		SetGlowRenderers(colour.GetCurrentColor(), 0.0f);
		tr.colorGradient = gradients[(int)colour.currentColourType];
	}

	protected virtual void OnDestroy () 
	{
		if (InputManager.Instance)
		{
			if (photonView.isMine)
			{
				//unregister events
				InputManager.Instance.Jump -= Jump;
				InputManager.Instance.Run -= Run;
				InputManager.Instance.Attack -= TryAttack;
				InputManager.Instance.Block -= TryBlock;
				InputManager.Instance.UnBlock -= UnBlock;
				InputManager.Instance.SwitchColour -= SwitchColour;
				InputManager.Instance.SwitchShade -= SwitchShade;
				InputManager.Instance.Slide -= Slide;
			}
		}
		if (PauseOverlay.Instance)
		{
			PauseOverlay.Instance.Freeze -= Freeze;
			PauseOverlay.Instance.UnFreeze -= UnFreeze;
		}

	}
	//method for passing stuff to photon 
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//send position
			stream.SendNext(rb.position);
			//send velocity
			stream.SendNext(rb.velocity);
			//send scale
			stream.SendNext(transform.localScale.x);
		}
		else
		{
			var syncPosition = (Vector2)stream.ReceiveNext();
			var syncVelocity = (Vector2)stream.ReceiveNext();
			var xScale = (float)stream.ReceiveNext();

			Vector3 scale = transform.localScale;
			transform.localScale = new Vector3(xScale, scale.y, scale.z);
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			syncEndPosition = syncPosition + syncVelocity * syncDelay;
			syncStartPosition = rb.position;
		}
	}
	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		//lerp position towards where the player is headed over the network
		rb.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}


	// Update is called once per frame
	protected virtual void Update () 
	{
		if(!photonView.isMine)
		{
			//sync other players movement over the network 
			SyncedMovement();
		}

		if(photonView.isMine)
		{
			//if the player has attacked
			if (isAttacking)
			{
				//start a timer
				timer += Time.deltaTime;

				//if the timer is greater than the attack cooldown rate
				if (timer >= stats.attackCooldownRate)
				{
					//reset the timer
					timer = 0.0f;

					ResetAttack();
					//Reset the attack over the network
					photonView.RPC("ResetAttack", PhotonTargets.Others);
				}
			}
		}
		
		//if the heros shield doesnt depletes when it is hit
		if (!depleteOnHit)
		{
			//start the shield timer
			if (startShieldTimer)
			{
				//they are invinsible
				isInvinsible = true;
				//shield depletes over time once the hero starts blocking
				stats.currentShieldStrength -= Time.deltaTime;
			}
			else
			{
				isInvinsible = false;
			}
		}
		//players shield depletes when it is hit
		else
		{
			//if the player is not blocking and their shield strength is not at its max capacity
			if (!isBlocking && stats.currentShieldStrength < stats.shieldCapacity)
			{
				//increase their shield strength over time
				stats.currentShieldStrength += Time.deltaTime;
				//check if their shield is full
				CheckShieldFull();
			}

		}
		//if their shield is empty
		if (stats.currentShieldStrength <= 0.0f)
		{
			//their shield is broken and they can not use it 
			ShieldDepleted();
		}
		//if the player can not block
		if (!canBlock)
		{
			//recharge their shield over time
			stats.currentShieldStrength += Time.deltaTime;
			//check if their shield is full
			CheckShieldFull();
		}
		if (isDamaged)
		{
			CooldownTimer += Time.deltaTime;
			//anim.speed += 0.01f;
			if (CooldownTimer > stats.damageCooldownTime)
			{
				SetGlowRenderers(colour.GetCurrentColor(), 0.1f);
				anim.SetBool("hurt", false);
				isDamaged = false;
			}
		}
		//make the x scale of the shield bar image equal the heros current shield strength / their max shield strength (value between 0-1)
		shieldBar.rectTransform.localScale = new Vector2(stats.currentShieldStrength / stats.shieldCapacity, 1.0f);

		if (rb.velocity.y > stats.maxVelocity.y)
			rb.velocity = new Vector2(rb.velocity.x, stats.maxVelocity.y);
		else if (rb.velocity.y < -stats.maxVelocity.y)
			rb.velocity = new Vector2(rb.velocity.x, -stats.maxVelocity.y);

		if (rb.velocity.x > stats.maxVelocity.x)
			rb.velocity = new Vector2(stats.maxVelocity.x, rb.velocity.y);
		else if (rb.velocity.x < -stats.maxVelocity.x)
			rb.velocity = new Vector2(-stats.maxVelocity.x, rb.velocity.y);



		if (transform.localPosition.y < LevelManager.LEVEL_BOTTOM || transform.localPosition.y > LevelManager.LEVEL_TOP)
			OffMap();
			

//		if (transform.position.y < -5.0f || transform.position.y > 5.0f)
//		{
//			UpdateCameraTarget();
//		}
	}

	int delay = 0;
	void FixedUpdate()
	{
		if(playerID == 0)
			return;
		
		var camBehaviour = CameraBehaviour.Instance;

		if(!camBehaviour.isSetup)
			return;

		if(delay > 5) 
		{
			var cam = camBehaviour.cam;
			var vpLeft = new Vector3(0f, 0f, -10f);
			var vpRight = new Vector3(1f, 0f, -10f);
			var left = cam.ViewportToWorldPoint(vpLeft) + (Vector3.right * 0.25f);
			var right = cam.ViewportToWorldPoint(vpRight) + (Vector3.left * 0.25f);
			rb.position = new Vector2(Mathf.Clamp(rb.position.x, left.x, right.x), rb.position.y);
		}
		else 
			delay++;
	}
	private void CheckShieldFull()
	{
		//if the players shield is full 
		if (stats.currentShieldStrength >= stats.shieldCapacity)
		{
			//check to see if the coroutine is still running
			if(transparencyCor != null)
			{
				//stop it from running
				StopCoroutine(transparencyCor);
				transparencyCor = null;
			}
			// dont let their shield capcity go higher than the max capacity
			stats.currentShieldStrength = stats.shieldCapacity;
			//Fade out the shield bar image
			shieldBar.CrossFadeAlpha(0.01f, 0.4f, false);
			//player can block once again
			canBlock = true;
		}
	}
	private void Jump(int playerID)
	{
		if (playerID != this.playerID)	
			return;
		
		if (!disableInput)
		{
			//if the player is not blocking
			if (!isBlocking)
			{
				//if the player is on the ground layer then apply a force in the y direction
				if (grounded) 
				{
					var force = (rb.gravityScale == 1) ? stats.jumpForce : -stats.jumpForce;
					rb.AddForce(force);
					transform.DOScaleZ(1.0f, 0.1f).OnComplete(() => {
						grounded = false;
					});
				}
			}
		}
	}
	private void Run(int playerID, float horizontalAxis)
	{
		if (playerID != this.playerID)	
			return;
		
		if (!disableInput)
		{
			//if the player is not blocking
			if (!isBlocking)
			{
				if (horizontalAxis != 0)
				{
					Vector2 dir = (horizontalAxis > 0) ? stats.movementForce : -stats.movementForce;
					facingRight = (horizontalAxis > 0) ? true : false;
					rb.AddForce(dir);
				}
			}
			HelperFunctions.FlipScale(gameObject, facingRight);
		}
	}
	//method to call the attack method when all the inside requirements are met
	private void TryAttack(int playerID)
	{
		if (playerID != this.playerID)	
			return;
		
		//if the player has input
		if (!disableInput)
		{
			//if the player is not blocking they can attack and they havent already attacked
			if (!isBlocking && canAttack && !isAttacking && !canSlide)
			{
				Attack();
				//the hero has attacked and will not be able to again until their cooldown is satisfied
				isAttacking = true;
			}
		}
	}
	//method to get overriden for each heroes attack
	protected virtual void Attack(){}

	//method for resetting attack variables
	[PunRPC] public void ResetAttack() 
	{
		//player can attack again
		isAttacking = false;

		//stop sword animations
		if (!rangedAttack)
			weaponAnim.SetBool("isAttacking", false);

		disableInput = false;
	}
	//method to call the block function when all the requirements are satisfied
	private void TryBlock(int playerID)
	{
		if (playerID != this.playerID)	
			return;
		
		//if the player has input
		if (!disableInput)
		{
			//if the player is not attacking and they can block
			if (!isAttacking && canBlock && !canSlide)
			{
				Block();

				//call the block method over the network
				if(photonView.isMine)
					photonView.RPC("Block", PhotonTargets.Others);
			}
		}
	}
	//method for blocking logic
	protected virtual void Block() 
	{
		//display their shield bar
		shieldBar.CrossFadeAlpha(1f, 0.4f, false);

		if (depleteOnHit)
		{
			//player is holding the block button and their shield will break on impact
			isBlocking = true;
		}
		else
		{
			//player has pushed the block button is temporarily invinsible
			startShieldTimer = true;
		}
		//hero can not attack if they are blocking
		canAttack = false;
	}
	//method for unblocking logic
	private void UnBlock(int playerID)
	{
		if (playerID != this.playerID)	
			return;
		
		//if their shield breaks on impact
		if (depleteOnHit) 
		{
			FinishedBlocking();

			//call the finished blocking method over the network
			if(photonView.isMine){
				photonView.RPC("FinishedBlocking", PhotonTargets.Others);
			}
		}
		//The hero can attack once they are no longer blocking 
		canAttack = true;
	}
	//method for when the player is finished blocking
	protected virtual void FinishedBlocking()
	{
		//check if their shield is full
		CheckShieldFull();
		//they are no longer blocking because they let go of the block keypress
		isBlocking = false;
	}
	//method for the players shield breaking
	private void ShieldDepleted()
	{
		// this will get increased in update because canBlock is false
		stats.currentShieldStrength = 0.0f;
		//player can no longer block 
		canBlock = false;
		//stop the shield timer
		startShieldTimer = false;

		if(transparencyCor == null) 
		{
			//start the coroutine to let the player know their shield is depleted and they cant use it
			transparencyCor = StartCoroutine(HelperFunctions.TransitionTransparency(shieldBar, 0.1f));
		}
	}
	private void Death(float timePenalty)
	{
		HeroManager.Instance.deadPlayers++;
		LevelManager.Instance.levelTimer -= timePenalty;
		PlayerUI.Instance.TimerFlash();
		StarBehaviour.Instance.ChangeDuration(timePenalty); 
		LevelManager.Instance.InvokeHeroDeath(this);

		if (HeroManager.Instance.numPlayers == 0)
		{
			Respawn();
		}
		else
		{
			gameObject.SetActive(false);
			isFollowTarget = false;
		}
	}
	public void Respawn()
	{
		HeroManager.Instance.deadPlayers--;
		gameObject.SetActive(true);
		canSlide = false;
		stats.currentHealth = stats.maxHealth;
		PlayerUI.Instance.SetHealthBarColour(this, Color.black, true);
		transform.position = LevelManager.Instance.currentSpawnPoint.transform.position;
		rb.velocity = Vector2.zero;
		OnHeroSpawn();
		EnemySpawner.ClearEnemies();
		SetupSprite();
		alive = true;
	}
	private void RestartLevel()
	{
		canSlide = false;
		stats.colourShifts = 1;
		stats.currentHealth = stats.maxHealth;
		transform.position = LevelManager.Instance.currentSpawnPoint.transform.position;
		rb.velocity = Vector2.zero;
		OnHeroSpawn();
	}
	public void OnHeroSpawn()
	{
		if (!isSpawning)
		{
			delay = 0;
			anim.SetTrigger("onHeroEntry");
			CameraBehaviour.Instance.atEnd = false;
			isSpawning = true;
		}
	}
	public void DisableGravity()
	{
		rb.gravityScale = 0.0f;
	}
	public void EnableGravity()
	{
		rb.gravityScale = 1.0f;
	}
	private void Slide(int playerID, float axis)
	{
		if (canSlide)
		{
			if (axis != 0)
			{
				canSlide = true;
				var force = (axis > 0) ? Vector2.up * 5.0f : Vector2.down * 5.0f;
				edgeCol.enabled = false;
				rb.AddForce(force);
			}
		}
	}
	private void UpdateCameraTarget()
	{
		if (!alive && isFollowTarget)
		{
			
		}
	}
	protected virtual void OnCollisionExit2D(Collision2D other) 
	{ 
		canSlide = false;
	}

	protected virtual void OnCollisionEnter2D(Collision2D other)
	{
		var enemy = other.gameObject.GetComponent<Enemy>();

		if (enemy != null && isBlocking && canBlock)
		{
			other.gameObject.SendMessage("Damage", 2, SendMessageOptions.DontRequireReceiver);

			stats.currentShieldStrength -= 2.0f;
		}

		if (other.collider.CompareTag("ShiftPowerUp") && stats.colourShifts < 5)
		{
			stats.colourShifts++;
			PlayerUI.Instance.SetColourShifts(this);
			Destroy(other.gameObject);
		}
	}
	protected virtual void OnCollisionStay2D(Collision2D other)
	{
		if (HelperFunctions.GroundCheck(edgeCol, rb))
		{
			grounded = true;
			canSlide = false;
		}
			
		if (HelperFunctions.WallCheck(boxCol, transform, facingRight) && other.collider.CompareTag("StickyPlatform"))
		{
			canSlide = true;
		}
	}

	//method for switching the players colour
	[PunRPC] public void SwitchColour(int playerID)
	{
		if (playerID != this.playerID)
			return;
		
		if (stats.colourShifts != 0)
		{
			stats.colourShifts--;
			//set the next colour
			colour.NextColour();

			//set the sprites colour to equal what the new colour is
			sprite.DOColor(colour.GetCurrentColor(), 0.5f);

			SetGlowRenderers(colour.GetCurrentColor(), 0.5f);

			if (PlayerUI.Instance)
				PlayerUI.Instance.SetColourShifts(this);

			tr.colorGradient = gradients[(int)colour.currentColourType];
				
			if (PlayerUI.Instance)
				PlayerUI.Instance.SetHealthBarGlow(this);
			
			//call the switch colour method over the network so players can see each others colour
			if(photonView.isMine)
				photonView.RPC("SwitchColour", PhotonTargets.OthersBuffered);
		}
	}

	//method for switching the players shade
	[PunRPC] public void SwitchShade(int playerID)
	{
		if (playerID != this.playerID)
			return;
		
		//set the next shade
		colour.NextShade();

		//set the colour of the shade to equal what the new shade is 
		sprite.DOColor(colour.GetCurrentColor(), 0.5f);

		SetGlowRenderers(colour.GetCurrentColor(), 0.5f);

		tr.colorGradient = gradients[(int)colour.currentColourType];
		//call the switch shade method so the players can see each others shades
		if(photonView.isMine)
			photonView.RPC("SwitchShade", PhotonTargets.OthersBuffered);
	}
	public virtual void Damage(int damageAmount)
	{
		//only damage the hero if they arent blocking or arent invisible
		if (depleteOnHit && !isBlocking && !isDamaged || !depleteOnHit && !isInvinsible && !isDamaged)
		{
			stats.currentHealth -= damageAmount;
			CooldownTimer = 0.0f;
			anim.SetBool("hurt", true);
			isDamaged = true;

			SetGlowRenderers(Color.clear, 0.1f);
			PlayerUI.Instance.SetHealthBarColour(this, Color.black, false);

			if (stats.currentHealth <= 0)
			{
				UnparentGear();
				anim.SetTrigger("heroDead");
				isDamaged = true;
				alive = false;
			}
		}
	}
	private void OffMap()
	{
		CameraBehaviour.Instance.Shake(0.75f,0.0f,1.0f,true);
		alive = false;
		Death(10.0f);
	}
	public void DisplayLoadingScreen()
	{
		LoadingScreen.Instance.DisplayLoadingScreen(LoadingScreen.ScreenState.NextLevel);
	}
	private void Freeze()
	{
		disableInput = true;
	}
	private void UnFreeze()
	{
		disableInput = false;
	}
	private void SetGlowRenderers(Color colour, float duration)
	{
		foreach (SpriteRenderer sr in glowRenderers)
		{
			sr.DOColor(colour, duration);
		}
	}
	private void UnparentGear()
	{
		for (int i = 0; i < gear.Length; i++)
		{
			var obj = Instantiate(gear[i], gearPos[i].position, Quaternion.identity) as GameObject;
			obj.AddComponent<BoxCollider2D>();
			obj.AddComponent<Rigidbody2D>();
			obj.layer = 15;

			var sword = obj.GetComponentInChildren<SwordAnimation>();

			if (sword != null)
				sword.enabled = false;

			LevelManager.Instance.droppedGear.Add(obj);
		}
	}
}

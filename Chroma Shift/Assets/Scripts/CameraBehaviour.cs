using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;

public class CameraBehaviour : MonoBehaviour {

	private static CameraBehaviour instance;
	public static CameraBehaviour Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(CameraBehaviour)) as CameraBehaviour;

			return instance;
		}
	}

	[SerializeField] Vector2 velocity;
	[SerializeField] float smoothTime;
	[SerializeField] Hero hero;
	[SerializeField] Vector2 minPos;
	[SerializeField] Vector2 maxPos;
	[SerializeField] Vector3 center;
	[SerializeField] float verticalSpeed;
	[SerializeField] float horizontalSpeed;
	[SerializeField] float ZoomValue;
	[SerializeField] int orthoSize;
	[SerializeField] GameObject child;
	[SerializeField] SpawnPoint startPoint;
	public Camera cam;
	public bool inGame;
	public bool paused;
	public bool atEnd;
	private Vector2 targetPos;
	public bool isSetup;

	void Awake()
	{
		var c = (maxPos.x + minPos.x) / 2;
		center = new Vector3(c, 0.0f, -10.0f);
		paused = false;
		orthoSize = 5;

		if (!inGame) {
			InputManager.Instance.HorizontalMovement += HorizontalMovement;
			InputManager.Instance.VerticalMovement += VerticalMovement;
			InputManager.Instance.CameraZoomIn += CameraZoomIn;
			InputManager.Instance.CameraZoomOut += CameraZoomOut;
		}

		if (inGame) {
			LevelManager.Instance.OnHeroSpawned += OnHeroSpawned;
			LevelManager.Instance.OnHeroDeath += OnHeroDeath;
//			foreach(var objList in LevelLoader.Instance.objectLists) {
//
//				foreach (var levelObject in objList.Value)
//				{
//					if (transform.position.x > levelObject.transform.position.x)
//					{
//						minPos = new Vector2(levelObject.transform.position.x, minPos.y);
//					}
//					if (transform.position.x < levelObject.transform.position.x + levelObject.GetComponent<BoxCollider2D>().bounds.extents.x)
//					{
//						maxPos = new Vector2(levelObject.transform.position.x, maxPos.y);
//					}
//				}
//			}

		}
			
	}
	public void SetPauseScreen()
	{
		paused = true;
		cam.DOOrthoSize(32, 2.0f);
		child.transform.DOMove(center, 2.0f).OnComplete(() => 
		{
			child.transform.DOMoveY(center.y - 10.0f, 2.0f, false).SetEase(Ease.InOutBack).OnComplete(() => 
			{
				PauseOverlay.Instance.canPause = true;
			});
		});
	}
	public void ExitPauseScreen()
	{
		cam.DOOrthoSize(orthoSize,2.0f);

		child.transform.DOMove(new Vector3(targetPos.x,targetPos.y, -10.0f), 1.0f).OnComplete(() => 
		{
			paused = false; 
		});
	}
	private void ResetChildObject()
	{
		child.transform.DOMove(Vector3.zero, 0.5f, false);
	}
	void OnDestroy()
	{
		
	}
	void OnHeroSpawned(Hero hero) 
	{
		if(hero.playerID != 0)
			return;
		
		this.hero = hero;

		//set the follow target
		hero.isFollowTarget = true;
		LoadingScreen.Instance.SetCanvasCamera();
	}

	// Update is called once per frame
	void Update () 
	{
		if(hero == null)
			return;

		if (inGame && !paused && !atEnd)
		{
			if (hero.GetComponent<Hero>().isFollowTarget)
			{
			targetPos = new Vector2(Mathf.SmoothDamp(transform.position.x, hero.transform.position.x, ref velocity.x, smoothTime),
				Mathf.SmoothDamp(transform.position.y, hero.transform.position.y, ref velocity.y, smoothTime));

			transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);

			transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x), Mathf.Clamp(transform.position.y, minPos.y, maxPos.y), transform.position.z);
			isSetup = true;
			}	
		}
	}

	public void Shake(float duration, float xSeverity, float ySeverity, bool lerp)
	{
		child.transform.DOShakePosition(duration,new Vector3(xSeverity, ySeverity, child.transform.position.z), 10, 0, false).OnComplete(() => {
			if (lerp)
				DOTween.To(() => 0f, t => {
					child.transform.transform.position = Vector3.Lerp(child.transform.position, transform.position, t);
				}, 1f, 0.5f);
		});
	}

	private void CameraZoomIn(int playerID)
	{
		var cam = gameObject.GetComponent<Camera>();
		var zV = ZoomValue;

		if (cam.orthographicSize < 1.5f)
		{
			zV = 0.0f;
		}
		else
		{
			zV = ZoomValue;
		}
		cam.orthographicSize -= zV;
	}

	private void CameraZoomOut(int playerID)
	{
		var cam = gameObject.GetComponent<Camera>();
		var zV = ZoomValue;

		if (cam.orthographicSize > 50.0f)
		{
			zV = 0.0f;
		}
		else
		{
			zV = ZoomValue;
		}
		cam.orthographicSize += zV;
	}
	//editor stuff
	private void HorizontalMovement(int playerID, float horizontalAxis)
	{
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x), 
										Mathf.Clamp(transform.position.y, minPos.y, maxPos.y), 
										transform.position.z);

		if (horizontalAxis > 0)
		{
			transform.position += Vector3.right * horizontalSpeed;
		}
		if (horizontalAxis < 0)
		{
			transform.position += Vector3.left * horizontalSpeed;
		}
	}
	private void VerticalMovement(int playerID, float verticalAxis)
	{
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPos.x, maxPos.x), 
			Mathf.Clamp(transform.position.y, minPos.y, maxPos.y), 
			transform.position.z);
		
		if (verticalAxis > 0)
		{
			transform.position += Vector3.up * verticalSpeed;
		}
		if (verticalAxis < 0)
		{
			transform.position += Vector3.down * verticalSpeed;
		}
	}
	public void SetCameraTarget(Hero h)
	{
		hero = h;

		//new hero is now the follow target
		hero.isFollowTarget = true;
	}
	private void OnHeroDeath(Hero hero)
	{
		//if there is an alive hero reset the camera target
		if (HeroManager.Instance.numPlayers > 0)
			SetCameraTarget(LevelManager.Instance.GetAliveHero());
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseOverlay : MonoBehaviour {

	private static PauseOverlay instance;
	public static PauseOverlay Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(PauseOverlay)) as PauseOverlay;

			return instance;
		}
	}
	private bool isPaused;
	public bool canPause;
	[SerializeField] Image sprite;
	[SerializeField] GameObject buttonPanel;
	[SerializeField] Animator anim;
	[SerializeField] Button[] pauseButtons;

	public delegate void FreezeEvent();
	public event FreezeEvent Freeze;
	public event FreezeEvent UnFreeze;
	public EventSystem es;


	// Use this for initialization
	void Start () 
	{
		InputManager.Instance.Pause += TogglePause;
		sprite.color = Color.clear;
		isPaused = false;
		canPause = true;
	}
	void OnDestroy()
	{
		if (InputManager.Instance)
			InputManager.Instance.Pause -= TogglePause;
	}
	public void TogglePause(int playerID)
	{
		if (canPause)
		{
			isPaused = !isPaused;

			if (isPaused)
			{
				
				if (Freeze != null)
				{
					Freeze();
				}
				buttonPanel.transform.DOScale(Vector3.one, 2.0f).SetDelay(3.0f).SetEase(Ease.OutBounce, 0.5f).OnComplete(() => 
				{
					es.SetSelectedGameObject(pauseButtons[0].gameObject);
					canPause = true;
				});

				sprite.DOColor(new Color(1,1,1,0.7f), 2.0f);

				CameraBehaviour.Instance.SetPauseScreen();
				PlayerUI.Instance.canvas.sortingOrder = 3;
				PlayerUI.Instance.TimerStop();
				LevelManager.Instance.startTimer = false;
				canPause = false;
			}
			else
			{
				if (UnFreeze != null)
				{
					UnFreeze();
				}
				es.SetSelectedGameObject(null);
				buttonPanel.transform.DOScale(Vector3.zero, 0.5f);
				sprite.DOColor(Color.clear, 2.0f);
				CameraBehaviour.Instance.ExitPauseScreen();
				PlayerUI.Instance.canvas.sortingOrder = 10;
				PlayerUI.Instance.TimerStart();
				LevelManager.Instance.startTimer = true;
			}
			SoundManager.Instance.TogglePause();
			StarBehaviour.Instance.SetPaused(isPaused);
		}

 	}

	public void Restart()
	{
		TogglePause(0);
		LoadingScreen.Instance.DisplayLoadingScreen(LoadingScreen.ScreenState.RestartLevel);
	}

	public void LoadScene(string sceneName)
	{
		LevelLoader.Instance.sceneLoaded = false;

		switch (sceneName)
		{
		case "CharacterSelect":
			LoadingScreen.Instance.DisplayLoadingScreen(LoadingScreen.ScreenState.CharacterSelect);
			break;
		case "StartScreen":
			LoadingScreen.Instance.DisplayLoadingScreen(LoadingScreen.ScreenState.MainMenu);
			break;
		case "LevelSelect":
			LoadingScreen.Instance.DisplayLoadingScreen(LoadingScreen.ScreenState.MainMenu);
			break;
			default:
			break;
		}
	}

	public void ToggleButtonSize(GameObject obj)
	{
		var size = (obj.transform.localScale.x > 1.0f) ? Vector3.one : Vector3.one * 1.1f;
		obj.transform.DOScale(size, 0.5f);
	}

	public void ButtonClick(GameObject obj)
	{
		obj.transform.DOScale(Vector3.one * 0.9f, 0.25f).OnComplete(() => 
		{
			obj.transform.DOScale(Vector3.one, 0.25f);
		});
	}

	void Update () 
	{

	}
}

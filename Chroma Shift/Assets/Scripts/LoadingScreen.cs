using UnityEngine;
using System.Collections;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

	public enum ScreenState { MainMenu ,LevelEditor, CharacterSelect, Game, RestartLevel, NextLevel };
	private static LoadingScreen instance;
	public static LoadingScreen Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(LoadingScreen)) as LoadingScreen;

			return instance;
		}
	}
	public delegate void StartEvent();
	public event StartEvent Begin;

	[SerializeField] Image loadingOverlay;
	[SerializeField] Animator anim;
	[SerializeField] Canvas canvas;
	public bool startGame;

	public void DisplayLoadingScreen(ScreenState screenState)
	{
		switch (screenState)
		{
		case ScreenState.CharacterSelect:
		case ScreenState.Game:
		case ScreenState.LevelEditor:
		case ScreenState.MainMenu:
			SoundManager.Instance.SceneSwitch();
			break;
		}

		loadingOverlay.DOColor(Color.white, 0.25f).OnComplete(() => 
		{
			anim.SetTrigger("startSpiral");
			loadingOverlay.DOColor(Color.black, 1.0f).OnComplete(() => 
			{
				loadingOverlay.DOColor(Color.clear, 1.0f);
				switch (screenState)
				{
				case ScreenState.MainMenu:
					SceneManager.LoadScene("StartScreen");
					SoundManager.Instance.PlaySong("Khromamenu", 0.0f, true);
					SoundManager.Instance.SceneBegin();
					break;
				case ScreenState.LevelEditor:
					SceneManager.LoadScene("LevelEditor");
					//SoundManager.Instance.PlaySong("Editor", 1.0f, true);
					SoundManager.Instance.SceneBegin();
					break;
				case ScreenState.CharacterSelect:
					SceneManager.LoadScene("CharacterSelectScreen");
					SoundManager.Instance.PlaySong("KhromaCharacterSelect", 0.0f, true);
					SoundManager.Instance.SceneBegin();
					break;
				case ScreenState.Game:
					SoundManager.Instance.PlaySong("Restless", 0.0f, true);
					if (Begin != null)
					{
						Begin();
					}
					break;
				case ScreenState.NextLevel:
					LevelManager.Instance.NextLevel();
					break;
				case ScreenState.RestartLevel:
					LevelManager.Instance.restart = true;
					break;
				}
				SoundManager.Instance.SceneBegin();
			});

		});
	}
	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetCanvasCamera()
	{
		canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();//FindObjectOfType(typeof(Camera)) as Camera;
	}
}

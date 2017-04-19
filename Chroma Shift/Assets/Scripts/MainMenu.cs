using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

	public enum Screen { MainMenu, MultiPlayer, Settings, LevelSelect }
	public Screen ScreenType;
	[System.Serializable]
	public class MenuButtons
	{
		public int index;
		public Screen screen;
		public Button[] buttons;
	}
	public MenuButtons[] menuButtons;
	private static MainMenu instance;
	public static MainMenu Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(MainMenu)) as MainMenu;

			return instance;
		}
	}

	public InputField roomNameInput;
	[SerializeField] GameObject[] screens;
	int currentScreen;
	[SerializeField] Image[] backgroundImages;
	[SerializeField] Animator[] animators;
	[SerializeField] Animator titleAnim;
	private float animTimer;
	private bool delay;
	private Coroutine delayCor;
	public bool toCharacterSelect;
	public bool toLevelEditor;
	public EventSystem es;
	[SerializeField] Button[] startButtons;
	private float timer;
	public bool freezeEventSystem;

	void Start () 
	{
		currentScreen = 0;
		animTimer = 0.0f;
		delayCor = null;
		toLevelEditor = false;
		InputManager.Instance.SwitchButton += SwitchButton;
		InputManager.Instance.Submit += Submit;
		//SoundManager.Instance.PlaySong("KhromaMenu", 1.0f, true);
	}
	void OnDestroy()
	{
		if (InputManager.Instance)
			InputManager.Instance.SwitchButton -= SwitchButton;
	}
	public void LoadLevelEditor()
	{
		toLevelEditor = true;
		LoadingScreen.Instance.DisplayLoadingScreen(LoadingScreen.ScreenState.LevelEditor);
	}

	public void SwitchScreen(int newScreenIndex)
	{
		switch (newScreenIndex)
		{
		case 0:
			if (currentScreen == 1)
			{
				animators[currentScreen].SetBool("toMultiplayerMenu", false);	
				animators[newScreenIndex].SetBool("toMainMenu", true);
			}
			else if (currentScreen == 2)
			{
				animators[currentScreen].SetBool("toSettingsMenu",false);
				animators[newScreenIndex].SetBool("toMainMenu", true);
			}
			else if (currentScreen == 3)
			{
				//delayCor = StartCoroutine(DelayAnimation(1.0f,animators[newScreenIndex], "toMainMenu", true));
				//animators[currentScreen].SetBool("activated", false);
				animators[newScreenIndex].SetBool("toMainMenu", true);
			}
			es.SetSelectedGameObject(menuButtons[newScreenIndex].buttons[0].gameObject);
			ScreenType = Screen.MainMenu;
			break;
		case 1:
			animators[currentScreen].SetBool("toMainMenu", false);
			animators[newScreenIndex].SetBool("toMultiplayerMenu", true);
			es.SetSelectedGameObject(menuButtons[newScreenIndex].buttons[0].gameObject);
			ScreenType = Screen.MultiPlayer;
			break;
		case 2:
			animators[currentScreen].SetBool("toMainMenu", false);
			animators[newScreenIndex].SetBool("toSettingsMenu", true);
			es.SetSelectedGameObject(menuButtons[newScreenIndex].buttons[0].gameObject);
			ScreenType = Screen.Settings;
			break;
		case 3:
			animators [currentScreen].SetBool ("toMainMenu", false);
			LevelSelectScreen.Instance.OpenLevelPanel ();
			ScreenType = Screen.LevelSelect;
			freezeEventSystem = true;
			break;
		default:
			break;
		}
		currentScreen = newScreenIndex;
	}
	public void SetLevelSelectScreenButton()
	{
		var currentMenu = menuButtons [(int)ScreenType];
		currentMenu.index++;
		es.SetSelectedGameObject(menuButtons[currentScreen].buttons[currentMenu.index].gameObject);
	}
	public IEnumerator DelayAnimation(float delayAmount, Animator anim, string animName, bool playAnim)
	{
		yield return new WaitForSeconds(delayAmount);
		anim.SetBool(animName, playAnim);
	}
	public void ConnectToServer()
	{
		NetworkManager.Instance.Connect();
		PhotonNetwork.offlineMode = false;
	}

	public void DisconnectFromServer()
	{
		NetworkManager.Instance.Disconnect();
		PhotonNetwork.offlineMode = true;
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void SinglePlayer() 
	{ 
		PhotonNetwork.offlineMode = true;
	}

	public void CreateServer() 
	{
		NetworkManager.Instance.StartServer(roomNameInput.text);
	}

	public void JoinServer() 
	{
		NetworkManager.Instance.JoinServer(roomNameInput.text);
	}
	private void SwitchButton(int playerID, float axis)
	{
		var currentMenu = menuButtons[(int)ScreenType];

		if (!freezeEventSystem) 
		{
			switch (ScreenType)
			{
			case Screen.MainMenu:
			case Screen.MultiPlayer:
			case Screen.Settings:
			case Screen.LevelSelect:

				if (axis < 0)
				{
					if (timer > 0.2f)
					{
						currentMenu.index++;
						timer = 0.0f;
					}
				}
				else if (axis > 0)
				{
					if (timer > 0.2f)
					{
						currentMenu.index--;
						timer = 0.0f;
					}
				}

				if (currentMenu.index < 0)
					currentMenu.index = currentMenu.buttons.Length - 1;

				if (currentMenu.index >= currentMenu.buttons.Length)
					currentMenu.index = 0;

				es.SetSelectedGameObject(currentMenu.buttons[currentMenu.index].gameObject);
				break;
			}
		}
	}
	private void Submit(int playerID)
	{
		if (!freezeEventSystem)
			menuButtons[currentScreen].buttons[menuButtons[currentScreen].index].onClick.Invoke();
	}
	private void Update()
	{
		timer += Time.deltaTime;
	}
}

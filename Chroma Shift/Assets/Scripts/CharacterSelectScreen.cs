using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using DG.DemiLib;

public class CharacterSelectScreen : Photon.MonoBehaviour {

	[System.Serializable]
	public class Layout
	{
		public Color[] colors;
	}
	public Layout[] layouts;

	public Transform heroPosition;
	//public static string currentLevelName;
	private List<GameObject> characters;
	[SerializeField] Text characterText;
	[SerializeField] Animator anim;
	[SerializeField] Image[] uiSprites;
	[SerializeField] Button[] buttons;
	[SerializeField] Text readyText;
	private int index;
	[SerializeField] EventSystem es;
	private int currentHero;
	private float timer;
	public Dictionary<Button,bool> buttonDict;
	public int playerID;
	private bool isReady;

	public static int playersReady = 0;
	// Use this for initialization
	void Start () 
	{
		buttonDict = new Dictionary<Button,bool>();
		characters = new List<GameObject>();
		LoadingScreen.Instance.SetCanvasCamera();

		for (int i = 0; i < uiSprites.Length; i++)
		{
			uiSprites[i].DOColor(layouts[0].colors[i], 0.0f);
		}
		if (InputManager.Instance)
		{
			InputManager.Instance.SwitchButton += SwitchButton;
			InputManager.Instance.Submit += Submit;
		}
		if (LoadingScreen.Instance)
		{
			LoadingScreen.Instance.Begin += Begin;
		}
		es.SetSelectedGameObject(buttons[0].gameObject);
	}
	private void OnDestroy()
	{
		if (LoadingScreen.Instance)
		{
			LoadingScreen.Instance.Begin -= Begin;
		}
		if (InputManager.Instance)
		{
			InputManager.Instance.SwitchButton -= SwitchButton;
			InputManager.Instance.Submit -= Submit;
		}
	}

	public void NextHero()
	{
		characters[currentHero].SetActive(false);
		currentHero++;
		if(currentHero >= characters.Count)
		{
			currentHero = 0;
		}

		characters[currentHero].SetActive(true);
		HeroManager.Instance.heroIndex[playerID] = currentHero;
		characterText.text = HeroManager.Instance.heroes[currentHero].type.ToString();

		for (int i = 0; i < uiSprites.Length; i++)
		{
			uiSprites[i].DOColor(layouts[(int)characters[currentHero].GetComponent<Hero>().colour.currentColourType].colors[i], 1.0f);
		}

		HeroEntry(characters[currentHero], true);

		uiSprites[0].gameObject.transform.DOScaleY(0.0f, 0.25f).OnComplete(() => 
		{
			uiSprites[0].gameObject.transform.DOScaleY(1.0f,0.5f).SetDelay(0.5f);
		});
	}
	public void PreviousHero()
	{
		characters[currentHero].SetActive(false);
		currentHero--;

		if(currentHero < 0)
		{
			currentHero = characters.Count - 1;
		}

		characters[currentHero].SetActive(true);
		HeroManager.Instance.heroIndex[playerID] = currentHero;
		characterText.text = HeroManager.Instance.heroes[currentHero].type.ToString();

		for (int i = 0; i < uiSprites.Length; i++)
		{
			uiSprites[i].DOColor(layouts[(int)characters[currentHero].GetComponent<Hero>().colour.currentColourType].colors[i], 1.0f);
		}

		HeroEntry(characters[currentHero], false);

		uiSprites[0].gameObject.transform.DOScaleY(0.0f, 0.25f).OnComplete(() => 
		{
			uiSprites[0].gameObject.transform.DOScaleY(1.0f,0.5f).SetDelay(0.5f);
		});
	}
	public void NextColour()
	{
		characters[currentHero].GetComponentInChildren<Hero>().SwitchColour(playerID);

		for (int i = 0; i < uiSprites.Length; i++)
		{
			uiSprites[i].DOColor(layouts[(int)characters[currentHero].GetComponent<Hero>().colour.currentColourType].colors[i], 1.0f);
		}
	}
	public void NextShade()
	{
		characters[currentHero].GetComponentInChildren<Hero>().SwitchShade(playerID);
	}
	public void Ready()
	{
		var colour = characters[currentHero].GetComponentInChildren<ColourManager>();
		HeroManager.Instance.currentColorType[playerID] = colour.currentColourType;
		HeroManager.Instance.currentShadeIndex[playerID] = colour.shadeIndex;

		if(isReady)
		{
			foreach(var button in buttons)
			{
				button.interactable = true;
			}
			playersReady--;
			buttons[2].GetComponentInChildren<Text>().text = "Ready";
		}
		else
		{
			foreach(var button in buttons)
			{
				// buttons[4] is the ready button
				if(button != buttons[2])
					button.interactable = false;
			}
			buttons[2].GetComponentInChildren<Text>().text = "Cancel";
			playersReady++;
			if(playersReady >= HeroManager.Instance.numPlayers)
			{
				LoadingScreen.Instance.DisplayLoadingScreen(LoadingScreen.ScreenState.Game);
				playersReady = 0;
			}
		}
		isReady = !isReady;
	}
	private void Begin()
	{
		LevelLoader.Instance.LoadLevel(LevelLoader.Instance.currentLevelName);
	}
	private void SwitchButton(int playerID, float axis)
	{
		if (playerID != this.playerID || isReady)
			return;
		
		if (axis > 0)
		{
			if (timer > 0.2f)
			{
				index++;
				timer = 0.0f;
			}
		}
		if (axis < 0)
		{
			if (timer > 0.2f)
			{
				index--;
				timer = 0.0f;
			}
		}

		if (index < 0)
		{
			index = buttons.Length - 1;
		}
		if (index >= buttons.Length)
		{
			index = 0;
		}
		es.SetSelectedGameObject(buttons[index].gameObject);
	}
	private void Submit(int id)
	{
		if (id != playerID)
			return;

		buttons[index].onClick.Invoke();


	}

	private void Update()
	{
		timer += Time.deltaTime;
	}

	public void DisplayCharacter()
	{
		foreach (var hero in HeroManager.Instance.heroes)
		{
			var go = Instantiate(hero.prefab, heroPosition) as GameObject;
			go.SetActive(false);
			var h = go.GetComponentInChildren<Hero>();

			h.SetupSprite();
			h.stats.colourShifts = int.MaxValue;
			h.playerID = playerID;
			go.GetComponentInChildren<Rigidbody2D>().gravityScale = 0;
			go.transform.localPosition = new Vector3(hero.selectScreenPosition.x, 0.0f,0.0f);
			go.transform.DOScale(new Vector3(130.0f,130.0f,1.0f), 1.0f);
			go.transform.DORotate(new Vector3(0.0f,0.0f,360.0f), 1.0f, RotateMode.FastBeyond360);
			go.GetComponentInChildren<Canvas>().enabled = false;
			characters.Add(go);
			h.enabled = false;
		}
		currentHero = 0;
		characters[0].SetActive(true);
	}
	private void HeroEntry(GameObject obj, bool rotBackwards)
	{
		obj.transform.localScale = Vector3.zero;
		obj.transform.DOScale(new Vector3(130.0f,130.0f,1.0f), 1.0f);

		var rot = (rotBackwards) ? -360.0f : 360.0f;
			
		obj.transform.DORotate(new Vector3(0.0f,0.0f,rot), 1.0f, RotateMode.FastBeyond360);
	}
}

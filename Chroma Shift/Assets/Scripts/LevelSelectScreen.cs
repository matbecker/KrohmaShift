using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DG.DemiLib;

public class LevelSelectScreen : MonoBehaviour {

	[System.Serializable]
	public class LevelButton
	{
		public int id;
		public Button button;
		public Text levelName;
		public Text bestTime;
		public Image currentStarRating;
		public Image levelPicture;
		public string levelNameString;
		public bool isLocked;
	}
	public LevelButton[] levelButtons;
	//public Dictionary<string,float> levelTimeDict;
	[SerializeField] Image lockImage;
	[SerializeField] Color backgroundColor;
	[SerializeField] Color[] starColours;
	[SerializeField] Sprite[] levelPictures;
	[SerializeField] Image[] imagePanels;


	private static LevelSelectScreen instance;
	public static LevelSelectScreen Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(LevelSelectScreen)) as LevelSelectScreen;

			return instance;
		}
	}

	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < imagePanels.Length - 1; i++)
		{
			imagePanels[i].transform.DOScaleY(0.0f,0.0f);
		}
		imagePanels[5].DOColor(Color.clear,0.0f);

	}

	// Update is called once per frame
	void Update () {
	
	}
	public void SelectLevel(int index)
	{
		if (levelButtons[index].isLocked)
		{
			return;
		}
		if (levelButtons[index].id == index && !levelButtons[index].isLocked)
		{
			LevelLoader.Instance.currentLevelName = levelButtons[index].levelNameString;
			LevelLoader.Instance.currentLevelId = index;
			MainMenu.Instance.toCharacterSelect = true;
			LoadingScreen.Instance.DisplayLoadingScreen(LoadingScreen.ScreenState.CharacterSelect);

		}
		else
		{
			Debug.LogError("Error in LevelSelectScreen class. The Level ID does not match the level index");
		}
	}
	public void SetLevelButtons()
	{
		foreach (LevelButton button in levelButtons)
		{
			button.levelName.text = button.levelNameString;
			button.currentStarRating.color = starColours[0];

			if (button.isLocked)
			{
				Locked(button.id);
			}
			else
			{
				button.levelPicture.sprite = levelPictures[button.id];

				if (!LevelManager.Instance.levelTimeDict.ContainsKey(button.levelNameString))
					button.bestTime.text = "0.0";
				else
					button.bestTime.text = LevelManager.Instance.levelTimeDict[button.levelNameString].ToString("F2");

				var time = float.Parse(button.bestTime.text);
				var level = LevelManager.Instance.levels[button.id];
				time = level.rankTimes[3] - time;

				if (time < level.rankTimes[0] && time > 0)
				{
					button.currentStarRating.color = starColours[3];
				}
				else if (time < level.rankTimes[1] && time > level.rankTimes[0])
				{
					button.currentStarRating.color = starColours[2];
				}
				else if (time < level.rankTimes[2] && time > level.rankTimes[1])
				{
					button.currentStarRating.color = starColours[1];
				}
				else if (time < level.rankTimes[3] && time > level.rankTimes[2])
				{
					button.currentStarRating.color = starColours[0];
				}
			}
		}
	}
	private void Locked(int index)
	{
		var levelButton = levelButtons[index];
		levelButton.button.animator.SetBool("Locked", true);
		levelButton.bestTime.text = "--:--";
		levelButton.currentStarRating.color = Color.clear;
		levelButton.levelName.text = "???";
		levelButton.levelPicture.sprite = lockImage.sprite;
	}
	public void OpenLevelPanel()
	{
		imagePanels[0].rectTransform.DORotate(Vector3.zero,0.0f,RotateMode.Fast);
		imagePanels[5].DOColor(backgroundColor,1.0f);
		imagePanels[0].rectTransform.DOScaleY(1.0f,0.5f).SetEase(Ease.OutBounce, 1.0f).OnComplete(() => 
		{
			imagePanels[1].rectTransform.DOScaleY(1.0f,0.5f).SetEase(Ease.OutBack, 1.0f).OnComplete(() => 
			{
				imagePanels[2].rectTransform.DOScaleY(1.0f,0.5f).SetEase(Ease.OutBack, 1.0f).OnComplete(() => 
				{
					imagePanels[3].rectTransform.DOScaleY(1.0f,0.5f).SetEase(Ease.OutBack, 1.0f).OnComplete(() => 
					{
						imagePanels[4].rectTransform.DOScaleY(1.0f,0.5f).SetEase(Ease.OutBack, 1.0f).OnComplete(() => 
						{
							MainMenu.Instance.freezeEventSystem = false;
							MainMenu.Instance.SetLevelSelectScreenButton();
						});
					});
				});
			});
		});
	}
	public void CloseLevelPanel()
	{
		MainMenu.Instance.freezeEventSystem = true;
		imagePanels[4].rectTransform.DOScaleY(0.0f,0.5f).SetEase(Ease.InBack,1.0f).OnComplete(() => 
		{
			imagePanels[3].rectTransform.DOScaleY(0.0f,0.5f).SetEase(Ease.Linear).OnComplete(() => 
			{
				imagePanels[2].rectTransform.DOScaleY(0.0f,0.5f).SetEase(Ease.Linear).OnComplete(() => 
				{
					imagePanels[1].rectTransform.DOScaleY(0.0f,0.5f).SetEase(Ease.Linear).OnComplete(() => 
					{
						imagePanels[0].rectTransform.DOScaleY(0.0f,0.5f).SetDelay(0.25f);
						imagePanels[0].rectTransform.DORotate(new Vector3(180.0f,0.0f,0.0f),0.5f,RotateMode.Fast).OnComplete(() =>
						{
							imagePanels[0].rectTransform.DORotate(new Vector3(-180.0f,0.0f,0.0f),0.5f,RotateMode.Fast).SetDelay(0.25f);
							imagePanels[5].DOColor(Color.clear,1.0f);
							MainMenu.Instance.SwitchScreen(0);
							MainMenu.Instance.freezeEventSystem = false;
						});
					});
				});
			});
		});
	}
}

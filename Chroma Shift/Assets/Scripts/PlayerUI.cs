using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;

public class PlayerUI : Photon.MonoBehaviour 
{
	public HeroContainer[] hc;

	private static PlayerUI instance;
	public static PlayerUI Instance
	{
		get
		{
			if (!instance) 
				instance = GameObject.FindObjectOfType (typeof(PlayerUI)) as PlayerUI;

			return instance;
		}

	}

	[SerializeField] Text levelTimeTextTop;
	[SerializeField] Text levelTimeTextBottom;
	[SerializeField] StarBehaviour star;
	[SerializeField] LargeGradient largeGradient;
	[Range(0.0f, 60.0f)] [Tooltip("Establishes the length of time to transition over a colour")]
	[SerializeField] float transitionTime;
	private float transitionStep;
	private float transitionTotal;
	private float transitionLast;
	private Color titleColourStart;
	private Color titleColourEnd;
	public Animator timerAnim;
	public Text currentEnemiesTop;
	public Text currentEnemiesBottom;
	private bool atEnd;
	public Canvas canvas;
	[SerializeField] List<GameObject> uiObjects;

	void OnHeroSpawned(Hero hero) 
	{
		
		hc[hero.playerID].hero = hero;
		hc[hero.playerID].heroIcon.sprite = HeroManager.Instance.heroes[HeroManager.Instance.heroIndex[hero.playerID]].heroIcon;
		SetHealthBarColour(hero, Color.black, true);
		SetHealthBarGlow(hero);
		SetColourShifts(hero);


		currentEnemiesTop.color = Color.clear;
		currentEnemiesBottom.color = Color.clear;
		star.NewLevel(LevelManager.Instance.levelTimer);
		timerAnim.SetBool("finish", false);

		SetHeroImage(hero, HeroManager.Instance.heroes[HeroManager.Instance.heroIndex[hero.playerID]].heroIcon);

		hc[hero.playerID].gameObject.SetActive(true);
	}

	void Awake() 
	{
		LevelManager.Instance.OnHeroSpawned += OnHeroSpawned;

		for (int i = 0; i < hc.Length; i++)
		{
			hc[i].gameObject.SetActive(false);
		}
	}

	// Use this for initialization
	void Start () 
	{
		canvas.sortingOrder = 10;
		uiObjects = new List<GameObject>();
		LevelManager.Instance.Restart += RestartLevel;
	}

	void OnDestroy()
	{
		if (LevelManager.Instance)
			LevelManager.Instance.OnHeroSpawned -= OnHeroSpawned;
	}

	// Update is called once per frame
	void Update () 
	{
		levelTimeTextBottom.text = LevelManager.Instance.levelTimer.ToString("F2");
		levelTimeTextTop.text = levelTimeTextBottom.text;
	}
	public void SetColourShifts(Hero hero)
	{
		
		switch (hero.stats.colourShifts)
		{
		case 0:
			hc[hero.playerID].colourShifts[0].SetActive(false);
			hc[hero.playerID].colourShifts[1].SetActive(false);
			hc[hero.playerID].colourShifts[2].SetActive(false);
			hc[hero.playerID].colourShifts[3].SetActive(false);
			hc[hero.playerID].colourShifts[4].SetActive(false);
			break;
		case 1:
			hc[hero.playerID].colourShifts[0].SetActive(true);
			hc[hero.playerID].colourShifts[1].SetActive(false);
			hc[hero.playerID].colourShifts[2].SetActive(false);
			hc[hero.playerID].colourShifts[3].SetActive(false);
			hc[hero.playerID].colourShifts[4].SetActive(false);
			break;
		case 2:
			hc[hero.playerID].colourShifts[0].SetActive(true);
			hc[hero.playerID].colourShifts[1].SetActive(true);
			hc[hero.playerID].colourShifts[2].SetActive(false);
			hc[hero.playerID].colourShifts[3].SetActive(false);
			hc[hero.playerID].colourShifts[4].SetActive(false);
			break;
		case 3:
			hc[hero.playerID].colourShifts[0].SetActive(true);
			hc[hero.playerID].colourShifts[1].SetActive(true);
			hc[hero.playerID].colourShifts[2].SetActive(true);
			hc[hero.playerID].colourShifts[3].SetActive(false);
			hc[hero.playerID].colourShifts[4].SetActive(false);
			break;
		case 4:
			hc[hero.playerID].colourShifts[0].SetActive(true);
			hc[hero.playerID].colourShifts[1].SetActive(true);
			hc[hero.playerID].colourShifts[2].SetActive(true);
			hc[hero.playerID].colourShifts[3].SetActive(true);
			hc[hero.playerID].colourShifts[4].SetActive(false);
			break;
		case 5:
			hc[hero.playerID].colourShifts[0].SetActive(true);
			hc[hero.playerID].colourShifts[1].SetActive(true);
			hc[hero.playerID].colourShifts[2].SetActive(true);
			hc[hero.playerID].colourShifts[3].SetActive(true);
			hc[hero.playerID].colourShifts[4].SetActive(true);
			break;
		default:
			break;
		}
	}
	public void SetHealthBarGlow(Hero hero)
	{
		for (int i =0; i < hc[hero.playerID].healthBarGlow.Length; i++)
			hc[hero.playerID].healthBarGlow[i].color = hero.colour.colors[(int)hero.colour.currentColourType].color[i];
	}
	public void SetHealthBarColour(Hero hero, Color color, bool atStart)
	{
		var a = (atStart) ? hero.stats.maxHealth / hero.stats.maxHealth : (float)hero.stats.currentHealth / (float)hero.stats.maxHealth;
		hc[hero.playerID].healthBar.color =  new Color(color.r,color.g, color.b, a);
	}
	private void RestartLevel()
	{
		TimerStart();
	}
	public void TimerStop()
	{
		timerAnim.SetBool("finish", true);
	}
	public void TimerStart()
	{
		timerAnim.SetBool("finish", false);
	}
	public void TimerEnd()
	{
		atEnd = true;
	}
	public void TimerFlash()
	{
		timerAnim.SetTrigger("flash");
	}
	private void TransitionTextColour()
	{
		transitionStep -= transitionTotal;
		float rand = Random.value;
		transitionTotal = (rand - transitionLast) * transitionTime;

		titleColourStart = titleColourEnd;
		titleColourEnd = largeGradient.Evaluate(rand);
	}
	public void SetHeroImage(Hero hero, Sprite sprite)
	{
		hc[hero.playerID].heroIcon.sprite = sprite;
	}

}

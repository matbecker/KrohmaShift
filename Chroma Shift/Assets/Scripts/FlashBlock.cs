using UnityEngine;
using System.Collections;
using DG.Tweening;
using DG.DemiLib;
using System.Linq;

public class FlashBlock : LevelObject {

	[SerializeField] ColourManager cm;
	[SerializeField] float transitionDuration;
	[SerializeField] SpriteRenderer sprite;
	private Coroutine cor;
	private bool collided;
	private int randColour;
	private int randShade;
	public ColourManager.ColourType TopColour;
	public ColourManager.ColourType BottomColour;
	private bool isWhite;
	private bool changeColour;
	private float timer;

	// Use this for initialization
	void Start () 
	{
		if (!inEditor)
		{
			ColourWheel.Instance.ColourTop += ColourTop;
			ColourWheel.Instance.ColourBottom += ColourBottom;
		}
		changeColour = true;
	}


	// Update is called once per frame
	void Update () 
	{
		if (collided && changeColour && !ColourWheel.Instance.startSpinning)
		{
			var colour = cm.GetRandomColor(cm.currentColourType);

//			//if the block is the same colour as the top of the background in the higher portion of the level
//			if (transform.position.y > 0 && cm.currentColourType == TopColour)
//			{
//				Disappear();
//			}
//			//if the block is the same colour as the bottom of the background in the lower portion of the level
//			else if (transform.position.y < 0 && cm.currentColourType == BottomColour)
//			{
//				Disappear();
//			}
			sprite.DOColor(colour, transitionDuration).OnComplete(() => {
				if (cm.currentColourType == TopColour || cm.currentColourType == BottomColour)
				{
					changeColour = false;
					Disappear();
					return;					
				}
				changeColour = true; 
			});
			changeColour = false;

		}
		if (ColourWheel.Instance.startSpinning && !isWhite)
		{
			sprite.DOColor(Color.white, transitionDuration);
			isWhite = true;
		} else {
			isWhite = false;
		}

	}
	private void OnCollisionEnter2D(Collision2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();

		if (hero != null)
		{
			collided = true;
			//change colour
//			sprite.DOColor(cm.GetRandomColor(cm.currentColourType),transitionDuration);
		}
	}
	private void OnCollisionExit2D(Collision2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();

		if (hero != null)
		{
			collided = false;
		}
	}
	public override string GetSaveString ()
	{
		return string.Join(SPLIT_CHAR.ToString(), new []{objectID, transform.position.x, transform.position.y, transform.position.z}.Select(s => s.ToString()).ToArray());
	}
	public override void LoadSaveData (string input)
	{
		var data = input.Split(SPLIT_CHAR);

		transform.position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
	}
	private void ColourTop(ColourManager.ColourType colour)
	{
		TopColour = colour;
	}
	private void ColourBottom(ColourManager.ColourType colour)
	{
		BottomColour = colour;
	}
	private void Disappear()
	{
		timer = 0.0f;
		sprite.DOColor(Color.clear, transitionDuration).SetEase(Ease.InOutFlash, 7, 0).OnComplete(() => {
			bc.enabled = false;
			collided = false;
			Reappear();
		});
	}
	private void Reappear()
	{
		sprite.DOColor(cm.GetRandomColor(cm.currentColourType),transitionDuration)
			.SetDelay(2.0f)
			.SetEase(Ease.InOutFlash, 7, 0)
			.OnComplete(() => {
				changeColour = true; 
				bc.enabled = true;
			});
	}
}

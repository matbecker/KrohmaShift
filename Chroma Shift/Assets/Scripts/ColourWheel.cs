using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;

public class ColourWheel : MonoBehaviour {

	//public enum ColourType { Purple, Blue, Green, Yellow, Orange, Red };
	public ColourManager.ColourType currentColourTop;
	public ColourManager.ColourType currentColourBottom;

	private static ColourWheel instance;
	public static ColourWheel Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(ColourWheel)) as ColourWheel;

			return instance;
		}
	}

	[SerializeField] List<Material> colourWheelFaceColours;
	[SerializeField] int faceIndex;
	[SerializeField] int randDirection;
	[SerializeField] float rotationSpeed;
	[SerializeField] float spinTime;
	[SerializeField] float rotationAngle;
	private float timer;
	public bool startSpinning;
	private string[] colors;
	private bool shake;
	private float shakeTimer;

	public delegate void ColourEvent(ColourManager.ColourType colour);
	public event ColourEvent ColourTop;
	public event ColourEvent ColourBottom;

	// Use this for initialization
	void Start () 
	{
		startSpinning = false;
		//shakeCor = null;

		colors = new string[2];

		rotationAngle = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (startSpinning)
		{
			timer += Time.deltaTime;

			if (timer >= spinTime)
			{
				SlowDown();
			}
			Spin();
		}
	}
	void Spin()
	{
		//spin right
		if (randDirection == 0)
			rotationAngle += rotationSpeed * Time.deltaTime;
		else //spin left
			rotationAngle -= rotationSpeed * Time.deltaTime;

		transform.localRotation = Quaternion.Euler(rotationAngle, 0, 0);
	}
	void SlowDown()
	{
		if (Mathf.Round(rotationAngle) % 30 == 0)
			StopSpinning();
	}
	private void StopSpinning()
	{
		while (rotationAngle < 0) 
			rotationAngle += 360;
		while (rotationAngle > 360)
			rotationAngle -= 360;
		
		rotationAngle = (float)Mathf.RoundToInt(rotationAngle);

		faceIndex = (int)(rotationAngle / 30f);

		if (faceIndex == 12)
			faceIndex = 0;
		
		colors = colourWheelFaceColours[faceIndex].name.Split('_');

		currentColourTop = ParseColour(colors[0]);
		currentColourBottom = ParseColour(colors[1]);

		if (ColourTop != null)
		{
			switch(currentColourTop)
			{
			case ColourManager.ColourType.Purple:
				ColourTop(currentColourTop);
				break;
			case ColourManager.ColourType.Blue:
				ColourTop(currentColourTop);
				break;
			case ColourManager.ColourType.Green:
				ColourTop(currentColourTop);				
				break;
			case ColourManager.ColourType.Yellow:
				ColourTop(currentColourTop);
				break;
			case ColourManager.ColourType.Orange:
				ColourTop(currentColourTop);
				break;
			case ColourManager.ColourType.Red:
				ColourTop(currentColourTop);
				break;
			}
		}
		if (ColourBottom != null)
		{
			switch(currentColourBottom)
			{
			case ColourManager.ColourType.Purple:
				ColourBottom(currentColourBottom);
				break;
			case ColourManager.ColourType.Blue:
				ColourBottom(currentColourBottom);
				break;
			case ColourManager.ColourType.Green:
				ColourBottom(currentColourBottom);
				break;
			case ColourManager.ColourType.Yellow:
				ColourBottom(currentColourBottom);
				break;
			case ColourManager.ColourType.Orange:
				ColourBottom(currentColourBottom);
				break;
			case ColourManager.ColourType.Red:
				ColourBottom(currentColourBottom);
				break;
			}
		}
			

//
//		Debug.Log(currentColourTop.ToString());
//		Debug.Log(currentColourBottom.ToString());
		CameraBehaviour.Instance.Shake(0.5f,0.0f,1.0f,true);
		//reset variables
		rotationSpeed = 0;
		startSpinning = false;
		shake = false;
		timer = 0.0f;
	}
	public void Shift()
	{
		CameraBehaviour.Instance.Shake(0.5f,0.0f,1.0f, false);
		//coin flip to determine if the wheel should spin right or left
		randDirection = UnityEngine.Random.Range(0,2);
		//wheel will spin at random speed between 50 and 100
		rotationSpeed = UnityEngine.Random.Range(50.0f, 100.0f);
		//spin for 5 to 10 seconds
		spinTime = UnityEngine.Random.Range(2.0f,5.0f);
		CameraBehaviour.Instance.Shake(spinTime - 0.5f,0.0f,0.2f,false);
		//start spinning the wheel
		startSpinning = true;
	}
	private ColourManager.ColourType ParseColour(string colour)
	{
		return (ColourManager.ColourType)Enum.Parse(typeof(ColourManager.ColourType), colour);
	}
}

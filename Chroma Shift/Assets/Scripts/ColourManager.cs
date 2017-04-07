using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ColourManager : MonoBehaviour {

	//enum for each different colour type
	public enum ColourType { Purple, Blue, Green, Yellow, Orange, Red };
	public ColourType currentColourType = ColourType.Purple;

	//micro class that holds colourType information and all the colours associated with each type
	[System.Serializable]
	public class ColorContainer
	{
		public ColourType type;
		public Color[] color;
	}
	//Container that has all the colors in it
	public ColorContainer[] colors;
	//index for the current shade
	public int shadeIndex;
	// Use this for initialization
	void Start () 
	{
	}

	public void NextColour()
	{
		int a = (int)currentColourType;
		//shift to the next color in the enum
		int b = (int)++currentColourType;
		//if we are at the end of the enum reset the values
		if(b >= colors.Length)
			b = 0;
		
		currentColourType = (ColourType)b;
	}

	public void NextShade()
	{
		//increase the shade index
		shadeIndex++;
	}

	public Color GetCurrentColor()
	{
		var a = (int)currentColourType;
		var b = shadeIndex % colors[a].color.Length;
		//c = the current shade of the current color 
		var c = colors[a].color[b];
		//always ensure the alpha is full
		c.a = 1;
		return c;
	}
	public Color GetRandomColor(ColourType otherColour)
	{
		currentColourType = (ColourType)UnityEngine.Random.Range(0, 6);
		shadeIndex = UnityEngine.Random.Range(0, 6);
		var newColour = colors[(int)currentColourType];

		while(newColour.type == otherColour)
		{
			currentColourType = (ColourType)UnityEngine.Random.Range(0, 6);
			newColour = colors[(int)currentColourType];
		}
		return newColour.color[shadeIndex];
	}

}

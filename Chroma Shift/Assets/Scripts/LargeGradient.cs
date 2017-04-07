using UnityEngine;
using System.Collections;

[System.Serializable]
public class LargeGradient {

	public Color[] colourArray;

	public Color Evaluate(float percent)
	{
		int lowIndex = (int)Mathf.Floor((colourArray.Length  - 1) * percent);
		Color lowColour = colourArray[lowIndex];

		int highIndex = (int)Mathf.Ceil((colourArray.Length - 1) * percent);
		Color highColour = colourArray[highIndex];

		float p = (colourArray.Length - 1) * percent - lowIndex;

		return Color.Lerp(lowColour, highColour, p);
	}
}

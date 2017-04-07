using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColourTransition {

	public float startOfLife;
	public float duration;
	public Gradient gradient;

	public void Start()
	{
		startOfLife = Time.time;
	}

	public Color Evaluate()
	{
		float percent = (Time.time - startOfLife) / duration;

		return gradient.Evaluate(percent);
	}
}

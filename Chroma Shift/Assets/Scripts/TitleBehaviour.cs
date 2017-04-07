using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TitleBehaviour : MonoBehaviour {

	[SerializeField] Animator shiftTitleAnimator;
	[SerializeField] Vector2[] shiftTitleTopPositions;
	[SerializeField] Text shiftTitleTop;
	[SerializeField] Text shiftTitleBottom;
	[SerializeField] Text krohmaTitleTop;
	[SerializeField] ColourManager colour;
	[SerializeField] LargeGradient largeGradient;
	[Range(0.0f, 30.0f)] [Tooltip("Establishes the length of time to transition over a colour")]
	[SerializeField] float transitionTime;
	private float transitionStep;
	private float transitionTotal;
	private float transitionLast;
	private Color titleColourStart;
	private Color titleColourEnd;
	private Color inverseTitleColourStart;
	private Color inverseTitleColourEnd;
	private Vector2 startPos;
	private bool slide;
	private bool shift;
	private int index;



	// Use this for initialization
	void Start () 
	{
		TransitionTextColour();
		TransitionTextColour();
	}
	
	// Update is called once per frame
	void Update () 
	{
		transitionStep += Time.deltaTime;
		if (transitionStep >= transitionTotal)
		{
			TransitionTextColour();
		}
		krohmaTitleTop.color = Color.Lerp(titleColourStart, titleColourEnd, transitionStep / transitionTotal);
		shiftTitleBottom.color = Color.Lerp(inverseTitleColourStart, inverseTitleColourEnd, transitionStep / transitionTotal);

		int rand = Random.Range(0, 200);
		if (rand == 7)
			slide = !slide;

		if (rand == 6)
		{
			shift = true;
			index = Random.Range(0, shiftTitleTopPositions.Length);
		}
		if (shift)
		{
			shiftTitleTop.rectTransform.localPosition = Vector3.Lerp(shiftTitleTop.rectTransform.localPosition, shiftTitleTopPositions[index], Time.deltaTime * 2.0f);

			var dir = shiftTitleTop.rectTransform.localPosition - (Vector3)shiftTitleTopPositions[index];

			var dist = dir.magnitude;

			if (dist < 0.5f)
			{
				shift = false;
			}
		}	
		shiftTitleAnimator.SetBool("slide", slide);
	}
	private void TransitionTextColour()
	{
		transitionStep -= transitionTotal;
		float rand = Random.value;
		transitionTotal = (rand - transitionLast) * transitionTime;

		titleColourStart = titleColourEnd;
		inverseTitleColourStart = inverseTitleColourEnd;
		titleColourEnd = largeGradient.Evaluate(rand);
		inverseTitleColourEnd = largeGradient.Evaluate((rand + 0.5f > 1.0f) ? rand - 0.5f : rand + 0.5f);
	}
}

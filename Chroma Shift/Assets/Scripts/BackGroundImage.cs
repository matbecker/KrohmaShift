using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BackGroundImage : MonoBehaviour{

	[SerializeField] Image effectedImage;
	[SerializeField] float startingAlpha;
	// Use this for initialization
	void Start () 
	{
		effectedImage.CrossFadeAlpha(startingAlpha, 0.5f, false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	public void TransitionImage(float desiredAmount)
	{
		effectedImage.CrossFadeAlpha(desiredAmount, 0.5f, false);
		//effectedImage.color = Color.black;
	}
		
}

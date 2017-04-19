using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

	[SerializeField] SpriteRenderer sr;
	// Use this for initialization
	void Start () 
	{
		SwitchColour ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SwitchColour()
	{
		sr.color = new Color(Random.value, Random.value, Random.value, 1.0f);
	}
}

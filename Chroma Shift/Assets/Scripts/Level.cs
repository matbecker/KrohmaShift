using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	public int id;
	public float[] rankTimes;
	public string name;

	private static Level instance;
	public static Level Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(Level)) as Level;

			return instance;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

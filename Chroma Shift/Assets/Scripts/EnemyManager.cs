using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	private static EnemyManager instance;
	public static EnemyManager Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(EnemyManager)) as EnemyManager;

			return instance;
		}
	}

	public GameObject[] enemyPrefab;
	public int enemyCount;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnerManager : MonoBehaviour {

	[SerializeField] List<GameObject> spawners;
	// Use this for initialization
	void Start () 
	{
		spawners = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

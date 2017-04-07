using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroManager : MonoBehaviour {

	public enum HeroType { Swordsmen, Archer, Ninja, Wizard };

	[System.Serializable]
	public class Heroes
	{
		public HeroType type;
		public GameObject prefab;
		public Vector3 selectScreenPosition;
		public Sprite heroIcon;
	}
	//container for Heroes class with their type and prefab
	public Heroes[] heroes;

	public GameObject GetCurrentHeroPrefab(int playerID) 
	{
		return heroes[heroIndex[playerID]].prefab;
	}

	public int[] heroIndex;
	public ColourManager.ColourType[] currentColorType;
	public int[] currentShadeIndex;
	public int numPlayers { get {return Input.GetJoystickNames().Length - deadPlayers; } }
	public int deadPlayers;

	private static HeroManager instance;
	public static HeroManager Instance
	{
		get 
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(HeroManager)) as HeroManager;

			return instance;
		}
	}
	void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
		heroIndex = new int[2];
		currentColorType = new ColourManager.ColourType[2];
		currentShadeIndex = new int[2];
	}
}

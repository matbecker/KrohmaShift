using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.DemiLib;
using DG.Tweening;

public class SoundManager : MonoBehaviour {

	private static SoundManager instance;
	public static SoundManager Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;

			return instance;
		}
	}

	[System.Serializable]
	public class Sound
	{
		public string name;
		public AudioClip sound;
	}

	[System.Serializable]
	public class Song
	{
		public string name;
		public AudioClip song;
	}
	public Sound[] sounds;
	public Song[] songs;

	public Dictionary<string, AudioClip> soundEffectDict;
	public Dictionary<string, AudioClip> songDict;

	[SerializeField] AudioSource musicSource;
	[SerializeField] AudioSource sfxSource;

	public void PlaySound(string name, float volume)
	{
		sfxSource.clip = soundEffectDict[name];
		sfxSource.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
		sfxSource.Play();
	}
	public void PlaySong(string name, float volume, bool isRepeating)
	{
		musicSource.clip = songDict[name];
		musicSource.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
		musicSource.Play();
		musicSource.loop = (isRepeating) ? true : false;
	}
	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
			return;
		}

		soundEffectDict = new Dictionary<string, AudioClip>();
		songDict = new Dictionary<string, AudioClip>();

		foreach (Sound s in sounds)
		{
			if (!soundEffectDict.ContainsValue(s.sound))
			{
				soundEffectDict.Add(s.name, s.sound);
			}
		}

		foreach (Song s in songs)
		{
			if (!songDict.ContainsValue(s.song))
			{
				songDict.Add(s.name, s.song);
			}
		}
		PlaySong("Khromamenu", 1.0f, true);


	}
	public void TogglePause()
	{
		var volume = (musicSource.volume > 0.2f) ? 0.2f : 1.0f;
		musicSource.DOFade(volume, 2.0f);
	}
	public void SceneSwitch()
	{
		musicSource.DOFade(0.0f, 0.5f);
	}
	public void SceneBegin()
	{
		musicSource.DOFade(1.0f, 5.0f);
	}
}

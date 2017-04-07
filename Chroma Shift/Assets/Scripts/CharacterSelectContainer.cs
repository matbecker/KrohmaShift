using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.DemiLib;
using UnityEngine.UI;

public class CharacterSelectContainer : MonoBehaviour {

	private static CharacterSelectContainer instance;
	public static CharacterSelectContainer Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(CharacterSelectContainer)) as CharacterSelectContainer;

			return instance;
		}
	}

	[SerializeField] Image banner;
	[SerializeField] CharacterSelectScreen player1;
	[SerializeField] CharacterSelectScreen player2;
	public int inputId;

	// Use this for initialization
	void Start () 
	{
		Camera.main.DOColor(Color.white, 1.0f).OnComplete(() => 
		{
			banner.rectTransform.DOScale(Vector3.one, 1.0f).SetEase(Ease.OutBack, 0.5f, 0.5f).OnComplete(() => 
			{
				player1.transform.DOScale(Vector3.one, 1.0f).SetEase(Ease.OutBounce,0.5f).OnComplete(() => 
				{
					player1.DisplayCharacter();
					if (Input.GetJoystickNames().Length > 1)
					{
						player2.transform.DOScale(Vector3.one, 1.0f).SetEase(Ease.OutBounce,1.0f).SetDelay(0.5f).OnComplete(() => 
						{
							player2.DisplayCharacter();
						});
					}
				});
			});
		});
	}
}

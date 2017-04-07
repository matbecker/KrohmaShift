using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIButton : MonoBehaviour {

	Tween tween;
	bool expanded;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void ToggleButtonSizeX(GameObject obj)
	{
		var size = (expanded) ? 1.0f : 1.1f;
		PlayTween(obj.transform.DOScaleX(size,0.5f));
		expanded = !expanded;
	}

	public void ToggleButtonSize(GameObject obj)
	{
		var size = (expanded) ? Vector3.one : Vector3.one * 1.1f;
		PlayTween(obj.transform.DOScale(size, 0.5f));
		expanded = !expanded;
	}

	public void ButtonClick(GameObject obj)
	{
		PlayTween(obj.transform.DOScale(Vector3.one * 0.9f, 0.25f).SetLoops(2, LoopType.Yoyo));
	}

	public void BumpPositionX(GameObject obj)
	{
		var rect = obj.GetComponent<RectTransform>();
		var direction = (rect.localPosition.x > 0) ? 5.0f : -5.0f;

		PlayTween(rect.transform.DOLocalMoveX(rect.localPosition.x + direction, 0.25f, false).SetLoops(2, LoopType.Yoyo));
	}
	public void PlayTween(Tween t)
	{
		if (tween != null)
		{
			tween.Kill(true);
		}
		tween = t;

		t.OnComplete(() => tween = null);
	}
}

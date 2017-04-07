using UnityEngine;
using System.Collections;
using System.Linq;
using DG.DemiLib;
using DG.Tweening;

public class GroundBlock : LevelObject {

	[SerializeField] float duration;
	[SerializeField] SpriteRenderer sprite;
	// Use this for initialization
	void Start () 
	{
		sprite = gameObject.GetComponent<SpriteRenderer>();
	}
	void OnCollisionEnter2D(Collision2D other)
	{
		sprite.DOColor(Color.white, duration);
	}
	void OnCollisionExit2D(Collision2D other)
	{
		sprite.DOColor(Color.black, duration);
	}
	public override string GetSaveString ()
	{
		return string.Join(SPLIT_CHAR.ToString(), new []{objectID, transform.position.x, transform.position.y, transform.position.z}.Select(s => s.ToString()).ToArray());
	}
	public override void LoadSaveData (string input)
	{
		var data = input.Split(SPLIT_CHAR);

		transform.position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
	}

}

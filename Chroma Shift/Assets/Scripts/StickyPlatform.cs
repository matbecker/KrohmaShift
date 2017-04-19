using UnityEngine;
using System.Collections;
using System.Linq;
using DG.DemiLib;
using DG.Tweening;

public class StickyPlatform : LevelObject {

	[SerializeField] float duration = 1.0f;
	[SerializeField] SpriteRenderer sprite;
	private bool hasCollided;
	private float timer;

	private void OnCollisionEnter2D(Collision2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();

		if (hero != null)
		{
			var rb = hero.GetComponent<Rigidbody2D>();

			if (rb.gravityScale == -1)
				rb.velocity = Vector2.zero;
			
			hero.DisableGravity();
		}
	}
	private void OnCollisionExit2D(Collision2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();

		if (hero != null)
		{
			hasCollided = false;
			hero.EnableGravity();
		}
	}

	public override string GetSaveString ()
	{
		return string.Join(SPLIT_CHAR.ToString(), new []{objectID, transform.position.x, transform.position.y, transform.position.z, transform.localScale.x, transform.localScale.y, duration}.Select(s => s.ToString()).ToArray());
	}
	public override void LoadSaveData (string input)
	{
		var data = input.Split(SPLIT_CHAR);

		transform.position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));

		transform.localScale = new Vector3(float.Parse(data[4]), float.Parse(data[5]), transform.localScale.z);

		duration = float.Parse(data[6]);
	}
}

using UnityEngine;
using System.Collections;
using System.Linq;
using DG.DemiLib;
using DG.Tweening;

public class AntiGravityBlock : LevelObject {

	[SerializeField] Color grayTransparent;
	[SerializeField] SpriteRenderer sprite;
	private Hero h;
	private float timer;
	private bool flipped;
	private bool collided;

	private void FlipGravity(Rigidbody2D rb)
	{
		rb.gravityScale *= -1;
	}
	private void OnCollisionEnter2D(Collision2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();

		if (hero != null)
		{
			h = hero;
			h.sprite.transform.DOShakePosition(1.0f,0.1f,0,0,false).SetEase(Ease.Linear);
			//hero.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			sprite.DOColor(grayTransparent, 1.0f).SetEase(Ease.InOutFlash, 5, 0).OnComplete(() => {
				bc.enabled = false;
				flipped = true;
				h.sprite.transform.position = h.transform.position;
			});
		}
	}
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (h != null)
		{
			var rb = h.GetComponent<Rigidbody2D>();
			rb.velocity = Vector2.zero;
			FlipGravity(rb);
		}
			
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (h != null)
		{
			sprite.DOColor(Color.black, 1.0f).OnComplete(() => {
				flipped = false;
				bc.enabled = true;
			});
		}
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
	void OnDrawGizmos()
	{
		if (inEditor)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(bc.bounds.center, bc.bounds.size);
		}
	}
}

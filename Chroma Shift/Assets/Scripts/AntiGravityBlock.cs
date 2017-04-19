using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.DemiLib;
using DG.Tweening;

public class AntiGravityBlock : LevelObject {

	[SerializeField] Color grayTransparent;
	[SerializeField] Animator anim;
	private List<Hero> heroes;
	private int heroCount;

	private void Start()
	{
		heroes = new List<Hero>();
	}
	public void FlipGravity()
	{

		foreach (Hero h in heroes) 
		{
			h.GetComponent<Rigidbody2D>().gravityScale *= -1;
		}

	}
	private void OnCollisionEnter2D(Collision2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();

		if (hero != null)
		{
			heroes.Add(hero);
			anim.SetTrigger ("flip");
		}
	}
	private void OnCollisionExit2D(Collision2D other)
	{
		var hero = other.gameObject.GetComponent<Hero>();

		if (hero != null)
		{
			heroes.Remove (hero);
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

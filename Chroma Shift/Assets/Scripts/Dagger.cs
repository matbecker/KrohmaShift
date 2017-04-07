using UnityEngine;
using System.Collections;

public class Dagger : MonoBehaviour {

	[SerializeField] Hero ninja;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (ninja.isAttacking)
		{
			int damage = ninja.stats.attackPower;

			var enemy = other.GetComponent<Enemy>();
			if (enemy != null && HelperFunctions.IsSameColour(ninja.colour.currentColourType, enemy.colour.currentColourType))
				damage = 10;

			var dmg = other.GetComponent<IDamageable>();
			if(dmg != null) {
				dmg.Damage(damage);
			}
		}
	}
}

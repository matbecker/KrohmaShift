using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {

	[SerializeField] Hero swordsmen;

	void OnTriggerEnter2D(Collider2D other)
	{
		var damage = swordsmen.stats.attackPower;

		var enemy = other.GetComponent<Enemy>();
		if (enemy != null && HelperFunctions.IsSameColour(swordsmen.colour.currentColourType, enemy.colour.currentColourType))
			damage = 10;

		var dmg = other.GetComponent<IDamageable>();

		if(dmg != null) 
			dmg.Damage(damage);
		

	}
}

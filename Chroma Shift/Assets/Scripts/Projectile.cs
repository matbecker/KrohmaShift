using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public enum ProjectileType { Arrow, Magic };
	public ProjectileType type;

	public Rigidbody2D rb;
	[SerializeField] Vector2 force;
	[SerializeField] Vector2 acceleration;
	[SerializeField] Vector2 intialVelocity;
	public Hero hero;
	[SerializeField] ParticleSystem[] projectileFx;
	private Vector2 hitPoint;

	void Start() {
		projectileFx[(int)hero.colour.currentColourType].Play();
	}
	// Update is called once per frame
	void Update () 
	{

		switch (type)
		{
		case ProjectileType.Arrow:
			var angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
			angle = angle * Mathf.Rad2Deg;
			transform.localRotation = Quaternion.Euler(0, 0, angle);
			break;
		case ProjectileType.Magic:
			break;
		default:
			break;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<IProjectileIgnore>() != null)
			return;
		
		var damage = hero.stats.attackPower;

		var enemy = other.GetComponent<Enemy>();
		if (enemy != null && HelperFunctions.IsSameColour(hero.colour.currentColourType, enemy.colour.currentColourType))
			damage = 10;

		var dmg = other.GetComponent<IDamageable>();
		if(dmg != null) {
			dmg.Damage(damage);
		}
		Destroy(projectileFx[(int)hero.colour.currentColourType], 5.0f);
		projectileFx[(int)hero.colour.currentColourType].transform.parent = null;
		projectileFx[(int)hero.colour.currentColourType].Stop();
		Debug.Log(other.gameObject.name);
		Destroy(gameObject);
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotatingImage : MonoBehaviour {

	[SerializeField] Image image;
	[SerializeField] GameObject sprite;
	[SerializeField] bool isUI;
	private float rotationSpeed = 200.0f;
	// Use this for initialization
	void Start () 
	{
		if (isUI)
		{
			image = gameObject.GetComponent<Image>();
			image.color = new Color(Random.value, Random.value, Random.value, 1.0f);
		}
		else
		{
			sprite = gameObject;
			sprite.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value, 1.0f);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isUI)
		{
			image.rectTransform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));

			Vector3 euler = image.rectTransform.localRotation.eulerAngles;

			if (euler.y > 180)
			{
				euler.y -= 180;
				image.rectTransform.localRotation = Quaternion.Euler(euler);
				image.color = new Color(Random.value, Random.value, Random.value, 1.0f);
			}
		}
		else
		{
			sprite.transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));

			Vector3 euler = sprite.transform.localRotation.eulerAngles;

			if (euler.y > 180)
			{
				euler.y -= 180;
				sprite.transform.localRotation = Quaternion.Euler(euler);
				sprite.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value, 1.0f);
			}
		}
	}
}

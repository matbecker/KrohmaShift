using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StarBehaviour : MonoBehaviour {


	private static StarBehaviour instance;
	public static StarBehaviour Instance
	{
		get
		{
			if (!instance)
				instance = GameObject.FindObjectOfType(typeof(StarBehaviour)) as StarBehaviour;

			return instance;
		}
	}
	[SerializeField] Animator animator;
	[SerializeField] string[] trigNames;
	[SerializeField] Color[] starColours;
	[SerializeField] Image image;
	[SerializeField] Animator sparkleAnimator;
	[SerializeField] Image sparkleImage;
	[SerializeField] Vector3[] sparklePositions;
	[SerializeField] Gradient gradient;
	[SerializeField] Transition transition;
	private Coroutine delayCor;
	private float currentDelay;
	private float timer;
	private int animCounter;
	private bool start;
	private bool paused;
	// Use this for initialization
	void Start () 
	{
		delayCor = null;
		currentDelay = Random.Range(1.0f,3.0f);
		animCounter = 0;
		LevelManager.Instance.Restart += RestartLevel;
		start = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!paused)
		{
			if (start)
				image.color = gradient.Evaluate(transition.Evaluate());

			timer += Time.deltaTime;
			if (timer > currentDelay)
			{
				float rand = Random.Range(1.0f, 3.0f);

				if (delayCor == null)
					delayCor = StartCoroutine(DelayAnimCor(rand,animator));

				timer = 0.0f;
			}
			animCounter++;

			if (animCounter > 60)
			{
				MoveSparkle();
				animCounter = 0;
			}
		}
	}
	private IEnumerator DelayAnimCor(float duration, Animator anim)
	{
		int rand = Random.Range(0,trigNames.Length);
		anim.SetTrigger(trigNames[rand]);

		yield return new WaitForSeconds(duration);

		currentDelay = Random.Range(1.0f,3.0f);

		if (delayCor != null)
		{
			StopCoroutine(delayCor);
			delayCor = null;
		}

	}
	private void MoveSparkle()
	{
		int rand = Random.Range(0,sparklePositions.Length);
		Vector3 randPointInSphere = Random.insideUnitSphere * 5;

		sparkleImage.rectTransform.localPosition = new Vector3(sparklePositions[rand].x + randPointInSphere.x, sparklePositions[rand].y + randPointInSphere.y, sparklePositions[rand].z + randPointInSphere.z);

		sparkleAnimator.SetTrigger("twinkle");
	}
	public void NewLevel(float levelTime)
	{
		start = false;
		transition.Reset();
		transition.duration = levelTime;
		start = true;
	}
	public void ChangeDuration(float value)
	{
		transition.duration -= value;
	}
	public void SetPaused(bool paused)
	{
		animator.SetBool("pause", paused);

		this.paused = paused;
	}
	private void RestartLevel()
	{
		NewLevel(LevelManager.Instance.GetLevelTime());
	}
}

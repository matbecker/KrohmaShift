using UnityEngine;
using System.Collections;

[System.Serializable]
public class Transition {

	public enum State { Linear, Loop, PingPong };
	public State currentState;

	[Range(0.0f,120.0f)]
	public float duration;
	//public float transitionStart;
	private float start;

	public float Evaluate()
	{
		float percentage = 0;

		if (start == 0)
		{
			start = Time.time;
		}

		float deltaTime = Time.time - start;

		if (currentState != State.PingPong)
		{
			if (currentState == State.Loop)
			{
				percentage = (deltaTime % duration) / duration;
			}
			if (currentState == State.Linear)
			{
				percentage = deltaTime / duration;
				percentage = Mathf.Min(percentage, 1.0f);
			}
		}
		else
		{
			percentage = (deltaTime > duration) ? 1 - (deltaTime - duration) / duration : deltaTime / duration;
		}
		return percentage;
	}
	public void Reset()
	{
		duration = 0.0f;
		start = 0.0f;
	}


}

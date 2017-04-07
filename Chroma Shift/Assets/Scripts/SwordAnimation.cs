using UnityEngine;
using System.Collections;

public class SwordAnimation : MonoBehaviour {

	[SerializeField] BoxCollider2D aoeTrigger;
	[SerializeField] ParticleSystem[] swordFx;
	[SerializeField] Hero swordsmen;
	private bool startAOE;
		
	// Use this for initialization
	void Start () 
	{
		swordsmen = GetComponentInParent<Swordsmen>();
		aoeTrigger.enabled = false;
		swordFx[(int)swordsmen.colour.currentColourType].Stop();
	}
	public void StartAreaOfEffect()
	{
		CameraBehaviour.Instance.Shake(0.2f,0.2f,0.2f, true);
		startAOE = true;
		swordFx[(int)swordsmen.colour.currentColourType].Play();	
		aoeTrigger.enabled = true;
	}

	public void StopAreaOfEffect()
	{
		startAOE = false;
		aoeTrigger.enabled = false;
		swordFx[(int)swordsmen.colour.currentColourType].Stop();
	}
}

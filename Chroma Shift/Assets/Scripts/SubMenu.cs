using UnityEngine;
using System.Collections;

public class SubMenu : MonoBehaviour {

	[SerializeField] Animator titleAnim;

	public void ShowTitle()
	{
		titleAnim.SetTrigger("appear");
	}
	public void HideTitle()
	{
		titleAnim.SetTrigger("dissappear");
	}
}

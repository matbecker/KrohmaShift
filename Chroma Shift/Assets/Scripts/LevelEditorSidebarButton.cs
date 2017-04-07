using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelEditorSidebarButton : MonoBehaviour {

	public LevelEditorSidebar.Tool tool;
	[SerializeField] Image image;
	public LevelObject createdObject;
	[SerializeField] Color colour;

	public void SetHighlight(bool clicked)
	{
		image.color = (clicked) ? Color.gray : colour;
	}
}

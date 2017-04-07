using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Grid : MonoBehaviour {

	public RectTransform root;
	public float lineWidth; 
	public Vector2 gridSize;
	public Vector2 cellSize;
	public static Grid instance;
	// Use this for initialization
	void Start () {
		var y = 0f;
		var x = 0f;

		while(y < gridSize.y)
		{
			var horizontalLine = new GameObject("horizontal line");
			var rt = horizontalLine.AddComponent<RectTransform>();
			rt.SetParent(root);
			rt.anchorMin = new Vector2(0, 0);
			rt.anchorMax = new Vector2(1, 0);
			rt.sizeDelta = new Vector2(0, lineWidth);
			var img = horizontalLine.AddComponent<Image>();
			img.color = Color.white;

			rt.anchoredPosition = new Vector2(0, y);
			y += cellSize.y;
		}

		while (x < gridSize.x)
		{
			var verticalLine = new GameObject("verical line");
			var rt = verticalLine.AddComponent<RectTransform>();
			rt.SetParent(root);
			rt.anchorMin = new Vector2(0,0);
			rt.anchorMax = new Vector2(0,1);
			rt.sizeDelta = new Vector2(lineWidth, 0);
			var img = verticalLine.AddComponent<Image>();
			img.color = Color.white;

			rt.anchoredPosition = new Vector2(x, 0);
			x += cellSize.x;
		}
		if (!instance)
			instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

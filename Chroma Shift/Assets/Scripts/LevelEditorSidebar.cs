using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.DemiLib;
using DG.Tweening;

public class LevelEditorSidebar : MonoBehaviour {

	public enum Tool { None, Move, Create, Delete };
	private LevelEditorSidebarButton currentButton;
	private Tool currentTool { get { return currentButton.tool; } }
	public LevelObject currentHeldObject;
	public Vector2 gridSize;
	public bool bounds;
	private Transform currentPanel;
	private Tween panelTween;

	public List<LevelObject> levelObjects = new List<LevelObject>();

	public void Start()
	{
		bounds = true;
	}

	public void PanelClicked(Transform panel) 
	{
		if(currentPanel == panel)
			return;

		if(panelTween != null)
			panelTween.Kill();
		
		HidePanel();
		panel.localScale = Vector3.zero;
		panel.gameObject.SetActive(true);
		panelTween = panel.DOScale(Vector3.one, 0.3f).OnComplete(() => panelTween = null);
		currentPanel = panel;
	}

	void HidePanel()
	{
		if(currentPanel != null) 
		{
			var panel = currentPanel;
			currentPanel.DOScale(Vector3.zero, 0.3f).OnComplete(() => panel.gameObject.SetActive(false));
			currentPanel = null;
		}
	}

	public void OnButtonClicked(LevelEditorSidebarButton button)
	{
		HidePanel();

		if(currentButton)
			currentButton.SetHighlight(false);

		if(currentHeldObject) 
			Destroy(currentHeldObject.gameObject);

		currentButton = button;

		currentButton.SetHighlight(true);

		if (currentTool == Tool.Create)
		{
			var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			worldPos.z = 0;
			currentHeldObject = Instantiate(currentButton.createdObject);
			currentHeldObject.Init(this);
			currentHeldObject.transform.position = worldPos;
			
			currentHeldObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
		}
	}

	void Update()
	{
		if(!currentButton)
			return;

		var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		worldPos.z = 0;

		bool overlap = false;
		if(currentHeldObject)
		{
			var halfWidth = currentHeldObject.GetOffset();
			//halfWidth.x *= currentHeldObject.transform.localScale.x;
			//halfWidth.y *= currentHeldObject.transform.localScale.y;
			worldPos.x -= worldPos.x % gridSize.x;
			worldPos.y -= worldPos.y % gridSize.y;
			worldPos += halfWidth;

			currentHeldObject.transform.position = worldPos;

			if (Input.GetMouseButtonUp(0) && currentTool == Tool.Move)
			{
				currentHeldObject = null;
			}

			for(int i = 0; i < levelObjects.Count; i++) 
			{
				//TODO fix error here
				var c2D = currentHeldObject.bc;
				if(c2D.bounds.Intersects(levelObjects[i].bc.bounds)) 
				{
					overlap = true;
					break;
				}
			}

			var objColor = Color.green;
			if (overlap || !bounds)
				objColor = Color.red;
			
			var sr = new []{currentHeldObject.GetComponent<SpriteRenderer>()};
			if(sr[0] == null)
				sr = currentHeldObject.GetComponentsInChildren<SpriteRenderer>();
			
			for(int i = 0; i < sr.Length; i++)
				sr[i].color = objColor;

		}

		if (!overlap && currentTool == Tool.Create && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
		{
			var o = Instantiate(currentButton.createdObject);
			o.Init(this);
			o.transform.position = worldPos;
			levelObjects.Add(o);
		}

	}
		
	public void Save()
	{
		#if UNITY_EDITOR
		var path = UnityEditor.EditorUtility.SaveFilePanel("Save Level", Application.streamingAssetsPath + "/Levels", "level.txt", "txt");
		HelperFunctions.Save(path, levelObjects);
		#endif
	}

	public void Load()
	{
		#if UNITY_EDITOR
		var path = UnityEditor.EditorUtility.OpenFilePanel("Load Level", Application.streamingAssetsPath + "/Levels", "txt");
		for(int i= 0; i < levelObjects.Count; i++) 
		{
			Destroy(levelObjects[i].gameObject);
		}
		levelObjects.Clear();
			
		HelperFunctions.Load(path, Creator);	
		#endif
	}

	LevelObject Creator(int id){
		var prefab = LevelObjectMap.Instance.GetPrefab(id);
		var obj = Instantiate(prefab);
		obj.Init(this);
		levelObjects.Add(obj);

		return obj;
	}

	public void OnMouseDown(LevelObject obj)
	{
		if (currentTool == Tool.Delete)
		{
			levelObjects.Remove(obj);
			Destroy(obj.gameObject);
		}
		if (currentTool == Tool.Move)
		{
			currentHeldObject = obj;
		}
	}
}
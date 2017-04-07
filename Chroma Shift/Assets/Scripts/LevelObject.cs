using UnityEngine;
using System.Collections;
using System.Linq;

public class LevelObject : MonoBehaviour {
	
	public const int BLOCK_16 = 100;
	public const int BLOCK_32 = 101;
	public const int BLOCK_64 = 102;
	public const int FLASH_16 = 103;
	public const int FLASH_32 = 104;
	public const int FLASH_64 = 105;
	public const int ANTIG_16 = 106;
	public const int ANTIG_32 = 107;
	public const int ANTIG_64 = 108;

	public const int PLAYER_SPAWNER = 200;
	public const int ENEMY_SPAWNER = 201;

	public int objectID;
	public const char SPLIT_CHAR = '_';
	private LevelEditorSidebar sidebar;
	protected bool inEditor = false;
	public BoxCollider2D bc;
	//public bool editorScript;

	public virtual string GetSaveString ()
	{
		return "";
	}
	public virtual void LoadSaveData (string input)
	{
		
	}
	public void Init(LevelEditorSidebar s)
	{
		
		sidebar = s;
		inEditor = true;
		enabled = false;
		bc = GetComponent<BoxCollider2D>();
		bc.size *= 0.99f;
		var animator = GetComponent<Animator>();
		if(animator != null) 
		{
			animator.enabled = false;
		}
	}
	void OnMouseDown()
	{
		if(inEditor)
			sidebar.OnMouseDown(this);
	}
	public virtual Vector3 GetOffset()
	{
		return gameObject.GetComponent<SpriteRenderer>().bounds.extents; 
	}

}

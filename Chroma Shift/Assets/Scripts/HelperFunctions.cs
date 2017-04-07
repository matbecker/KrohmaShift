using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using DG.DemiLib;

public static class HelperFunctions {

	public static LayerMask collidableLayers = 1 << LayerMask.NameToLayer("Collidable");
	public static int collidableLayerId = LayerMask.NameToLayer("Collidable");
	private static bool shrink = true;

	public static IEnumerator TransitionTransparency(Image img, float duration)
	{
		while (true)
		{
			img.CrossFadeAlpha(0.0f, duration, false);
			yield return new WaitForSeconds(duration);
			img.CrossFadeAlpha(1.0f, duration, false);
			yield return new WaitForSeconds(duration);
		}
	}
	public static Vector3 ArcTowards(Transform start, Transform end, float angle)
	{
		var direction = end.position - start.position;
		var height = direction.y;
		direction.y = 0;
		var distance = direction.magnitude;
		var a = angle * Mathf.Deg2Rad;
		direction.y = distance * Mathf.Tan(a);
		distance += height / Mathf.Tan(a);
		var velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));

		return velocity * direction.normalized;
	}

	public static Vector2 Arc(Transform projectileLauncher)
	{
		Vector2 val; 

		val = new Vector2(Mathf.Cos(projectileLauncher.localRotation.z * 2), Mathf.Sin(projectileLauncher.localRotation.z * 2));

		return val;
	}

	public static bool GroundCheck(EdgeCollider2D col, Rigidbody2D rb)
	{
		var point = (rb.gravityScale >= 0) ? (Vector2)col.transform.position + col.offset : (Vector2)col.transform.position - col.offset;
		return Physics2D.OverlapCircle(point, 0.1f, collidableLayers);
		//var leftPoint = (Vector2)col.transform.position + col.offset - (Vector2)col.bounds.extents + new Vector2(0.1f, 0f);
		//var rightPoint = (Vector2)col.transform.position + col.offset + (Vector2)col.bounds.extents - new Vector2(0.1f, 0f);
		//return Physics2D.OverlapArea(leftPoint, rightPoint, collidableLayers);
//		var hits = new List<RaycastHit2D>();
//		hits.AddRange(Physics2D.RaycastAll(leftPoint, new Vector2(0, -0.05f)));
//		hits.AddRange(Physics2D.RaycastAll(rightPoint, new Vector2(0, -0.05f)));
//
//		for(int i = 0; i < hits.Count; i++)
//		{
//			var hit = hits[i];
//			if(hit.collider != null && hit.collider.IsTouchingLayers(collidableLayers))
//				return true;
//		}
//		return false;
		//return (leftHit.collider != null && leftHit.collider.IsTouchingLayers(collidableLayers)) 
		//	|| (rightHit.collider != null && rightHit.collider.IsTouchingLayers(collidableLayers));
	}
	public static bool WallCheck(BoxCollider2D col, Transform transform, bool facingRight)
	{
		if (facingRight)
			return Physics2D.OverlapCircle(new Vector2(transform.position.x + col.bounds.extents.x, transform.position.y), 0.05f, collidableLayers);
		else
			return Physics2D.OverlapCircle(new Vector2(transform.position.x - col.bounds.extents.x, transform.position.y), 0.05f, collidableLayers);
	}

	public static Color ColorLerp(Color currentColor, Color desiredColor, float duration)
	{
		float startTime = Time.time;
		float endTime = startTime + duration;

		while (Time.time < endTime)
		{
			float elapsedTime = Time.time - startTime;
			float percentComplete = elapsedTime / (endTime - startTime);

			return Color.Lerp(currentColor, desiredColor, percentComplete);
		}
		return currentColor;
	}

	public static void FlipScale(GameObject obj, bool facingRight)
	{
		var rb = obj.GetComponent<Rigidbody2D>();
		var y = (rb.gravityScale >= 0) ? 1.0f : -1.0f;
		var x = (facingRight) ? 1.0f : -1.0f;

		obj.transform.DOScale(new Vector3(x,y,1.0f),0.5f);
	}
		
	public static bool IsSameColour(ColourManager.ColourType typeA, ColourManager.ColourType typeB)
	{
		if (typeA == typeB)
			return true;
		else
			return false;
	}
	public static bool IsContrastingColour(ColourManager.ColourType typeA, ColourManager.ColourType typeB)
	{
		switch (typeA)
		{
		case ColourManager.ColourType.Purple:
			if (typeB == ColourManager.ColourType.Yellow)
				return true;
			break;
		case ColourManager.ColourType.Blue:
			if (typeB == ColourManager.ColourType.Orange)
				return true;
			break;
		case ColourManager.ColourType.Green:
			if (typeB == ColourManager.ColourType.Red)
				return true;
			break;
		case ColourManager.ColourType.Yellow:
			if (typeB == ColourManager.ColourType.Purple)
				return true;
			break;
		case ColourManager.ColourType.Orange:
			if (typeB == ColourManager.ColourType.Blue)
				return true;
			break;
		case ColourManager.ColourType.Red:
			if (typeB == ColourManager.ColourType.Green)
				return true;
			break;
		default:
			return false;
			break;
		}
		return false;
	}

	public static void Save(string path, List<LevelObject> levelObjects)
	{
		//var path = EditorUtility.SaveFilePanel("Save Level", Application.streamingAssetsPath + "/Levels", "level.txt", "txt");
		if(path.Length != 0)
		{
			var sb = new System.Text.StringBuilder();
			for(int i = 0; i < levelObjects.Count; i++){
				sb.AppendLine(levelObjects[i].GetSaveString());
			}
			System.IO.File.WriteAllText(path, sb.ToString());
		}
	}

	public static void Load(string path, System.Func<int, LevelObject> creator)
	{
		//var path = 
		if (path.Length != 0)
		{
			var data = System.IO.File.ReadAllText(path);

			var lines = data.Split(new []{'\n'}, System.StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < lines.Length; i++)
			{
				var s = lines[i].Split(LevelObject.SPLIT_CHAR);
				var id = int.Parse(s[0]);

				var obj = creator(id);
				obj.LoadSaveData(lines[i]);
			}
		}
	}
}

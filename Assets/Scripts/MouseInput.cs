using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour {
	private Token selectedTokenScript;

	// Update is called once per frame
	private void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			LeftClick();
		}
		if (Input.GetMouseButtonDown(1))
		{
			RightClick();
		}
	}

	private void LeftClick() {
		if (selectedTokenScript)
		{
			selectedTokenScript.PrintName();
			ClearOldSelection();
		}
		
		Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

		// Find out if we hit a child component
		 foreach(Transform t in transform)
		 {
			 if (!hit.collider || hit.collider.name != t.name) continue;
			 HandleTokenSelected(t);
			 break;
		 }
	}

	private void RightClick() {
		if (selectedTokenScript)
		{
			Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
			selectedTokenScript.MoveToken(worldPoint);
		}
		else
		{
			print("No Selected Token");
		}
	}

	private void ClearOldSelection() 
	{
		selectedTokenScript.Reset();
		selectedTokenScript = null;
	}

	private void HandleTokenSelected(Component t) 
	{
		selectedTokenScript = t.GetComponent<Token>();
		selectedTokenScript.IsSelected = true;
		selectedTokenScript.FindSelectableTiles();
	}
}



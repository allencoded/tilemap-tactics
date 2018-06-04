using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour {
	private Token selectedTokenScript;

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			LeftClick();
		}
		if (Input.GetMouseButtonDown(1))
		{
			RightClick();
		}
	}

	void LeftClick() {
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
			if (hit.collider && hit.collider.name == t.name) 
			{
				HandleTokenSelected(t);
				break;
			}
		 }
	}

	void RightClick() {
		if (selectedTokenScript)
		{
			Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
			selectedTokenScript.MoveToken(worldPoint);
		}
		else
		{
			print("No Selected Token");
			return;
		}
	}

	void ClearOldSelection() 
	{
		selectedTokenScript.Reset();
		selectedTokenScript = null;
	}

	void HandleTokenSelected(Transform t) 
	{
		selectedTokenScript = t.GetComponent<Token>();
		selectedTokenScript.isSelected = true;
		selectedTokenScript.FindSelectableTiles();
	}
}



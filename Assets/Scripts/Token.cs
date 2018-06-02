using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Token : MonoBehaviour {
	/* TODO SCRIPTABLT OBJECT Tokens: isEnemy, movement, amount moved, health, armor, abilites, isSelected */

	public int tokenMovementDistance = 7;
	public bool isEnemy = false;
	public List<WorldTile> selectableTiles;
	public WorldTile tokenLocation;
	public bool isSelected;

	// Use this for initialization
	void Start () 
	{
		GetTokenWorldTileLocation();
		print(tokenLocation.Name);
	}

	public void GetTokenWorldTileLocation()
	{
		var tiles = GameTiles.instance.tiles;
		tokenLocation = tiles[transform.position];
	}


	
	// Update is called once per frame
	void Update () 
	{
		checkMouse();
	}

	void checkMouse() 
	{
		if (Input.GetMouseButtonDown(0) && isSelected == false)
		{
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

			print(hit.collider.name);
			print(transform.name);

			if (hit.collider && hit.collider.name == transform.name)
			{
				Debug.Log("HIT!");
				FindSelectableTiles();
			}
			isSelected = true;
		}

		if (Input.GetMouseButtonDown(1))
		{
			Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
			print("World Location Clicked: " + worldPoint.ToString());
			foreach(var tile in selectableTiles)
			{
				if (worldPoint == tile.WorldLocation)
				{
					Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
					var path = pathfinder.CreatePath(selectableTiles, tile);
					StartCoroutine(FollowPath(path));
					isSelected = false;
					break;
				}
			}
		}

		if (Input.GetMouseButtonDown(2) && selectableTiles.Count > 1)
		{
			Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
			pathfinder.ResetSelectableTiles();
		}
	}

	void FindSelectableTiles()
	{
		Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
		selectableTiles = pathfinder.GetSelectableWorldTiles(tokenMovementDistance, tokenLocation);
	}

	IEnumerator FollowPath(List<WorldTile> path)
	{
		foreach (WorldTile tile in path)
		{
			Debug.Log("Moving to :" + tile.WorldLocation.ToString());
			transform.position = new Vector3(tile.WorldLocation.x, tile.WorldLocation.y, tile.WorldLocation.z);
			tokenLocation = tile;
			yield return new WaitForSeconds(1f);
		}
		Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
		pathfinder.ResetSelectableTiles();
	}
}

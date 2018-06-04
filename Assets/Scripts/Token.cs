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

	public void PrintName() 
	{
		print(transform.name);
	}

	public void FindSelectableTiles()
	{
		Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
		selectableTiles = pathfinder.GetSelectableWorldTiles(tokenMovementDistance, tokenLocation);
	}

	public void MoveToken(Vector3Int clickedPoint) 
	{
		foreach(var tile in selectableTiles)
		{
			if (clickedPoint == tile.WorldLocation) 
			{
				Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
				var path = pathfinder.CreatePath(selectableTiles, tile);
				StartCoroutine(FollowPath(path));
				return;
			}
		}
		print("No Valid Path");
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
		Reset();
	}

	public void Reset()
	{
		Pathfinder pathfinder = FindObjectOfType<Pathfinder>();
		pathfinder.ResetSelectableTiles();
		isSelected = false;
		selectableTiles = new List<WorldTile>();
	}
}

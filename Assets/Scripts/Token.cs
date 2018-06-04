using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Token : MonoBehaviour {
	/* TODO SCRIPTABLT OBJECT Tokens: isEnemy, movement, amount moved, health, armor, abilites, isSelected */
	public int TokenMovementDistance = 7;
	public bool IsEnemy;
	public List<WorldTile> SelectableTiles;
	public WorldTile TokenLocation;
	public bool IsSelected;

	// Use this for initialization
	private void Start () 
	{
		GetTokenWorldTileLocation();
		print(TokenLocation.Name);
	}

	private void GetTokenWorldTileLocation()
	{
		var tiles = GameTiles.instance.tiles;
		TokenLocation = tiles[transform.position];
	}

	public void PrintName() 
	{
		print(transform.name);
	}

	public void FindSelectableTiles()
	{
		var pathfinder = FindObjectOfType<Pathfinder>();
		SelectableTiles = pathfinder.GetSelectableWorldTiles(TokenMovementDistance, TokenLocation);
	}

	public void MoveToken(Vector3Int clickedPoint) 
	{
		foreach(var tile in SelectableTiles)
		{
			if (clickedPoint != tile.WorldLocation) continue;
			var pathfinder = FindObjectOfType<Pathfinder>();
			var path = pathfinder.CreatePath(SelectableTiles, tile);
			StartCoroutine(FollowPath(path));
			return;
		}
		print("No Valid Path");
	}

	private IEnumerator FollowPath(IEnumerable<WorldTile> path)
	{
		foreach (var tile in path)
		{
			Debug.Log("Moving to :" + tile.WorldLocation);
			transform.position = new Vector3(tile.WorldLocation.x, tile.WorldLocation.y, tile.WorldLocation.z);
			TokenLocation = tile;
			yield return new WaitForSeconds(1f);
		}
		Reset();
	}

	public void Reset()
	{
		var pathfinder = FindObjectOfType<Pathfinder>();
		pathfinder.ResetSelectableTiles();
		IsSelected = false;
		SelectableTiles = new List<WorldTile>();
	}
}

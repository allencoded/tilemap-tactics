using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour {

	[SerializeField] WorldTile startTile, endTile;
	Dictionary<Vector3, WorldTile> grid = new Dictionary<Vector3, WorldTile>();
	bool isRunning = true;

	// BFS Specific Stuff
	Queue<WorldTile> queue = new Queue<WorldTile>();
	WorldTile currentSearchCenter; // this is for when its looping it knows where its searching from
	List<WorldTile> selectableTiles = new List<WorldTile>(); 

	public int tokenMovementDistance;

	// This might go on the tile rule so each tile has its own movement
	Vector3[] directions = 
	{
		Vector3.up,
		Vector3.right,
		Vector3.down,
		Vector3.left,
	};

	/// <summary>
	/// Returns all the selectable world tiles based on movement.
	/// <param name="distance">int of tokens available movement distance</param>
	/// </summary>
	public List<WorldTile> GetSelectableWorldTiles(int distance, WorldTile tokenLocation)
	{
		tokenLocation.Cost = 1; // TODO: BUG FIX HERE. The tokenLocation is never getting its cost reset so I hard reset it here but this is wrong.
		startTile = tokenLocation;
		tokenMovementDistance = distance;
		LoadBlocks();
		BreadthFirstSearch();
		return selectableTiles;
	}

	public void ResetSelectableTiles()
	{
		foreach(WorldTile tile in selectableTiles)
		{
			tile.Cost = 1; // TODO: We can't always assume tiles are = to 1 this is wrong.
			tile.IsExplored = false;
			SetTileColor(tile, Color.white);
		}
		startTile = null;
		grid = new Dictionary<Vector3, WorldTile>();
		queue = new Queue<WorldTile>();
		currentSearchCenter = new WorldTile();
		selectableTiles = new List<WorldTile>();
	}

	public List<WorldTile> CreatePath(List<WorldTile> tiles, WorldTile end)
	{	
		List<WorldTile> test = new List<WorldTile>();
		test.Add(end);
		WorldTile previous = end.ExploredFrom;
		while (previous != startTile)
		{
			test.Add(previous);
			previous = previous.ExploredFrom;
		}

		test.Add(startTile);
		test.Reverse();
		return test;
	}

	/// <summary>
	/// Loads all the WorldTiles that are traversable by the player into Dictionary grid.
	/// </summary>
	private void LoadBlocks() 
	{
		foreach (var tile in GameTiles.instance.tiles)
		{
			var pos = tile.Value.WorldLocation;
			bool isOverlapping = grid.ContainsKey(pos);
			if (isOverlapping)
			{
				Debug.Log("Overlapping block" + tile + ". Skipping but you need to remove it.");
			}
			else
			{
				grid.Add(pos, tile.Value); // I rather prob use a struct because here if we change a value on a tile its changed on the instance
			}
		}
	}

	/// <summary>
	/// Preforms our BreadthFirstSearch on WorldTiles in the Dictionary grid.
	/// </summary>
	private void BreadthFirstSearch()
	{
		queue.Enqueue(startTile);

		while(queue.Count > 0 && isRunning)
		{
			currentSearchCenter = queue.Dequeue();
			
			if (currentSearchCenter.Cost < tokenMovementDistance)
			{
				selectableTiles.Add(currentSearchCenter);
				SetTileColor(currentSearchCenter, Color.green);

				ExploreNeighbours();

				currentSearchCenter.IsExplored = true;
			}
		}
	}

	/// <summary>
	/// Sets our WorldTile color
	/// </summary>
	private void SetTileColor(WorldTile tile, Color color)
	{
		tile.TilemapMember.SetTileFlags(tile.LocalPlace, TileFlags.None);
		tile.TilemapMember.SetColor(tile.LocalPlace, color);
	}

	/// <summary>
	/// Explores the neighbours of our currentSearchCenter by provided directions (up down left right).
	/// If it finds a WorldTile in any of those directions it runs the QueueNewNeighbour for that WorldTile.
	/// </summary>
	private void ExploreNeighbours()
	{
		if (!isRunning) { return; }
		
		foreach (Vector3 direction in directions)
		{
			Vector3 neighbourCoordinates = currentSearchCenter.WorldLocation + direction;
			if (grid.ContainsKey(neighbourCoordinates))
			{
				QueueNewNeighbour(neighbourCoordinates);
			}
		}
	}

	/// <summary>
	/// Handles Queueing new neighbour WorldTile. Also increments our Cost.
	/// </summary>
	private void QueueNewNeighbour(Vector3 neighbourCoordinates)
	{
		WorldTile neighbour = grid[neighbourCoordinates];
		if (neighbour.IsExplored || queue.Contains(neighbour))
		{
			return;
		}
		queue.Enqueue(neighbour);
		neighbour.ExploredFrom = currentSearchCenter;
		neighbour.Cost = 1 + currentSearchCenter.Cost; // TODO: This isn't right it assumes all tiles are 1 Cost and using wrong field
	}
}

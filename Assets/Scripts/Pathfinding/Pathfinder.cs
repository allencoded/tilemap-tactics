using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour {

	[SerializeField] WorldTile _startTile, _endTile;
	Dictionary<Vector3, WorldTile> _grid = new Dictionary<Vector3, WorldTile>();
	bool isRunning = true;

	// BFS Specific Stuff
	private Queue<WorldTile> _queue = new Queue<WorldTile>();
	private WorldTile _currentSearchCenter; // this is for when its looping it knows where its searching from
	private List<WorldTile> _selectableTiles = new List<WorldTile>(); 
	private int TokenMovementDistance;

	// This might go on the tile rule so each tile has its own movement
	private readonly Vector3[] _directions = 
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
		_startTile = tokenLocation;
		TokenMovementDistance = distance;
		LoadBlocks();
		BreadthFirstSearch();
		return _selectableTiles;
	}

	public void ResetSelectableTiles()
	{
		foreach(WorldTile tile in _selectableTiles)
		{
			tile.Cost = 1; // TODO: We can't always assume tiles are = to 1 this is wrong.
			tile.IsExplored = false;
			SetTileColor(tile, Color.white);
		}
		_startTile = null;
		_grid = new Dictionary<Vector3, WorldTile>();
		_queue = new Queue<WorldTile>();
		_currentSearchCenter = new WorldTile();
		_selectableTiles = new List<WorldTile>();
	}

	public List<WorldTile> CreatePath(List<WorldTile> tiles, WorldTile end)
	{	
		var test = new List<WorldTile>();
		test.Add(end);
		WorldTile previous = end.ExploredFrom;
		while (previous != _startTile)
		{
			test.Add(previous);
			previous = previous.ExploredFrom;
		}

		test.Add(_startTile);
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
			Vector3 pos = tile.Value.WorldLocation;
			var isOverlapping = _grid.ContainsKey(pos);
			if (isOverlapping)
			{
				Debug.Log("Overlapping block" + tile + ". Skipping but you need to remove it.");
			}
			else
			{
				_grid.Add(pos, tile.Value); // I rather prob use a struct because here if we change a value on a tile its changed on the instance
			}
		}
	}

	/// <summary>
	/// Preforms our BreadthFirstSearch on WorldTiles in the Dictionary grid.
	/// </summary>
	private void BreadthFirstSearch()
	{
		_queue.Enqueue(_startTile);

		while(_queue.Count > 0 && isRunning)
		{
			_currentSearchCenter = _queue.Dequeue();
			
			if (_currentSearchCenter.Cost < TokenMovementDistance)
			{
				_selectableTiles.Add(_currentSearchCenter);
				SetTileColor(_currentSearchCenter, Color.green);

				ExploreNeighbours();

				_currentSearchCenter.IsExplored = true;
			}
		}
	}

	/// <summary>
	/// Sets our WorldTile color
	/// </summary>
	private static void SetTileColor(WorldTile tile, Color color)
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
		
		foreach (Vector3 direction in _directions)
		{
			Vector3 neighbourCoordinates = _currentSearchCenter.WorldLocation + direction;
			if (_grid.ContainsKey(neighbourCoordinates))
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
		WorldTile neighbour = _grid[neighbourCoordinates];
		if (neighbour.IsExplored || _queue.Contains(neighbour))
		{
			return;
		}
		_queue.Enqueue(neighbour);
		neighbour.ExploredFrom = _currentSearchCenter;
		neighbour.Cost = 1 + _currentSearchCenter.Cost; // TODO: This isn't right it assumes all tiles are 1 Cost and using wrong field
	}
}

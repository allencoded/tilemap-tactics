using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTiles : MonoBehaviour {
	public static GameTiles instance;
	public Tilemap tilemap;

	public Dictionary<Vector3, WorldTile> tiles;

	void Awake() 
	{
		if (instance == null) 
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		GetWorldTiles();
	}

	// Use this for initialization
	void GetWorldTiles () 
	{
        tiles = new Dictionary<Vector3, WorldTile>();
		foreach (var pos in tilemap.cellBounds.allPositionsWithin)
		{
			Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);

			if (tilemap.HasTile(localPlace))
			{
				WorldTile tile = new WorldTile();
				tile.LocalPlace = localPlace;
				tile.WorldLocation = tilemap.CellToWorld(localPlace);
				tile.TileBase = tilemap.GetTile(localPlace);
				tile.TilemapMember = tilemap;
				tile.Name = localPlace.x.ToString() + "," + localPlace.y.ToString();
				tile.Cost = 1; // TODO: Change this with the proper cost from ruletile
				tiles.Add(tile.WorldLocation, tile);
			}
		}
	}
}

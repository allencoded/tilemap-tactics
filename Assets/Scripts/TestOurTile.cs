using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestOurTile : MonoBehaviour {
	private WorldTile tile;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);

			var tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

			if (tiles.TryGetValue(worldPoint, out tile)) 
			{
				print("Tile " + tile.Name + " costs: " + tile.Cost);
				tile.TilemapMember.SetTileFlags(tile.LocalPlace, TileFlags.None);
				tile.TilemapMember.SetColor(tile.LocalPlace, Color.green);
			}
		}
	}
}

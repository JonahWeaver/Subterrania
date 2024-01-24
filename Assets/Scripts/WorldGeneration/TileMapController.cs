using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{
    public TileBase[] tiles;

    public void RegenerateCompositeCollider()
    {
        CompositeCollider2D coll = gameObject.GetComponent<CompositeCollider2D>();
        coll.GenerateGeometry();
    }
    public void DrawTile(Vector3Int position, int tileToDrawIndex)
    {
        this.GetComponent<Tilemap>().SetTile(position, tiles[tileToDrawIndex]);
    }

    // Delete a tile at the given position from the tile map
    public void DeleteTile(Vector3Int position)
    {
        this.GetComponent<Tilemap>().SetTile(position, null);
    }
}

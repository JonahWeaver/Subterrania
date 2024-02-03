using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChunk
{
    byte[,] tileMap = new byte[TileData.ChunkWidth, TileData.ChunkWidth];
    ChunkCoord coord;
    TileWorld world;

    public GameObject chunkObject;
    // Start is called before the first frame update
    public TileChunk(ChunkCoord _coord, TileWorld _world)
    {
        coord = _coord;
        world = _world;
        chunkObject = new GameObject();

        //meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * TileData.ChunkWidth, coord.y * TileData.ChunkWidth, 0f);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.y;

        PopulateTileMap();
        AddTilesToChunk();
    }

    public void RemoveTileFromTileMap(int x, int y)
    {
        tileMap[x, y] = 0;
    }

    public void AddTileToTileMap(int x, int y, byte tileByte)
    {
        tileMap[x, y] = tileByte;
    }

    void PopulateTileMap()
    {
        for (int x = 0; x < TileData.ChunkWidth; x++)
        {
            for (int y = 0; y < TileData.ChunkWidth; y++)
            {
                tileMap[x, y] = world.GenerateTile(new Vector3(x, y, 0) + position);
            }
        }
    }

    void AddTilesToChunk()
    {
        for (int x = 0; x < TileData.ChunkWidth; x++)
        {
            for (int y = 0; y < TileData.ChunkWidth; y++)
            {
                Vector3 pos = new Vector3(x, y, 0);
                if (CheckTile(pos))
                {
                    byte tileID = tileMap[(int)pos.x, (int)pos.y];
                    world.tileMapController.DrawTile(new Vector3Int((int)pos.x + coord.x * TileData.ChunkWidth, (int)pos.y + coord.y * TileData.ChunkWidth, 0), tileID);
                }
            }
        }
    }

    public bool isActive
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }

    public Vector3 position
    {
        get { return chunkObject.transform.position; }
    }

    bool IsTileInChunk(int x, int y, int z)
    {
        return !(x < 0 || x > TileData.ChunkWidth - 1 || z != 0 || y < 0 || y > TileData.ChunkWidth - 1);
    }

    bool CheckTile(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsTileInChunk(x, y, z))
        {
            return world.tileTypes[world.GenerateTile(pos + position)].isSolid;
        }

        return world.tileTypes[tileMap[x, y]].isSolid;
    }

    public void DrawTiles()
    {
        for (int x = 0; x < TileData.ChunkWidth; x++)
        {
            for (int y = 0; y < TileData.ChunkWidth; y++)
            {
                if (CheckTile(new Vector3(x, y, 0)))
                {
                    byte tileID = tileMap[x, y];
                    world.tileMapController.DrawTile(new Vector3Int(x + coord.x * TileData.ChunkWidth, y + coord.y * TileData.ChunkWidth, 0), tileID);
                }
            }
        }
    }
    public void RemoveTiles()
    {
        for (int x = 0; x < TileData.ChunkWidth; x++)
        {
            for (int y = 0; y < TileData.ChunkWidth; y++)
            {
                world.tileMapController.DeleteTile(new Vector3Int(x + (int)(position.x), y + (int)(position.y), 0));
            }
        }
    }

    public byte GetTileByte(int x, int y)
    {
        return tileMap[x, y];
    }
}
public class ChunkCoord
{
    public int x;
    public int y;

    public ChunkCoord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null)
        {
            return false;
        }
        else if (other.x == x && other.y == y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
} 


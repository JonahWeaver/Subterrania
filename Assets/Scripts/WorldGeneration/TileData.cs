using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    //public static readonly int TextureAtlasSizeInBlocks = 4;
    //public static float NormalizedBlockTextureSize
    //{
    //    get { return 1f / (float)TextureAtlasSizeInBlocks; }
    //}


    public static readonly int ChunkWidth = 8;
    //public static readonly int ChunkHeight = 2;
    public static readonly int WorldSizeInChunks = 4000;

    public static int WorldSizeInTiles
    {
        get { return WorldSizeInChunks * ChunkWidth; }
    }

    public static readonly int ViewDistanceInChunks = 1;
}


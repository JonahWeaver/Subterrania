using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int seed;

    public Transform player;
    public Vector3 spawnPosition;

    public Material material;
    public TileType[] tileTypes;
    public BiomeType[] biomeTypes;
    byte[,] tile;

    public float[] offsets;
    public float[] scales;

    float[] previousOffsets;
    float[] previousScales;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord prevChunkCoord;

    private void Start()
    {
        offsets = new float[biomeTypes.Length];
        scales = new float[biomeTypes.Length];
        previousOffsets = new float[biomeTypes.Length];
        previousScales = new float[biomeTypes.Length];

        for (int i=0; i< biomeTypes.Length;i++)
        {
            offsets[i] = 0f;
            scales[i] = .1f;
            previousOffsets[i] = 0f;
            previousScales[i] = .1f;
        }

        tile = new byte[1000, 1000];
        Random.InitState(seed);
        spawnPosition = new Vector3(VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth / 2f, VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth / 2f, 0);
        GenerateWorld();
        prevChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    private void Update()
    {
        if (NewChunkCoord())
        {
            CheckViewDistance();
        }
        if(OffsetOrScaleChanged())
        {
            for(int i=0;i< VoxelData.WorldSizeInChunks;i++)
            {
                for (int j = 0; j < VoxelData.WorldSizeInChunks; j++)
                {
                    if (chunks[i, j] != null)
                    {
                        Destroy(chunks[i, j].chunkObject);
                        chunks[i, j] = null;
                    }
                }
            }

            activeChunks.Clear();
            GenerateWorld();
        }
    }

    bool NewChunkCoord()
    {
        if(GetChunkCoordFromVector3(player.position)==prevChunkCoord)
        {
            return false;
        }
        else
        {
            prevChunkCoord = GetChunkCoordFromVector3(player.position);
            return true;
        }
    }
    void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSizeInChunks / 2)-VoxelData.ViewDistanceInChunks-1; x <= (VoxelData.WorldSizeInChunks / 2) +VoxelData.ViewDistanceInChunks; x++)
        {
            for (int y = (VoxelData.WorldSizeInChunks / 2) -VoxelData.ViewDistanceInChunks-1; y <= (VoxelData.WorldSizeInChunks / 2) +VoxelData.ViewDistanceInChunks; y++)
            {
                CreateNewChunk(x, y);
            }
        }
        player.position = spawnPosition;
        prevChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int y = Mathf.FloorToInt(pos.y / VoxelData.ChunkWidth);

        return new ChunkCoord(x, y);

    }

    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        prevChunkCoord = coord;
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x-VoxelData.ViewDistanceInChunks; x< coord.x+ VoxelData.ViewDistanceInChunks;x++)
        {
            for (int y = coord.y - VoxelData.ViewDistanceInChunks; y < coord.y + VoxelData.ViewDistanceInChunks; y++)
            {
                if(IsChunkInWorld(new ChunkCoord(x,y)))
                {
                    if(chunks[x,y]==null)
                    {
                        CreateNewChunk(x, y);
                    }
                    else if(!chunks[x,y].isActive)
                    {
                        chunks[x, y].isActive = true;
                        activeChunks.Add(new ChunkCoord(x, y));
                    }
                }

                for(int i=0; i<previouslyActiveChunks.Count;i++)
                {
                    if(previouslyActiveChunks[i].Equals(new ChunkCoord(x,y)))
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        foreach(ChunkCoord cc in previouslyActiveChunks)
        {
            chunks[cc.x, cc.y].isActive = false;
            activeChunks.Remove(cc);
        }
    }

    public byte GetVoxel(Vector3 pos)
    {
        if(!IsVoxelInWorld(pos))
        {
            return 0;
        }
        //float tempNoise = Noise.Get2DPerlin(pos, 0, 0.1f);
        //int options = 4;
        //return (byte)(Mathf.RoundToInt(tempNoise * options));
        return GetBiome(pos);
    }

    void CreateNewChunk(int x, int y)
    {
        chunks[x, y] = new Chunk(new ChunkCoord(x, y), this);
        activeChunks.Add(new ChunkCoord(x, y));
        chunks[x, y].isActive = true;
    }

    bool IsChunkInWorld(ChunkCoord coord)
    {
        if(coord.x>0&&coord.x<VoxelData.WorldSizeInChunks-1&& coord.y > 0 && coord.y < VoxelData.WorldSizeInChunks - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.WorldSizeInVoxels && pos.z==0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    byte GetBiome(Vector3 pos)
    {
        float moisture = Noise.Get2DPerlin(pos, offsets[0], scales[0]);
        float height = Noise.Get2DPerlin(pos, offsets[1], scales[1]);
        float heat = Noise.Get2DPerlin(pos, offsets[2], scales[2]);

        List<BiomeType> biomeTypesTemp = new List<BiomeType>();

        foreach(BiomeType bt in biomeTypes)
        {
            if (bt.moisture <= moisture && bt.height <= height && bt.heat <= heat)
            {
                biomeTypesTemp.Add(bt);
            }
        }

        float minDif = 3;
        BiomeType r = null;

        foreach (BiomeType bt in biomeTypesTemp)
        {
            float tempDif = (moisture - bt.moisture) + (height - bt.height) + (heat - bt.heat);
            if(tempDif< minDif)
            {
                minDif = tempDif;
                r = bt;
            }
        }

        if (r==null) return 0;
        //Debug.Log(r.biomeID);
        return r.biomeID;
    }

    bool OffsetOrScaleChanged()
    {
        for(int i=0; i< biomeTypes.Length;i++)
        {
            if(offsets[i]!=previousOffsets[i]||scales[i]!=previousScales[i])
            {
                previousOffsets[i] = offsets[i];
                previousScales[i] = scales[i];
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class TileType
{
    public string tileName;
    public bool isSolid;
    public int textureID;
}

[System.Serializable]
public class BiomeType
{
    public float height;
    public float moisture;
    public float heat;

    public byte biomeID;
}



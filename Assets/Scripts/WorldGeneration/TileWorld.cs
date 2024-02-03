using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileWorld : MonoBehaviour
{
    public ItemManager itemManager;

    public GenerationMethod method;
    public int seed;

    public Transform player;
    ChunkCoord previousPlayerChunkCoord;

    //public Transform[] players;
    //ChunkCoord[] prevChunkCoords;

    public Vector3 spawnPosition;
    public TileMapController tileMapController;

    //public Material material;
    public TileType[] tileTypes;
    public BiomeType[] biomeTypes;
    byte[,] tile;

    public float[] offsets; //these are relevant functions that allow for dynamically testing different values to see how it affects world generation
    public float[] scales;

    float[] previousOffsets;
    float[] previousScales;

    TileChunk[,] chunks = new TileChunk[TileData.WorldSizeInChunks, TileData.WorldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();

    Queue<ChunkCoord> chunksToCreate = new Queue<ChunkCoord>();
    Queue<ChunkCoord> chunksToDelete = new Queue<ChunkCoord>();
    //if chunk is in creation list and deletion list?

    public int maxChunkActionCount = 1;


    public int regenAttemptPerFrame = 2;
    public int regenCount = 10;
    public int maxRegenCount = 10;
    bool shouldRecalc = false;

    // Start is called before the first frame update
    void Start()
    {
        //players = new Transform[4]; //when implementing multiplayer, should integrate some global array of player information and two max multiplayer counts
        //one for hard cap (array size) and one for what designer believes to be the max capable/necessary (i.e 4 for Monster hunter)
        //ChunkCoord = new ChunkCoord[4];
        offsets = new float[biomeTypes.Length];
        scales = new float[biomeTypes.Length];
        previousOffsets = new float[biomeTypes.Length];
        previousScales = new float[biomeTypes.Length];

        for (int i = 0; i < biomeTypes.Length; i++)
        {
            offsets[i] = 0f;
            scales[i] = .8f;
            previousOffsets[i] = 0f;
            previousScales[i] = .8f;
        }

        Random.InitState(seed);
        spawnPosition = DeterminePlayerSpawnPosition();
        GenerateWorld();
        previousPlayerChunkCoord = GetChunkCoordFromVector3(player.position);
        tileMapController.RegenerateCompositeCollider();
    }

    // Update is called once per frame
    void Update()
    {
        //if player changes chunkcoords
        //Update current viewable chunks

        if (PlayerChunkCoordChanged())
        {
            AllocateChunksWithinView();
        }
        //if offset or scale changed
        //Regenerate the world with these settings
        if (OffsetOrScaleChanged())
        {
            for (int i = 0; i < TileData.WorldSizeInChunks; i++)
            {
                for (int j = 0; j < TileData.WorldSizeInChunks; j++)
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
            tileMapController.RegenerateCompositeCollider();
        }

        //keep queue of chunks to be changed
        int chunkActions = 0;

        while (chunkActions < maxChunkActionCount && chunksToDelete.Count > 0)
        {
            ChunkCoord chunkCoord = chunksToDelete.Dequeue();
            RemoveChunk(chunkCoord);
            shouldRecalc = true;
            chunkActions++;
        }
        while (chunkActions < maxChunkActionCount && chunksToCreate.Count > 0)
        {
            ChunkCoord chunkCoord = chunksToCreate.Dequeue();
            AssignChunk(chunkCoord);
            shouldRecalc = true;
            chunkActions++;
        }

        if(shouldRecalc && chunksToCreate.Count == 0 && chunksToDelete.Count == 0)
        {
            shouldRecalc = false;
            regenCount = 0; //attempt to regenerate composite collider
        }

        int frameRegenCount = 0;
        while(regenCount < maxRegenCount && frameRegenCount < regenAttemptPerFrame)
        {
            tileMapController.RegenerateCompositeCollider();
            frameRegenCount++;
            regenCount++;
        }
    }

    public void BreakBlockInWorld(Vector2 worldPoint)
    {
        ChunkCoord cc = GetChunkCoordFromVector3(worldPoint);

        TileChunk chunk = chunks[cc.x, cc.y];
        if (chunk != null)
        {
            byte tileByte = chunk.GetTileByte((int)(worldPoint.x - (cc.x * TileData.ChunkWidth)), (int)(worldPoint.y) - (cc.y * TileData.ChunkWidth));
            BaseItem item = tileTypes[tileByte].item;
            tileMapController.DeleteTile(new Vector3Int((int)(worldPoint.x), (int)(worldPoint.y), 0));
            if (tileByte != 0) itemManager.DropItemOnGround(new Vector2((int)(worldPoint.x) + .5f, (int)(worldPoint.y) + .5f), item);
            chunk.RemoveTileFromTileMap((int)(worldPoint.x - (cc.x * TileData.ChunkWidth)), (int)(worldPoint.y) - (cc.y * TileData.ChunkWidth));
            regenCount = 0;
        }
    }
    
    public void PlaceBlockInWorld(Vector2 worldPoint)
    {
        ChunkCoord cc = GetChunkCoordFromVector3(worldPoint);

        TileChunk chunk = chunks[cc.x, cc.y];
        if (chunk != null)
        {
            byte tileByte = chunk.GetTileByte((int)(worldPoint.x - (cc.x * TileData.ChunkWidth)), (int)(worldPoint.y) - (cc.y * TileData.ChunkWidth));

            if (tileByte == 0)
            {
                BaseItem item = player.GetComponent<Inventory>().GetFirstItem();
                if (item != null)
                {
                    tileMapController.DrawTile(new Vector3Int((int)(worldPoint.x), (int)(worldPoint.y), 0), item.tileTypeIndex);
                    chunk.AddTileToTileMap((int)(worldPoint.x - (cc.x * TileData.ChunkWidth)), (int)(worldPoint.y) - (cc.y * TileData.ChunkWidth), item.tileTypeIndex);
                }
            }
            regenCount = 0;
        }
    }

    bool PlayerChunkCoordChanged()
    {
        ChunkCoord playerCoord = GetChunkCoordFromVector3(player.position);
        if (playerCoord.Equals(previousPlayerChunkCoord))
        {
            return false;
        }
        else
        {
            previousPlayerChunkCoord = playerCoord;
            return true;
        }
    }

    void AllocateChunksWithinView()
    {
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        //Debug.Log(previouslyActiveChunks.Count.ToString());
        chunksToCreate.Clear();
        //chunksToDelete.Clear();
        activeChunks.Clear();

        int l = coord.x - TileData.ViewDistanceInChunks;
        int r = coord.x + TileData.ViewDistanceInChunks;
        int u = coord.y + TileData.ViewDistanceInChunks;
        int d = coord.y - TileData.ViewDistanceInChunks;

        for (int x = l; x <= r; x++)
        {
            for (int y = d; y <= u; y++)
            {
                ChunkCoord coordToCheck = new ChunkCoord(x, y);
                //Debug.Log(x.ToString() +" "+ y.ToString())
                if (chunks[x, y]==null||!chunks[x,y].isActive)
                {
                    chunksToCreate.Enqueue(coordToCheck);
                }
                if(chunks[x, y] != null && chunks[x,y].isActive)
                {
                    activeChunks.Add(coordToCheck);
                }
            }
        }
        for (int i=0;i< previouslyActiveChunks.Count;i++)
        {
            int x = previouslyActiveChunks[i].x;
            int y = previouslyActiveChunks[i].y;

            if(!(x<l||x>r||y<d||y>u))
            {
                previouslyActiveChunks.RemoveAt(i--);
            }
        }

        foreach (ChunkCoord cc in previouslyActiveChunks)
        {
            chunksToDelete.Enqueue(cc);
        }
    }

    void GenerateWorld()
    {
        int l = (TileData.WorldSizeInChunks / 2) - TileData.ViewDistanceInChunks;
        l = l > 0 ? l : 0;
        int r = (TileData.WorldSizeInChunks / 2) + TileData.ViewDistanceInChunks;
        r = r < TileData.WorldSizeInTiles ? r : TileData.WorldSizeInTiles;
        for (int x = l; x <= r; x++)
        {
            for (int y = l; y <= r; y++)
            {
                CreateNewChunk(x, y);
            }
        }
        player.position = spawnPosition;
        previousPlayerChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    Vector3 DeterminePlayerSpawnPosition()
    {
       return new Vector3(TileData.WorldSizeInChunks * TileData.ChunkWidth / 2f, TileData.WorldSizeInChunks * TileData.ChunkWidth / 2f, 0) + new Vector3(.5f, .5f, 0);
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / TileData.ChunkWidth);
        int y = Mathf.FloorToInt(pos.y / TileData.ChunkWidth);

        return new ChunkCoord(x, y);
    }

    public byte GenerateTile(Vector3 pos)
    {
        int distanceFromSpawn = 6;

        //when generating tiles, check if tiles near spawn should be different
        if (pos.y == spawnPosition.y - 1 && pos.x > spawnPosition.x - distanceFromSpawn / 2 && pos.x < spawnPosition.x + distanceFromSpawn / 2)
        {
            return 1;
        }
        if (!IsTileInWorld(pos) || (Mathf.Abs(pos.x - spawnPosition.x) + Mathf.Abs(pos.y - spawnPosition.y)) < distanceFromSpawn)
        {
            return 0;
        }

        return GetBiome(pos);
        //currently choose tile based on current biome instead of any other procedurally generated content 
    }

    void CreateNewChunk(int x, int y)
    {
        ChunkCoord chunkCoord = new ChunkCoord(x, y);
        chunks[x, y] = new TileChunk(chunkCoord, this);
        AddChunk(chunkCoord);
    }

    void AssignChunk(ChunkCoord chunkCoord)
    {
        int x = chunkCoord.x;
        int y = chunkCoord.y;
        if (IsChunkInWorld(chunkCoord))
        {
            if (chunks[x, y] == null)
            {
                CreateNewChunk(x, y);
                chunks[x, y].DrawTiles();
            }
            else if (!chunks[x, y].isActive)
            {
                AddChunk(chunkCoord);
                chunks[x, y].DrawTiles();
            }
        }
    }

    void AddChunk(ChunkCoord chunkCoord)
    {
        activeChunks.Add(chunkCoord);
        chunks[chunkCoord.x, chunkCoord.y].isActive = true;
    }

    void RemoveChunk(ChunkCoord chunkCoord)
    {
        chunks[chunkCoord.x, chunkCoord.y].RemoveTiles();
        chunks[chunkCoord.x, chunkCoord.y].isActive = false;
    }

    bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < TileData.WorldSizeInChunks - 1 && coord.y > 0 && coord.y < TileData.WorldSizeInChunks - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsTileInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < TileData.WorldSizeInTiles && pos.y >= 0 && pos.y < TileData.WorldSizeInTiles && pos.z == 0)
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
        switch(method)
        {
            case (GenerationMethod.Basic):
                return BasicGeneration(pos);
        }

        return 0;
    }

    bool OffsetOrScaleChanged()
    {
        for (int i = 0; i < biomeTypes.Length; i++)
        {
            if (offsets[i] != previousOffsets[i] || scales[i] != previousScales[i])
            {
                previousOffsets[i] = offsets[i];
                previousScales[i] = scales[i];
                return true;
            }
        }
        return false;
    }

    //public byte InitializationMethod() //when generating tiles, check if tiles near spawn should be different
    //{
    //    if (pos.y == spawnPosition.y - 1 && pos.x > spawnPosition.x - distanceFromSpawn / 2 && pos.x < spawnPosition.x + distanceFromSpawn / 2)
    //    {
    //        return 2;
    //    }
    //    if (!IsVoxelInWorld(pos) || (Mathf.Abs(pos.x - spawnPosition.x) + Mathf.Abs(pos.y - spawnPosition.y)) < distanceFromSpawn)
    //    {
    //        return 1;
    //    }
    //    //return desired byte+1, or 0 if no byte change is necessary
    //    return 0;
    //}

    [System.Serializable]
    public class TileType
    {
        public string tileName;
        public bool isSolid;
        public int textureID;
        public BaseItem item;
    }

    [System.Serializable]
    public class BiomeType
    {
        public float height;
        public float moisture;
        public float heat;

        public byte biomeID;
    }


    public enum GenerationMethod
    {
        Basic
    }

    byte BasicGeneration(Vector3 pos)
    {
        float moisture = Noise.Get2DPerlin(pos, offsets[0], scales[0]);
        float height = Noise.Get2DPerlin(pos, offsets[1], scales[1]);
        float heat = Noise.Get2DPerlin(pos, offsets[2], scales[2]);

        List<BiomeType> biomeTypesTemp = new List<BiomeType>();

        foreach (BiomeType bt in biomeTypes)
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
            if (tempDif < minDif)
            {
                minDif = tempDif;
                r = bt;
            }
        }

        if (r == null) return 0;
        return r.biomeID;
    }
}
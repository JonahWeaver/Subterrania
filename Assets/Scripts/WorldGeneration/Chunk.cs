using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk 
{
    public ChunkCoord coord;

    int vertIndex = 0;
    public List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();


    public GameObject chunkObject;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    byte[,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkWidth];

    World world;


    public Chunk(ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, coord.y * VoxelData.ChunkWidth, 0f);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.y;

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    void PopulateVoxelMap()
    {
        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.ChunkWidth; y++)
            {
                voxelMap[x, y] = world.GetVoxel(new Vector3(x, y, 0) + position);
            }
        }
    }

    void CreateMeshData()
    {
        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.ChunkWidth; y++)
            {
                AddVoxelDataToChunk(new Vector3(x, y, 0));
            }
        }
    }

    public bool isActive
    {
        get{ return chunkObject.activeSelf; }
        set{ chunkObject.SetActive(value); }
    }

    public Vector3 position
    {
        get { return chunkObject.transform.position; }
    }

    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || z !=0 || y < 0 || y > VoxelData.ChunkWidth - 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
        {
            return world.tileTypes[world.GetVoxel(pos + position)].isSolid;
        }

        return world.tileTypes[voxelMap[x, y]].isSolid;
    }
    void AddVoxelDataToChunk(Vector3 pos)
    {
        if (CheckVoxel(pos))
        {
            byte tileID = voxelMap[(int)pos.x, (int)pos.y];

            verts.Add(pos);
            verts.Add(pos + Vector3.up);
            verts.Add(pos + Vector3.right);
            verts.Add(pos + Vector3.up + Vector3.right);

            AddTexture(world.tileTypes[tileID].textureID);

            tris.Add(vertIndex);
            tris.Add(vertIndex + 1);
            tris.Add(vertIndex + 2);
            
            tris.Add(vertIndex + 2);
            tris.Add(vertIndex + 1);
            tris.Add(vertIndex + 3);

            vertIndex += 4;
        }
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }

    void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - y*VoxelData.TextureAtlasSizeInBlocks;

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
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
        if(other==null)
        {
            return false;
        }
        else if (other.x==x && other.y==y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

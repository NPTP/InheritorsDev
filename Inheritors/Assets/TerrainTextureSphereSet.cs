using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTextureSphereSet : MonoBehaviour
{
    [Header("TERRAIN TO MODIFY")]
    public Terrain terrain;
    [Tooltip("The sphere will only remap textures or remove details in a build, unless you check this box - then it will happen in the editor. Only do this in a safe testing environment!")]
    public bool overrideEditorSafety = false;

    [Header("TEXTURE REMAP")]
    [Space(10)]
    public bool remapTexture = false;
    public TerrainManager.Layers remapLayer;
    public int fallOffExponent = 3;

    [Header("GRASS DETAILS REMOVAL")]
    [Space(10)]
    public bool removeDetails = false;

    TerrainData td;
    int numSplatLayers;
    int splatSize; // Splatmap is a square
    int numDetailLayers;
    int detailSize; // Detail map is a square
    Vector3 center;
    float radius;
    int texturePosZ;
    int texturePosX;

    void Start()
    {
        if (Application.isEditor && !overrideEditorSafety)
        {
            return;
        }

        td = terrain.terrainData;
        numSplatLayers = td.alphamapLayers;
        center = transform.position;
        radius = transform.lossyScale.x / 2;

        StartCoroutine(Functionality());
    }

    IEnumerator Functionality()
    {
        // Wait a couple frames for loading to complete, so that we aren't
        // competing for who gets to set terrain data last.
        yield return null;
        yield return null;
        
        if (remapTexture)
            TextureRemap();

        if (removeDetails)
            GrassRemove();
    }

    void GrassRemove()
    {
        int grassDetailLayer = 0;

        detailSize = td.detailWidth; // detailWidth == detailHeight
        int[,] oldDetails = td.GetDetailLayer(0, 0, detailSize, detailSize, grassDetailLayer);
        int[,] newDetails = new int[detailSize, detailSize];
        for (int z = 0; z < detailSize; z++)
        {
            for (int x = 0; x < detailSize; x++)
            {
                Vector3 worldPos = ConvertToWorldSpace(x, z, detailSize);
                float distance = (worldPos - center).magnitude;
                if (distance <= radius)
                {
                    newDetails[z, x] = 0;
                }
                else
                {
                    newDetails[z, x] = oldDetails[z, x];
                }
            }
        }

        // for (int l = 0; l < td.detailPrototypes.Length; l++)
        td.SetDetailLayer(0, 0, grassDetailLayer, newDetails);
    }

    Vector3 ConvertToWorldSpace(int x, int z, int gridSize)
    {
        float xCoord = ((x * td.size.x) / gridSize) + terrain.transform.position.x;
        float zCoord = ((z * td.size.z) / gridSize) + terrain.transform.position.z;
        float yCoord = transform.position.y; // Get y from the sphere
        return new Vector3(xCoord, yCoord, zCoord);
    }

    void TextureRemap()
    {
        splatSize = td.alphamapWidth; // alphamapWidth == alphamapHeight
        float[,,] alphaMap = td.GetAlphamaps(0, 0, splatSize, splatSize);
        float[,,] remap = new float[splatSize, splatSize, numSplatLayers];
        int remapLayerNum = (int)remapLayer;

        for (int z = 0; z < splatSize; z++)
        {
            for (int x = 0; x < splatSize; x++)
            {
                // Convert from splatmap to world space
                Vector3 worldPos = ConvertToWorldSpace(x, z, splatSize);
                float distance = (worldPos - center).magnitude;
                float falloff = Mathf.Pow(distance / radius, fallOffExponent);

                for (int l = 0; l < numSplatLayers; l++)
                {
                    if (distance <= radius)
                    {
                        if (l == remapLayerNum)
                            remap[z, x, l] = 1f - falloff;
                        else
                            remap[z, x, l] = alphaMap[z, x, l] - (1f - falloff);

                        if (remap[z,x,l] < 0) {remap[z,x,l] = 0;}
                    }
                    else
                    {
                        remap[z, x, l] = alphaMap[z, x, l];
                    }
                }
            }
        }

        td.SetAlphamaps(0, 0, remap);
    }

    Vector3 ConvertSplatToWorldSpace(int x, int z)
    {
        float xCoord = ((x * td.size.x) / splatSize) + terrain.transform.position.x;
        float zCoord = ((z * td.size.z) / splatSize) + terrain.transform.position.z;
        float yCoord = transform.position.y; // Get y from the sphere
        return new Vector3(xCoord, yCoord, zCoord);
    }
}

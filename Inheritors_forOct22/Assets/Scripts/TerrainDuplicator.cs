using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TerrainDuplicator : MonoBehaviour
{
    public Terrain t; // The original we're copying
    public bool tryToMakeAsset = false;

    void Start()
    {
        CreateTerrain();
    }


    void CreateTerrain()
    {
        GameObject parent = (GameObject)Instantiate(new GameObject("Terrain"));
        parent.transform.position = new Vector3(0f, 0f, 14f);

        // for (int x = 1; x <= tileAmount.x; x++)
        //     for (int y = 1; y <= tileAmount.y; y++)
        string name = "Duplicated Terrain";

        TerrainData terrainData = new TerrainData();

        GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(terrainData);

        terrain.name = name;
        terrain.transform.parent = parent.transform;
        // terrain.transform.position = new Vector3(length * (x - 1), 0, width * (y - 1));
        terrain.transform.position = new Vector3(0f, 0f, 14f);

        int baseTextureResolution = t.terrainData.baseMapResolution;
        terrainData.baseMapResolution = baseTextureResolution;

        int heightmapResolution = t.terrainData.heightmapResolution;
        terrainData.heightmapResolution = heightmapResolution;

        int controlTextureResolution = t.terrainData.alphamapResolution;
        terrainData.alphamapResolution = controlTextureResolution;

        int detailResolution = t.terrainData.detailResolution;
        int detailResolutionPerPatch = t.terrainData.detailResolutionPerPatch;
        terrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

        terrainData.size = new Vector3(
            t.terrainData.size.x,
            t.terrainData.size.y,
            t.terrainData.size.z
        );

        terrainData.name = name;

        if (tryToMakeAsset)
        {
            string path = "Scenes/Tests/ResetTerrainTest/";
            AssetDatabase.CreateAsset(terrainData, "Assets/" + path + name + ".asset");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TerrainCopyTest : MonoBehaviour
{
    public Terrain originalTerrain; // The original we're copying
    TerrainData origData;
    public Terrain copyTerrain; // The template we want to copy onto
    public bool tryToMakeAssetInEditor = false; // Write a new terrain data to file on CreateTerrain?

    void Start()
    {
        origData = originalTerrain.terrainData;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            CreateTerrain();
        else if (Input.GetKeyDown(KeyCode.C))
            CopyTerrain();
    }

    void CopyTerrain()
    {
        /* Terrain we're copying onto should have the same size, heightmap/detail/alphamap/etc resolutions,
        ** and same detail and tree layers. We're just copying the "layout" so to speak, not the parameters. */

        // Just a shorter reference for the readability's sake
        TerrainData td = copyTerrain.terrainData;

        // Copy height data
        td.SetHeights(0, 0, origData.GetHeights(0, 0, origData.heightmapResolution, origData.heightmapResolution));

        // Copy texture map
        td.SetAlphamaps(0, 0, origData.GetAlphamaps(0, 0, origData.alphamapWidth, origData.alphamapHeight));

        // Copy detail layer
        for (int layer = 0; layer < origData.detailPrototypes.Length; layer++)
        {
            td.SetDetailLayer(0, 0, layer, origData.GetDetailLayer(
                0, 0, origData.detailWidth, origData.detailHeight, layer));
        }

        // Copy trees
        TreeInstance[] copyTrees = new TreeInstance[origData.treeInstances.Length];
        for (int tree = 0; tree < copyTrees.Length; tree++)
        {
            TreeInstance copiedTree = new TreeInstance();
            copiedTree.position = origData.treeInstances[tree].position;
            copiedTree.widthScale = origData.treeInstances[tree].widthScale;
            copiedTree.heightScale = origData.treeInstances[tree].heightScale;
            copiedTree.color = origData.treeInstances[tree].color;
            copiedTree.lightmapColor = origData.treeInstances[tree].lightmapColor;
            copiedTree.prototypeIndex = origData.treeInstances[tree].prototypeIndex;
            copyTrees[tree] = copiedTree;
        }
        td.treeInstances = copyTrees;
    }

    void CreateTerrain()
    {
        Vector3 desiredPosition = new Vector3(0f, 0f, 30f);

        // GameObject parent = (GameObject)Instantiate(new GameObject("DuplicateTerrainParent"));`
        // GameObject parent = new GameObject("DuplicateTerrainParent");
        // parent.transform.position = desiredPosition;
        // terrain.transform.parent = parent.transform;

        string name = "DuplicateTerrain";

        TerrainData terrainData = new TerrainData();

        GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(terrainData);

        terrain.name = name;
        terrainData.name = name + "Data";

        terrain.transform.position = desiredPosition;

        terrainData.baseMapResolution = origData.baseMapResolution;
        terrainData.heightmapResolution = origData.heightmapResolution;
        terrainData.alphamapResolution = origData.alphamapResolution;
        terrainData.SetDetailResolution(origData.detailResolution, origData.detailResolutionPerPatch);
        terrainData.size = new Vector3(origData.size.x, origData.size.y, origData.size.z);

        terrainData.SetHeights(0, 0, origData.GetHeights(0, 0, origData.heightmapResolution, origData.heightmapResolution));

#if UNITY_EDITOR
        if (tryToMakeAssetInEditor)
        {
            string path = "Scenes/Tests/ResetTerrainTest/";
            AssetDatabase.CreateAsset(terrainData, "Assets/" + path + name + ".asset");
        }
#endif
    }

}

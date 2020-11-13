#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class EditorTerrainTools : EditorWindow
{
    private string originalTerrainName = "OriginalTerrain";
    private string copyTerrainName = "Terrain";

    [MenuItem("Window/Devtools/Nick's Terrain Tools")]
    public static void ShowWindow()
    {
        GetWindow<EditorTerrainTools>("Nick's Terrain Tools");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Copy Terrain"))
        {
            CopyTerrain();
        }

        if (GUILayout.Button("Apply painting to original terrain"))
        {
            PaintTerrain(originalTerrainName);
        }

        if (GUILayout.Button("Apply painting to copied terrain"))
        {
            PaintTerrain(copyTerrainName);
        }
    }

    private void PaintTerrain(string terrainName)
    {
        int cliffLayer = (int)TerrainManager.Layers.Cliffside;
        int grassLayer = (int)TerrainManager.Layers.GrassLight;
        Terrain t = GameObject.Find(terrainName)?.GetComponent<Terrain>();
        int numLayers = t.terrainData.alphamapLayers;
        float[,,] alphaMap = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        float[,,] remap = new float[t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, numLayers];

        // For each point on the alphamap...
        for (int i = 0; i < t.terrainData.alphamapHeight; i++)
        {
            for (int j = 0; j < t.terrainData.alphamapWidth; j++)
            {
                // Get the normalized terrain coordinate that
                // corresponds to the the point.
                float normX = j * 1.0f / (t.terrainData.alphamapWidth - 1);
                float normY = i * 1.0f / (t.terrainData.alphamapHeight - 1);

                // Get the steepness value at the normalized coordinate.
                float angle = t.terrainData.GetSteepness(normX, normY);

                // Steepness is given as an angle, 0..90 degrees. Divide
                // by 90 to get an alpha blending value in the range 0..1.
                float frac = angle / 90f;
                for (int k = 0; k < numLayers; k++)
                {
                    if (frac > 0.0)
                    {
                        remap[i, j, k] = 0f;
                        remap[i, j, cliffLayer] = frac;
                        remap[i, j, grassLayer] = 1f - frac;
                    }
                    else
                    {
                        remap[i, j, k] = alphaMap[i, j, k];
                        // if (k == grassLayer)
                        //     remap[i, j, k] = 1;
                    }
                }

            }
        }
        t.terrainData.SetAlphamaps(0, 0, remap);
    }

    private void CopyTerrain()
    {
        /* Terrain we're copying onto should have the same size, heightmap/detail/alphamap/etc resolutions,
        ** and same detail and tree layers. We're just copying the "layout" so to speak, not the parameters. */
        TerrainData origData = GameObject.Find(originalTerrainName)?.GetComponent<Terrain>().terrainData;
        Terrain copyTerrain = GameObject.Find(copyTerrainName)?.GetComponent<Terrain>();

        if (copyTerrain == null || origData == null)
        {
            Debug.Log("You must name the original terrain '" + originalTerrainName +
            "' and the terrain to be copied into '" + copyTerrainName
            + "' in order for the copy function to work.");
            return;
        }

        // Just a shorter reference for the readability's sake
        TerrainData td = copyTerrain.terrainData;

        // Copy height data
        td.SetHeights(0, 0, origData.GetHeights(0, 0, origData.heightmapResolution, origData.heightmapResolution));

        // Copy texture remap
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

        Debug.Log("Successfully copied terrain from " + originalTerrainName + " into " + copyTerrainName);
    }
}
#endif
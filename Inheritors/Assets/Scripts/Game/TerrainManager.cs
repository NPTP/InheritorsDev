using UnityEngine;

// Current terrainbuilder params are saved in CSC494/Current Terrainbuilder Params.png
public class TerrainManager : MonoBehaviour
{
    private string originalTerrainName = "OriginalTerrain";
    private string copyTerrainName = "Terrain";
    public bool copyTerrainOnLoad = false;

    public enum Layers
    {
        GrassLight,
        GrassDark,
        DirtLight,
        DirtDark,
        Dust,
        LeavesYellow,
        LeavesGreen,
        LeavesBrown,
        AshDark,
        Cliffside,
        Farm,
        Wood,
        Water,
        Trail,
    }

    void Start()
    {
        if (copyTerrainOnLoad)
            CopyTerrain();
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

        Debug.Log("Successfully copied terrain from " + originalTerrainName + " into " + copyTerrainName);
    }
}

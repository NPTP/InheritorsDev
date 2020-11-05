#if UNITY_EDITOR
using System.Collections;
using UnityEngine;

public class EditorTerrainReset : MonoBehaviour
{

    [Tooltip("The original terrain we're copying")]
    public Terrain originalTerrain;
    TerrainData origData; // The original terrain data
    [Tooltip("The terrain we want to copy into")]
    public Terrain copyTerrain;
    [Tooltip("How long to hold 'T' before the terrain is copied over")]
    public int resetHoldTime = 100;

    bool holdingReset = false;

    void Start()
    {
        origData = originalTerrain.terrainData;
    }

    void OnGUI()
    {
        if (holdingReset)
        {
            GUILayout.Button("Keep holding T to reset terrain...");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            holdingReset = true;
            StartCoroutine(CopyTerrainCountdown());
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            holdingReset = false;
        }
    }

    IEnumerator CopyTerrainCountdown()
    {
        int timer = 0;
        while (holdingReset && timer < resetHoldTime)
        {
            timer += 1;
            yield return new WaitForSeconds(0.001f);
            Debug.Log(timer + " | " + resetHoldTime);
        }

        if (timer >= resetHoldTime)
        {
            Debug.Log("Reset terrain!");
            holdingReset = false;
            CopyTerrain();
        }
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
}
#endif
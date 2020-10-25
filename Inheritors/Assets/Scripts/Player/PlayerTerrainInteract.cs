using UnityEngine;

/*---- LAYER REFERENCE ----

-------------------------*/

public class PlayerTerrainInteract : MonoBehaviour
{
    public bool leavePaths = true;
    public bool cutGrass = true;
    Transform playerTransform;
    Terrain t;

    public int texturePosZ;
    public int texturePosX;
    public int detailPosZ;
    public int detailPosX;

    float[] texturesUnderfoot;
    float[,,] alphaMap;
    int numLayers;
    public int trailSize = 3;
    private int playerSplatmapSize;

    bool[,] walkedMap;
    float[,,] debugWalked;

    void Start()
    {
        playerTransform = GetComponent<Transform>();
        t = GameObject.Find("Terrain").GetComponent<Terrain>();
        numLayers = t.terrainData.alphamapLayers;
        playerSplatmapSize = (int)trailSize / 2;
        texturesUnderfoot = new float[numLayers];

        // Splat map width = splat map height. Irrelevant distinction
        int width = t.terrainData.alphamapWidth;
        int height = t.terrainData.alphamapHeight;
        debugWalked = new float[width, height, numLayers];
        InitializeWalkedMap();
        InitializeDebugWalked();

        Debug.Log(t.terrainData.alphamapWidth);
        Debug.Log(t.terrainData.alphamapHeight);
    }

    void Update()
    {
        ConvertPosition(playerTransform.position);
        CheckTextureUnderfoot();

        if (true) // TODO: check a new function IsTouchingTerrain() here (simple raycast check?)
        {
            if (leavePaths)
                ChangeTexture();

            if (cutGrass)
                RemoveDetails();
        }

        // TakeDebugInputs();
    }

    // Use for testing terrain modifications in debug.
    void TakeDebugInputs()
    {
        // Test walked map
        if (Input.GetKeyDown(KeyCode.W))
        {
            for (int i = 0; i < t.terrainData.detailWidth; i++)
                for (int j = 0; j < t.terrainData.detailHeight; j++)
                    if (walkedMap[i, j])
                        debugWalked[i, j, 0] = 1f;
            t.terrainData.SetAlphamaps(0, 0, debugWalked);
        }
        // Remove all details layer 0
        if (Input.GetKeyDown(KeyCode.R))
        {
            int[,] details = new int[t.terrainData.detailWidth, t.terrainData.detailHeight];
            for (int i = 0; i < t.terrainData.detailWidth; i++)
                for (int j = 0; j < t.terrainData.detailHeight; j++)
                    details[i, j] = 0;
            t.terrainData.SetDetailLayer(0, 0, 0, details);
        }
        // Fill terrain with details layer 0
        else if (Input.GetKeyDown(KeyCode.T))
        {
            int[,] details = new int[t.terrainData.detailWidth, t.terrainData.detailHeight];
            for (int i = 0; i < t.terrainData.detailWidth; i++)
                for (int j = 0; j < t.terrainData.detailHeight; j++)
                    details[i, j] = 8;
            t.terrainData.SetDetailLayer(0, 0, 0, details);
        }
    }

    void ChangeTexture()
    {
        float[,,] remap = new float[trailSize, trailSize, numLayers];
        for (int i = 0; i < trailSize; i++)
        {
            for (int j = 0; j < trailSize; j++)
            {
                for (int k = 0; k < numLayers; k++)
                {
                    if (k == 1)
                        remap[i, j, k] = 1f;
                    else
                        remap[i, j, k] = 0f;
                }
                // Record that we have walked here
                walkedMap[texturePosZ - playerSplatmapSize + i, texturePosX - playerSplatmapSize + j] = true;
            }
        }
        t.terrainData.SetAlphamaps(texturePosX - playerSplatmapSize, texturePosZ - playerSplatmapSize, remap);
    }

    void RemoveDetails()
    {
        int areaSize = trailSize;
        int[,] details = new int[areaSize, areaSize];
        for (int i = 0; i < areaSize; i++)
            for (int j = 0; j < areaSize; j++)
                details[i, j] = 0;
        t.terrainData.SetDetailLayer(detailPosX, detailPosZ, 0, details);
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;
        Vector3 mapPosition = new Vector3(
            terrainPosition.x / t.terrainData.size.x,
            0f,
            terrainPosition.z / t.terrainData.size.z
        );

        texturePosZ = (int)(mapPosition.z * t.terrainData.alphamapHeight);
        texturePosX = (int)(mapPosition.x * t.terrainData.alphamapWidth);

        detailPosX = (int)(mapPosition.x * t.terrainData.detailWidth);
        detailPosZ = (int)(mapPosition.z * t.terrainData.detailHeight);
    }

    // Stores the underfoot texture mix per layer in texturesUnderfoot.
    void CheckTextureUnderfoot()
    {
        alphaMap = t.terrainData.GetAlphamaps(texturePosX, texturePosZ, 1, 1);
        for (int k = 0; k < numLayers; k++)
            texturesUnderfoot[k] = alphaMap[0, 0, k];
    }

    void InitializeWalkedMap()
    {
        int width = t.terrainData.alphamapWidth;
        int height = t.terrainData.alphamapHeight;
        walkedMap = new bool[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                walkedMap[i, j] = false;
    }

    void InitializeDebugWalked()
    {
        int width = t.terrainData.alphamapWidth;
        int height = t.terrainData.alphamapHeight;
        float[,,] origMaps = t.terrainData.GetAlphamaps(0, 0, width, height);
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                for (int k = 0; k < numLayers; k++)
                    debugWalked[i, j, k] = 0f;

        t.terrainData.SetAlphamaps(0, 0, debugWalked);
    }
}

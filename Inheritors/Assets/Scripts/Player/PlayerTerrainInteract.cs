using UnityEngine;

/*---- LAYER REFERENCE ----

-------------------------*/

public class PlayerTerrainInteract : MonoBehaviour
{
    public bool leavePaths = true;
    public bool cutGrass = true;
    Transform playerTransform;
    Terrain t;

    public int texturePosX;
    public int texturePosZ;
    public int detailPosX;
    public int detailPosZ;
    float[] textureValues;
    float[,,] alphaMap;
    bool[,] walkedMap; /* Keeps track of which terrain splat map coordinates have been walked on. */
    int numLayers;
    public int trailSize = 3;
    private int playerSplatmapRadius;
    int foremostLayer = 0;

    void Start()
    {
        playerTransform = GetComponent<Transform>();
        t = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        numLayers = t.terrainData.alphamapLayers;
        playerSplatmapRadius = (int)trailSize / 2;
        textureValues = new float[numLayers];
        InitializeWalkedMap();
    }

    void Update()
    {
        CheckTexture();
        ConvertPosition(playerTransform.position);

        if (leavePaths) // && isGrounded - check this in the new player type
            ChangeTexture();
        if (cutGrass) // && isGrounded - check this in the new player type
            RemoveDetails();

        // TakeDebugInputs();
    }

    // Use for testing terrain modifications in debug.
    void TakeDebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            int[,] details = new int[t.terrainData.detailWidth, t.terrainData.detailHeight];
            for (int i = 0; i < t.terrainData.detailWidth; i++)
                for (int j = 0; j < t.terrainData.detailHeight; j++)
                    details[i, j] = 0;
            t.terrainData.SetDetailLayer(0, 0, 0, details);
        }
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
                if (!walkedMap[i, j])
                {
                    for (int k = 0; k < numLayers; k++)
                    {
                        // TODO: find way to mark dirty so this can only happen once to each section.
                        // May require creating a duplicate array of 1/0 bools same size as terrain splatmap.
                        // Messing with the API's mark-dirty stuff may fuck up GPU stuff
                        if (k == 1)
                            remap[i, j, k] = 1f;
                        else
                            remap[i, j, k] = 0f;
                    }
                    // This line is causing crashes for some reason:
                    // walkedMap[texturePosX - playerSplatmapRadius + i, texturePosZ - playerSplatmapRadius + j] = true;
                }
            }
        }
        t.terrainData.SetAlphamaps(texturePosX - playerSplatmapRadius, texturePosZ - playerSplatmapRadius, remap);
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

        texturePosX = (int)(mapPosition.x * t.terrainData.alphamapWidth);
        texturePosZ = (int)(mapPosition.z * t.terrainData.alphamapHeight);

        detailPosX = (int)(mapPosition.x * t.terrainData.detailWidth);
        detailPosZ = (int)(mapPosition.z * t.terrainData.detailHeight);
    }

    // Stores the underfoot texture mix per layer in textureValues.
    void CheckTexture()
    {
        alphaMap = t.terrainData.GetAlphamaps(texturePosX, texturePosZ, trailSize, trailSize);

        for (int i = 0; i < trailSize; i++)
            for (int j = 0; j < trailSize; j++)
                for (int k = 0; k < numLayers; k++)
                    textureValues[k] = alphaMap[0, 0, k];
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
}

using UnityEngine;

/*---- LAYER REFERENCE ----

-------------------------*/

public class PlayerTerrainInteract : MonoBehaviour
{
    public bool leavePaths = true;
    Transform playerTransform;
    CharacterController characterController;
    Terrain t;

    public int posX;
    public int posZ;
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
        characterController = GetComponent<CharacterController>();
        t = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        numLayers = t.terrainData.alphamapLayers;
        playerSplatmapRadius = (int)trailSize / 2;
        textureValues = new float[numLayers];
        InitializeWalkedMap();
    }

    void Update()
    {
        if (leavePaths) // && characterController.isGrounded
        {
            ConvertPosition(playerTransform.position);
            CheckTexture();
            ChangeTexture();
        }

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
                    }
                    // This line is causing crashes for some reason:
                    // walkedMap[posX - playerSplatmapRadius + i, posZ - playerSplatmapRadius + j] = true;
                }
            }
        }
        t.terrainData.SetAlphamaps(posX - playerSplatmapRadius, posZ - playerSplatmapRadius, remap);

        // Walking over detail layer and removing grass underfoot
        // NOTE: Strange Unity shit afoot. This only seems to work if
        // - Detail resolution per patch: 8
        // - Detail resolution: 512
        // And we happen to be on terrain size 100, if that matters.
        int[,] details = new int[trailSize, trailSize];
        for (int i = 0; i < trailSize; i++)
            for (int j = 0; j < trailSize; j++)
                details[i, j] = 0;
        t.terrainData.SetDetailLayer(posX, posZ, 0, details);
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;

        Vector3 mapPosition = new Vector3(
            terrainPosition.x / t.terrainData.size.x,
            0f,
            terrainPosition.z / t.terrainData.size.z
        );

        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;

        posX = (int)xCoord;
        posZ = (int)zCoord;
    }

    // Stores the underfoot texture mix in textureValues, and
    // saves the index of the most prominent texture of the bunch
    // (so we know what we're mainly walking on).
    void CheckTexture()
    {
        alphaMap = t.terrainData.GetAlphamaps(posX, posZ, trailSize, trailSize);
        float max = -1f;

        for (int i = 0; i < trailSize; i++)
        {
            for (int j = 0; j < trailSize; j++)
            {
                for (int k = 0; k < numLayers; k++)
                {
                    textureValues[k] = alphaMap[0, 0, k];
                    if (textureValues[k] > max)
                    {
                        max = textureValues[k];
                        foremostLayer = k;
                    }
                }
            }
        }
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

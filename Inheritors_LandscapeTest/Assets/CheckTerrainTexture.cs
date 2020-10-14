using UnityEngine;

/*---- LAYER REFERENCE ----
0 - Grass
1 - Unchangeable light dirt
2 - Ash
3 - Dark dirt
-------------------------*/

public class CheckTerrainTexture : MonoBehaviour
{
    public Transform playerTransform;
    public Terrain t;
    public CharacterController characterController;

    public int posX;
    public int posZ;
    public float[] textureValues;
    int numLayers;
    int trailSize = 3;
    int foremostLayer = 0;

    void Start()
    {
        numLayers = t.terrainData.alphamapLayers;
        textureValues = new float[numLayers];
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            ConvertPosition(playerTransform.position);
            CheckTexture();
            ChangeTexture();
        }
    }

    void ChangeTexture()
    {
        int xSize, zSize;
        xSize = zSize = trailSize;

        // TODO: make CheckTexture() collect a trailSize x trailSize 3d array to match this function,
        // so we can compare appropriately.
        float unchangeableFactor = textureValues[1];

        float[,,] map = new float[xSize, zSize, numLayers];
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                for (int k = 0; k < numLayers; k++)
                {
                    // TODO: find way to mark dirty so this can only happen once to each section.
                    // May require creating a duplicate array of 1/0 bools same size as terrain splatmap.
                    // Messing with the API's mark-dirty stuff may fuck up GPU stuff

                    if (k == 3)
                        map[i, j, k] = 1f - unchangeableFactor;
                    else
                        map[i, j, k] = textureValues[k];
                }
            }
        }
        t.terrainData.SetAlphamaps(posX, posZ, map);

        int w, h;
        w = h = 12;
        int[,] details = new int[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                details[i, j] = 0;
            }
        }
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
        float[,,] alphaMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);
        float max = -1f;
        for (int i = 0; i < numLayers; i++)
        {
            textureValues[i] = alphaMap[0, 0, i];
            if (textureValues[i] > max)
            {
                max = textureValues[i];
                foremostLayer = i;
            }
        }
    }
}

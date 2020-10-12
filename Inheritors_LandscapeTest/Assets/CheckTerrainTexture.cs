using UnityEngine;

public class CheckTerrainTexture : MonoBehaviour
{
    public Transform playerTransform;
    public Terrain t;
    public CharacterController characterController;

    public int posX;
    public int posZ;
    public float[] textureValues;
    public int numTextures = 3;

    void Start()
    {
        textureValues = new float[numTextures];
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

    // TODO: set alphamaps more smoothly.
    void ChangeTexture()
    {
        int xSize = 6;
        int zSize = 6;
        float[,,] map = new float[xSize, zSize, 3];
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                map[i, j, 0] = 0f;
                map[i, j, 1] = 1f;
                map[i, j, 2] = 0f;
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

    void CheckTexture()
    {
        float[,,] alphaMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);

        // TODO: make this auto generate based on the number of terrain textures
        textureValues[0] = alphaMap[0, 0, 0];
        textureValues[1] = alphaMap[0, 0, 1];
        textureValues[2] = alphaMap[0, 0, 2];
    }
}

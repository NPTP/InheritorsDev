using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerFootstepFX))]
[RequireComponent(typeof(AudioSource))]
public class PlayerTerrainInteract : MonoBehaviour
{
    PlayerMovement playerMovement;
    Animator animator;
    PlayerFootstepFX playerFootstepFX;

    public Terrain t;
    public bool leavePaths = true;
    public bool cutGrass = true;
    public int trailSize = 1;
    public float trailAmount = .25f;
    public int grassCutSize = 3;
    public float stepDeformDepth = 0.0002f;

    Transform playerTransform;

    int texturePosZ;
    int texturePosX;
    int detailPosZ;
    int detailPosX;
    int heightPosY;
    int heightPosX;

    float[] texturesUnderfoot;
    float[,,] alphaMap;
    int numLayers;
    int playerSplatmapSize;
    int playerDetailMapSize;
    int trailLayer = (int)TerrainManager.Layers.Trail;

    bool[,] walkedToday;
    float[,] pastTrail;

    float moveSpeed;
    float stepPrev = -1;
    float step = -1;
    bool steppedThisFrame = false;

    void Start()
    {
        if (!Application.isEditor)
        {
            leavePaths = true;
            cutGrass = true;
        }

        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        playerFootstepFX = GetComponent<PlayerFootstepFX>();

        playerTransform = GetComponent<Transform>();
        numLayers = t.terrainData.alphamapLayers;
        playerSplatmapSize = trailSize / 2;
        playerDetailMapSize = grassCutSize / 2;
        texturesUnderfoot = new float[numLayers];

        // Splat map is always a square so width = height. Irrelevant distinction
        int width = t.terrainData.alphamapWidth;
        int height = t.terrainData.alphamapHeight;
        debugWalked = new float[width, height, numLayers];
        InitializeWalkedToday();
        // InitializeDebugWalked();
    }

    void Update()
    {
        if (playerMovement.m_isGrounded)
        {
            ConvertPosition(playerTransform.position);
            FootstepFX();

            if (leavePaths)
                ChangeTexture(trailSize);

            if (cutGrass)
                RemoveDetails(grassCutSize);
        }

        // TakeDebugInputs();
    }

    void ChangeTexture(int areaSize)
    {
        bool changedTex = false;
        alphaMap = t.terrainData.GetAlphamaps(texturePosX - playerSplatmapSize, texturePosZ - playerSplatmapSize, areaSize, areaSize);
        float[,,] remap = new float[areaSize, areaSize, numLayers];
        for (int i = 0; i < areaSize; i++)
        {
            for (int j = 0; j < areaSize; j++)
            {
                for (int k = 0; k < numLayers; k++)
                {
                    remap[i, j, k] = alphaMap[i, j, k];
                }

                // Flipped order intentionally
                int z = texturePosZ - playerSplatmapSize + i;
                int x = texturePosX - playerSplatmapSize + j;

                // Check if we've walked here already today - if not, lay down some trail (limit at 1f).
                if (!walkedToday[z, x] && alphaMap[i, j, trailLayer] < 1)
                {
                    walkedToday[z, x] = true;
                    changedTex = true;
                    remap[i, j, trailLayer] = alphaMap[i, j, trailLayer] + trailAmount;

                    // if (steppedThisFrame)
                    // {
                    //     float[,] heights = t.terrainData.GetHeights(heightPosX, heightPosY, 1, 1);
                    //     float[,] newHeights = new float[1, 1];
                    //     newHeights[0, 0] = heights[0, 0] - 0.0002f;
                    //     t.terrainData.SetHeightsDelayLOD(heightPosX, heightPosY, newHeights);
                    //     steppedThisFrame = false;
                    // }
                }
            }
        }
        if (changedTex) // Performance boost. SetAlphaMaps is the bottleneck, don't call it when on an already-walked position.
            t.terrainData.SetAlphamaps(texturePosX - playerSplatmapSize, texturePosZ - playerSplatmapSize, remap);
    }

    void RemoveDetails(int areaSize)
    {
        int[,] details = new int[areaSize, areaSize];
        for (int i = 0; i < areaSize; i++)
            for (int j = 0; j < areaSize; j++)
                details[i, j] = 0;

        for (int k = 0; k < t.terrainData.detailPrototypes.Length; k++)
            t.terrainData.SetDetailLayer(detailPosX - playerDetailMapSize, detailPosZ - playerDetailMapSize, k, details);
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

        heightPosX = (int)(mapPosition.x * t.terrainData.heightmapResolution);
        heightPosY = (int)(mapPosition.z * t.terrainData.heightmapResolution);
    }

    // Stores the underfoot texture mix per layer in texturesUnderfoot and return it.
    float[] GetTexturesUnderfoot()
    {
        alphaMap = t.terrainData.GetAlphamaps(texturePosX, texturePosZ, 1, 1);
        for (int k = 0; k < numLayers; k++)
            texturesUnderfoot[k] = alphaMap[0, 0, k];
        return texturesUnderfoot;
    }

    void LandingFX()
    {
        playerFootstepFX.PlayFX(GetTexturesUnderfoot());
        steppedThisFrame = true;
    }

    void FootstepFX()
    {
        moveSpeed = animator.GetFloat("MoveSpeed");
        stepPrev = step;
        step = animator.GetFloat("Footstep");

        if (moveSpeed > 0 && stepPrev < 0 && 0 <= step)
        {
            playerFootstepFX.PlayFX(GetTexturesUnderfoot());
            steppedThisFrame = true;
        }
    }

    void InitializeWalkedToday()
    {
        int width = t.terrainData.alphamapWidth;
        int height = t.terrainData.alphamapHeight;
        walkedToday = new bool[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                walkedToday[i, j] = false;
    }

    // ████████████████████████████████████████████████████████████████████████
    // ███ DEBUG
    // ████████████████████████████████████████████████████████████████████████

    // Use for testing terrain modifications in debug.
    void TakeDebugInputs()
    {
        // Test walked map
        if (Input.GetKeyDown(KeyCode.W))
        {
            for (int i = 0; i < t.terrainData.detailWidth; i++)
                for (int j = 0; j < t.terrainData.detailHeight; j++)
                    if (walkedToday[i, j])
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

    // float array for debugging
    float[,,] debugWalked;

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

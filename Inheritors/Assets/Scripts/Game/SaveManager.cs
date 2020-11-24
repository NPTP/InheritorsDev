
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Save
{
    public int dayNumber = 0;

    // Terrain data details
    public float[,] heights;
    public float[,,] alphaMaps;
    public int detailPrototypesLength;
    public List<int[,]> detailLayers;
}

public class SaveManager : MonoBehaviour
{
    public bool enableSaveManager = true;
    string filePath;
    Save save;
    TerrainData todayTerrainData;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/save.data";
        todayTerrainData = GameObject.FindWithTag("Terrain").GetComponent<Terrain>().terrainData;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("continuing"))
        {
            PlayerPrefs.DeleteKey("continuing");
            PlayerPrefs.Save();
            LoadGame();
        }
    }

    // DEBUG ONLY
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.P))
    //     {
    //         SaveGame(SceneManager.GetActiveScene().buildIndex - 1);
    //     }
    // }

    /// <summary>
    /// Should only be used at the end of a day. Save the current state of the terrain,
    /// and increment the given day number by 1 so that on loading we know to start on the next day.
    /// </summary>
    public void SaveGame(int dayNumber)
    {
        if (!enableSaveManager) return;

        print("Saving day: " + dayNumber);

        save = new Save();

        SaveTerrain();
        save.dayNumber = dayNumber + 1;

        FileStream dataStream = new FileStream(filePath, FileMode.Create);
        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, save);
        dataStream.Close();

        PlayerPrefs.SetInt("saveExists", 1);
        PlayerPrefs.SetInt("savedDayNumber", dayNumber);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (!enableSaveManager) return;

        if (File.Exists(filePath))
        {
            // File exists 
            FileStream dataStream = new FileStream(filePath, FileMode.Open);

            BinaryFormatter converter = new BinaryFormatter();
            save = converter.Deserialize(dataStream) as Save;

            print("Loaded save for day #: " + save.dayNumber);

            dataStream.Close();

            // // Check that we're loading the right day here.
            // if (save.dayNumber != dayNumber)
            // {
            //     print("Trying to load the wrong day number.");
            //     return;
            // }

            LoadTerrain();
        }
        else
        {
            print("Save file does not exist!");
        }
    }

    private void SaveTerrain()
    {
        save.heights = todayTerrainData.GetHeights(0, 0, todayTerrainData.heightmapResolution, todayTerrainData.heightmapResolution);
        save.alphaMaps = todayTerrainData.GetAlphamaps(0, 0, todayTerrainData.alphamapWidth, todayTerrainData.alphamapHeight);
        save.detailPrototypesLength = todayTerrainData.detailPrototypes.Length;
        save.detailLayers = new List<int[,]>();
        for (int layer = 0; layer < todayTerrainData.detailPrototypes.Length; layer++)
        {
            save.detailLayers.Add(todayTerrainData.GetDetailLayer(0, 0, todayTerrainData.detailWidth, todayTerrainData.detailHeight, layer));
        }
    }

    private void LoadTerrain()
    {
        /* Terrain we're copying onto should have the same size, heightmap/detail/alphamap/etc resolutions,
        ** and same detail and tree layers. We're just copying the "layout" so to speak, not the parameters. */

        // Load height data
        todayTerrainData.SetHeights(0, 0, save.heights);

        // Load texture map
        todayTerrainData.SetAlphamaps(0, 0, save.alphaMaps);

        // Load detail layer
        for (int layer = 0; layer < save.detailPrototypesLength; layer++)
        {
            todayTerrainData.SetDetailLayer(0, 0, layer, save.detailLayers[layer]);
        }
    }

}

/* INHERITORS by Nick Perrin (c) 2020 */
using System.Runtime.Serialization;
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

    public List<SampleBuffer> recordings;

    // Terrain data details
    public float[,] heights;
    public float[,,] alphaMaps;
    public int detailPrototypesLength;
    public List<int[,]> detailLayers;
}

public class SaveManager : MonoBehaviour
{
    public bool enableSavingLoading = true;
    string filePath;
    Save save;
    RecordManager recordManager;
    TerrainData todayTerrainData;

    void Awake()
    {
        filePath = Application.persistentDataPath + "/save.data";
        recordManager = FindObjectOfType<RecordManager>();
        todayTerrainData = GameObject.FindWithTag("Terrain").GetComponent<Terrain>().terrainData;
    }

    void Start()
    {
        print("Checking playerprefs in savemanager");
        if (PlayerPrefs.HasKey("continuing"))
        {
            PlayerPrefs.DeleteKey("continuing");
            PlayerPrefs.Save();
            LoadGame("All");
            recordManager.Begin();
            print("had the pref 1");
        }
        else if (PlayerPrefs.GetInt("currentDayNumber", -1) > 0)
        {
            LoadGame("Recordings");
            recordManager.Begin();
            print("had the pref 2");
        }
        else
        {
            print("Didn't load any saves.");
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
        if (!enableSavingLoading) return;

        save = new Save();
        print("Saving day: " + dayNumber);

        // Recordings
        save.recordings = recordManager.GetCombinedRecordings();

        // Terrain
        SaveTerrain();
        save.dayNumber = dayNumber + 1;

        // Serialization
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        AddSurrogates(binaryFormatter);
        binaryFormatter.Serialize(fileStream, save);
        fileStream.Close();

        // PlayerPrefs bookkeeping
        PlayerPrefs.SetInt("saveExists", 1);
        PlayerPrefs.SetInt("savedDayNumber", dayNumber);
        PlayerPrefs.Save();
    }

    public void LoadGame(string parameters)
    {
        if (!enableSavingLoading) return;

        if (File.Exists(filePath))
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            AddSurrogates(binaryFormatter);
            save = binaryFormatter.Deserialize(fileStream) as Save;
            fileStream.Close();
            print("Loaded save for day #: " + save.dayNumber);

            switch (parameters.ToLower())
            {
                case "all":
                    recordManager.LoadRecordings(save.recordings);
                    LoadTerrain();
                    break;

                case "recordings":
                    recordManager.LoadRecordings(save.recordings);
                    break;

                case "terrain":
                    LoadTerrain();
                    break;

                default:
                    print("Unknown load params in SaveManager, couldn't load");
                    break;
            }
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

    void AddSurrogates(BinaryFormatter bf)
    {
        // Construct a SurrogateSelector object
        SurrogateSelector ss = new SurrogateSelector();

        // Add surrogate for: Vector3
        ss.AddSurrogate(typeof(Vector3),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector3SerializationSurrogate());

        // Add surrogate for: Quaternion
        ss.AddSurrogate(typeof(Quaternion),
                        new StreamingContext(StreamingContextStates.All),
                        new QuaternionSerializationSurrogate());

        // Have the formatter use each
        bf.SurrogateSelector = ss;
    }

}


// BinaryFormatter bf = new BinaryFormatter();

// // 1. Construct a SurrogateSelector object
// SurrogateSelector ss = new SurrogateSelector();

// Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
// ss.AddSurrogate(typeof(Vector3),
//                 new StreamingContext(StreamingContextStates.All),
//                 v3ss);

// // 2. Have the formatter use our surrogate selector
// bf.SurrogateSelector = ss;
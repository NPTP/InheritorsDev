using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaterials : MonoBehaviour
{
    private Renderer rdr;
    private Material[] materials;
    public Material grass;
    public Material dirt;
    public Material gravel;
    private int materialIndex = 0;


    void Start()
    {
        rdr = gameObject.GetComponent<Renderer>();
        materials = new Material[3];
        materials[0] = grass;
        materials[1] = dirt;
        materials[2] = gravel;
    }

    public void changeToNextMaterial()
    {
        if (++materialIndex < materials.Length)
            rdr.material = materials[materialIndex];
    }
}

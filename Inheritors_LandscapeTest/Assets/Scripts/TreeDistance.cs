using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDistance : MonoBehaviour
{
    public Terrain terrain;
    void Start()
    {
        terrain.treeDistance = 10000f;
    }
}

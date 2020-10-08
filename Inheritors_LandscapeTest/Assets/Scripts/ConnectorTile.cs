using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorTile : MonoBehaviour
{
    private Renderer rdr;
    public Material dirt_none;
    public Material dirt_n;
    public Material dirt_s;
    public Material dirt_ns;
    public Material gravel;
    public GameObject northTile;
    public GameObject southTile;
    private bool changedToDirt = false;
    private bool changedToGravel = false;


    void Start()
    {
        rdr = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        bool northDirty = northTile.GetComponent<ConnectorTile>().changedToDirt;
        bool southDirty = southTile.GetComponent<ConnectorTile>().changedToDirt;

        if (changedToDirt && !changedToGravel)
        {
            if (northDirty && southDirty)
                rdr.material = dirt_ns;
            else if (southDirty)
                rdr.material = dirt_s;
            else if (northDirty)
                rdr.material = dirt_n;
        }
    }

    public void changeToNextMaterial()
    {
        if (!changedToDirt)
        {
            rdr.material = dirt_none;
            changedToDirt = true;
        }
    }

    public void changeMaterialGhost()
    {
        rdr.material = gravel;
        changedToGravel = true;
    }
}

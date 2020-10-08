using UnityEngine;

public class GhostTileWalk : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tile")
        {
            collision.gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.tag == "Tile")
        {
            hit.collider.GetComponent<ConnectorTile>().changeMaterialGhost();
        }
    }
}

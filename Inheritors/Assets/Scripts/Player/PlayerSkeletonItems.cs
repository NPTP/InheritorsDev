using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkeletonItems : MonoBehaviour
{
    GameObject playerBow;

    void Awake()
    {
        playerBow = GameObject.Find("PlayerBow");
        playerBow.SetActive(false);
    }

    public GameObject GetItem(ItemType type)
    {
        GameObject item = null;
        switch (type)
        {
            case ItemType.Bow:
                item = playerBow;
                break;

            default:
                print("Don't know this type in skeleton items GetItem().");
                break;
        }
        return item;
    }
}

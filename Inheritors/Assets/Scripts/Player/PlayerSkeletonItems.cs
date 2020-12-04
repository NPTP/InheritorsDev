using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkeletonItems : MonoBehaviour
{
    GameObject playerBow;
    GameObject playerPapaya;

    void Awake()
    {
        playerBow = GameObject.Find("PlayerBow");
        playerBow.SetActive(false);

        playerPapaya = GameObject.Find("PlayerPapaya");
        playerPapaya.SetActive(false);
    }

    public GameObject GetItem(ItemType type)
    {
        GameObject item = null;
        switch (type)
        {
            case ItemType.Bow:
                item = playerBow;
                break;

            case ItemType.Papaya:
                item = playerPapaya;
                break;

            default:
                print("Don't know this type in skeleton items GetItem().");
                break;
        }
        return item;
    }
}

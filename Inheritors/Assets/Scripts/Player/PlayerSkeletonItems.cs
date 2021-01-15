using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkeletonItems : MonoBehaviour
{
    GameObject playerBow;
    GameObject playerPapaya;
    GameObject playerCorn;
    GameObject playerYopo;
    GameObject playerHerbs;
    GameObject playerDarkFlute;

    void Awake()
    {
        playerBow = GameObject.Find("PlayerBow");
        playerBow.SetActive(false);

        playerPapaya = GameObject.Find("PlayerPapaya");
        playerPapaya.SetActive(false);

        playerCorn = GameObject.Find("PlayerCorn");
        playerCorn.SetActive(false);

        playerYopo = GameObject.Find("PlayerYopo");
        playerYopo.SetActive(false);

        playerHerbs = GameObject.Find("PlayerHerbs");
        playerHerbs.SetActive(false);

        playerDarkFlute = GameObject.Find("PlayerDarkFlute");
        playerDarkFlute.SetActive(false);
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

            case ItemType.Corn:
                item = playerCorn;
                break;

            case ItemType.Yopo:
                item = playerYopo;
                break;

            case ItemType.Herbs:
                item = playerHerbs;
                break;

            case ItemType.DarkFlute:
                item = playerDarkFlute;
                break;

            default:
                print("Don't know this type in PlayerSkeletonItems : GetItem().");
                break;
        }
        return item;
    }
}

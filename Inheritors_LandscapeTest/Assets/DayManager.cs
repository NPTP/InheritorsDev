using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    GameObject day1, day2, player;
    public GameObject currentDay;
    void Start()
    {
        player = GameObject.Find("Player");

        day1 = GameObject.Find("Day1");
        day1.SetActive(true);
        currentDay = day1;
        day2 = GameObject.Find("Day2");
        day2.SetActive(false);

        player.transform.position = GameObject.Find("Day1PlayerStart").transform.position;
        player.transform.rotation = GameObject.Find("Day1PlayerStart").transform.rotation;
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            day2.SetActive(false);
            day1.SetActive(true);
            currentDay = day1;
            player.transform.position = GameObject.Find("Day1PlayerStart").transform.position;
            player.transform.rotation = GameObject.Find("Day1PlayerStart").transform.rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            day1.SetActive(false);
            day2.SetActive(true);
            currentDay = day2;
            player.transform.position = GameObject.Find("Day2PlayerStart").transform.position;
            player.transform.rotation = GameObject.Find("Day2PlayerStart").transform.rotation;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Server;
using UnityEngine;

public class ClientMainMenu : MonoBehaviour
{
    private Transform[] team1;

    private Transform[] team2;

    private Lobby _lobby;
    // Start is called before the first frame update
    void Start()
    {
        team1 = new Transform[5];
        team2 = new Transform[5];
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            team1[i] = transform.GetChild(0).GetChild(i);
        }
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            team2[i] = transform.GetChild(0).GetChild(i);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}

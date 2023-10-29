using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public GameObject player;

    private int _CURRENT_TURN;
    private int _MAX_PLAYERS = 8;
    public int playerCount;
    void Awake()
    {
        if (playerCount > _MAX_PLAYERS) playerCount = _MAX_PLAYERS;
        SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlayers()
    {
        Vector2 direction = new Vector2(0, -4);
        Vector2 axis = new Vector2(-1, 0);
        float angle = 360 / playerCount;

        for (int i = 0; i < playerCount; i++)
        {
            GameObject p = Instantiate(player);
            p.name = "Player " + (i + 1);
            Vector3 pos = Quaternion.AngleAxis(angle * i, axis) * direction;
            pos.x = pos.z;      //x i z se mijesaju a ja neznam zasto
            pos.z = 0;
            //Debug.Log(pos);
            p.transform.position = pos;
            p.GetComponent<PlayerScript>()._NUMBER = i;
        }
    }
}

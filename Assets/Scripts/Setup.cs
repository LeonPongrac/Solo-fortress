/*
 * ovu skriptu koristimo za setup, pra�enje poteza, kad se kome pali HUD i slicno
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private Camera currentCamera;
    private bool hotseat;

    public GameObject player;
    public int turn_player = 0;
    private int _MAX_PLAYERS = 8;
    public int playerCount;
    public int target = -1;
    public GameObject[] players;
    void Awake()
    {
        hotseat = true;

        if (playerCount > _MAX_PLAYERS) playerCount = _MAX_PLAYERS;
        SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, Mathf.Infinity, LayerMask.GetMask("Player")))
        {
            if (Input.GetMouseButtonDown(0))
            {
                target = info.transform.gameObject.GetComponent<PlayerScript>()._NUMBER;
                if (target == turn_player) target = -1;
                else Debug.Log("TARGET: " + target);
            }
        }

        //OVO DOLE MAKNUT KAD SE NAPRAVI HUD, TRENUTNO NEMA CHECK JEL LEGALNO ISPUCAT ABILITY
        if (Input.GetKeyDown("1"))
        {
            players[turn_player].GetComponent<PlayerScript>().Ability_Basic();
            players[turn_player].GetComponent<PlayerScript>().turn_end();
            next_turn();
        }
        if (Input.GetKeyDown("2"))
        {
            players[turn_player].GetComponent<PlayerScript>().Ability_1();
            players[turn_player].GetComponent<PlayerScript>().turn_end();
            next_turn();
        }
        if (Input.GetKeyDown("3"))
        {
            players[turn_player].GetComponent<PlayerScript>().Ability_2();
            players[turn_player].GetComponent<PlayerScript>().turn_end();
            next_turn();
        }

    }
    void SpawnPlayers()
    {
        Vector2 direction = new Vector2(0, -4);
        Vector2 axis = new Vector2(-1, 0);
        float angle = 360 / playerCount;

        for (int i = 0; i < playerCount; i++)
        {
            GameObject p = Instantiate(player);
            p.name = "Player " + (i);
            Vector3 pos = Quaternion.AngleAxis(angle * i, axis) * direction;
            pos.x = pos.z;      //x i z se mijesaju a ja neznam zasto
            pos.z = 0;
            //Debug.Log(pos);
            p.transform.position = pos;
            p.GetComponent<PlayerScript>()._NUMBER = i;

            players[i] = p;
        }
    }

    public void next_turn()
    {
        players[turn_player].GetComponent<PlayerScript>().turn_end();
        if (++turn_player >= _MAX_PLAYERS) turn_player = 0;
        if (hotseat) target = -1;
        Debug.Log("TURN PLAYER: " + turn_player);
    }
}

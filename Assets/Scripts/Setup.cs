/*
 * ovu skriptu koristimo za setup, praæenje poteza, kad se kome pali HUD i slicno
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Setup : MonoBehaviour
{
    private Camera currentCamera;
    public bool hotseat;

    public GameObject player;
    public int turn_player = 0;
    private int _MAX_PLAYERS = 8;
    public int playerCount;
    public int target = -1;
    public GameObject[] players;

    private TextMeshProUGUI secondaryInfoText;
    public PlayerColors color;



    void Awake()
    {

        if (playerCount > _MAX_PLAYERS) playerCount = _MAX_PLAYERS;
        SpawnPlayers();

        secondaryInfoText = GameObject.Find("SecondaryInfo").GetComponent<TextMeshProUGUI>();

        updateSecondaryInfoColor();
      

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
        // rubovi player spritea ne aktiviraju if
        if (Physics.Raycast(ray, out info, Mathf.Infinity, LayerMask.GetMask("Player")))
        {
            if (Input.GetMouseButtonDown(0))
            {
                target = info.transform.gameObject.GetComponent<PlayerScript>()._NUMBER;
                if (target == turn_player || info.transform.gameObject.GetComponent<PlayerScript>().hp == 0) target = -1;
                else 
                {
                    SetNormal();
                    info.transform.gameObject.GetComponent<PlayerScript>().SetTargetSprite();
                    Debug.Log("TARGET: " + target); 
                }
            }
        }

        //OVO DOLE MAKNUT KAD SE NAPRAVI HUD, TRENUTNO NEMA CHECK JEL LEGALNO ISPUCAT ABILITY
        if (Input.GetKeyDown("1"))
        {
            BasicAbility();
        }
        if (Input.GetKeyDown("2"))
        {
            Ability1();
        }
        if (Input.GetKeyDown("3"))
        {
            Ability2();
        }

        // DISABLE ATTACK BUTTON IF NO TARGET
        if ( target == -1) {
            
            GameObject.Find("Attack").GetComponent<UnityEngine.UI.Button>().interactable = false;
        } else
        {
            GameObject.Find("Attack").GetComponent<UnityEngine.UI.Button>().interactable = true;
        }

    }   

    public void BasicAbility()
    {
        players[turn_player].GetComponent<PlayerScript>().Ability_Basic();
        players[turn_player].GetComponent<PlayerScript>().turn_end();

        next_turn();
    }

    public void Ability1()
    {
        players[turn_player].GetComponent<PlayerScript>().Ability_1();
        players[turn_player].GetComponent<PlayerScript>().turn_end();
        next_turn();
    }

    public void Ability2()
    {
        players[turn_player].GetComponent<PlayerScript>().Ability_2();
        players[turn_player].GetComponent<PlayerScript>().turn_end();
        next_turn();
    }


    public void SpawnPlayers()
    {
        if (players[0] != null) 
            for (int i = 0; i < players.Length; i++) {
                players[i] = null;        //reset
                GameObject t = GameObject.Find("Player " + (i));
                Destroy(t);
            }

        Vector2 direction = new Vector2(0, -3.5f);
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
            p.GetComponent<PlayerScript>().color = (PlayerColors)i;
            p.GetComponent<PlayerScript>().SetNormalSprite();

            players[i] = p; 
        }

        players[turn_player].GetComponent<PlayerScript>().SetTurnSprite();

    }

    public void next_turn()
    {
        players[turn_player].GetComponent<PlayerScript>().turn_end();
        if (++turn_player >= _MAX_PLAYERS) turn_player = 0;
        while (players[turn_player].GetComponent<PlayerScript>().hp == 0)           //TODO: winner declaration
        {
            if (++turn_player >= _MAX_PLAYERS) turn_player = 0;
        }
        if (hotseat) target = -1;
        SetNormal();
        players[turn_player].GetComponent<PlayerScript>().SetTurnSprite();
        Debug.Log("TURN PLAYER: " + turn_player);

        updateSecondaryInfoColor    ();
    }

    private void updateSecondaryInfoColor()
    {
        secondaryInfoText.text = ((PlayerColors)turn_player).ToString() + " rook";
        secondaryInfoText.color = PlayerColorUtils.GetColor((PlayerColors)turn_player);
    }

    //Postavlja sve srpiteove osim turn playera na no   rmal tj. funkcija se etukoristi za resetiranje spriteova
    public void SetNormal()
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (i != turn_player)
            {
                players[i].GetComponent<PlayerScript>().SetNormalSprite();
            }
        }
    }
}

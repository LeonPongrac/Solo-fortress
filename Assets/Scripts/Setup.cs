/*
 * ovu skriptu koristimo za setup, pra�enje poteza, kad se kome pali HUD i slicno
 */

using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using TMPro;

public class Setup : MonoBehaviour
{
    private Camera currentCamera;
    public GameObject MainMenu;
    public GameObject Background;
    public GameObject connectButton;
    public GameObject IPinput;

    public bool hotseat;
    public GameObject player;
    public int turn_player = 0;
    private int _MAX_PLAYERS = 8;
    public int playerCount;
    public int deadCount = 0;
    private int connectedPlayer = -1;
    public int target = -1;
    public GameObject[] players;

    private TextMeshProUGUI secondaryInfoText;
    private TextMeshProUGUI primaryInfoText;
    public PlayerColors color;

    private int _MYINDEX = -1;

    [Header("Victory Particle System")]
    public ParticleSystem victoryParticle;
    public GameObject _ENDSCREEN;
    public GameObject _YOUWIN;
    public GameObject _YOULOSE;

    private void Awake()
    {
        GameSetup();
        RegisterEvents();
    }
    void GameSetup()
    {

        if (playerCount > _MAX_PLAYERS) playerCount = _MAX_PLAYERS;
        SpawnPlayers();

        deadCount = 0;

        secondaryInfoText = GameObject.Find("SecondaryInfo").GetComponent<TextMeshProUGUI>();
        primaryInfoText = GameObject.Find("PrimaryInfo").GetComponent<TextMeshProUGUI>();

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
                    //Debug.Log("TARGET: " + target); 
                }
            }
        }

        // DISABLE ATTACK BUTTON IF NO TARGET or if online and not on turn
        if (target == -1 || (turn_player != _MYINDEX) && !hotseat)
        {

            GameObject.Find("Attack").GetComponent<UnityEngine.UI.Button>().interactable = false;
            GameObject.Find("Heal").GetComponent<UnityEngine.UI.Button>().interactable = false;
            GameObject.Find("Sabotage").GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else
        {
            GameObject.Find("Attack").GetComponent<UnityEngine.UI.Button>().interactable = true;

            //disable abilities if sabotaged
            if (players[turn_player].GetComponent<PlayerScript>().ability_block == 0)
            {
                GameObject.Find("Heal").GetComponent<UnityEngine.UI.Button>().interactable = true;
                GameObject.Find("Sabotage").GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }

        /*if (!hotseat && _MYINDEX != -1){
            if (turn_player == _MYINDEX){
                players[_MYINDEX].GetComponent<PlayerScript>().setMyPlayerHaloVisiblity(false);
            } else {
                players[_MYINDEX].GetComponent<PlayerScript>().setMyPlayerHaloVisiblity(true);
            }
        }*/
    }


    public void useAbility(int a)
    {
        if (hotseat)
        {
            if (a == 1) players[turn_player].GetComponent<PlayerScript>().Ability_1();
            else if (a == 2) players[turn_player].GetComponent<PlayerScript>().Ability_2();
            else players[turn_player].GetComponent<PlayerScript>().Ability_Basic();
            next_turn();
        }
        else
        {
            NetMakeMove mm = new NetMakeMove();
            mm.player = _MYINDEX;
            mm.target = target;
            mm.ability = a;
            Client.Instance.SendToServer(mm);
        }
    }

    public void endScreen() {
        Debug.Log("endscreen");
        _ENDSCREEN.SetActive(true);
        if(hotseat) {
            victoryParticle.Play();
            _YOUWIN.SetActive(true);
        }
        else
        {
            if(players[_MYINDEX].GetComponent<PlayerScript>().hp > 0) {
                victoryParticle.Play();
                _YOUWIN.SetActive(true);
            }
            else
            {
                _YOULOSE.SetActive(true);
            }
        }
    }

    public void SpawnPlayers()
    {
        if (players[0] != null) {
            deadCount = 0;
            for (int i = 0; i < players.Length; i++) {
                players[i] = null;        //reset
                GameObject t = GameObject.Find("Player " + (i));
                Destroy(t);
            }
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
            /*if (!hotseat && _MYINDEX != -1 && i == _MYINDEX)
            {
                p.GetComponent<PlayerScript>().setMyPlayerHaloVisiblity(true);
            } 
            else
            {
                p.GetComponent<PlayerScript>().setMyPlayerHaloVisiblity(false);
            }*/
            players[i] = p; 
        }

        players[turn_player].GetComponent<PlayerScript>().SetTurnSprite();

    }

    public void next_turn()
    {
        players[turn_player].GetComponent<PlayerScript>().turn_end();
        if (++turn_player >= playerCount) turn_player = 0;
        while (players[turn_player].GetComponent<PlayerScript>().hp == 0)           //TODO: winner declaration
        {
            if (++turn_player >= playerCount) turn_player = 0;
        }
        target = -1;
        SetNormal();
        players[turn_player].GetComponent<PlayerScript>().SetTurnSprite();
        Debug.Log("TURN PLAYER: " + turn_player);

        updateSecondaryInfoColor    ();
        updatePrimaryInfoText();
    }

    private void updateSecondaryInfoColor()
    {
        secondaryInfoText.text = ((PlayerColors)turn_player).ToString() + " rook";
        secondaryInfoText.color = PlayerColorUtils.GetColor((PlayerColors)turn_player);
    }

    private void updatePrimaryInfoText()
    {
        if (!hotseat && _MYINDEX == turn_player)
        {
            primaryInfoText.text = "YOUR TURN!";
        }
        else
        {
            primaryInfoText.text = "CURRENT TURN:";
        }
       
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

    //net stuff
    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.C_WELCOME += OnWelcomeClient;

        NetUtility.C_START_GAME += OnStartGameClient;

        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
    }

    private void UnRegisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.C_WELCOME -= OnWelcomeClient;

        NetUtility.C_START_GAME -= OnStartGameClient;

        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
    }

    private void OnWelcomeServer(NetMessage message, NetworkConnection connection)
    {
        //Client connected, assign color and bounce back
        NetWelcome nw = message as NetWelcome;

        nw.Index = ++connectedPlayer;
        nw.playerCount = playerCount;
        Debug.Log("new connection: " + connectedPlayer);

        Server.Instance.SendToClient(connection, nw);

        if (connectedPlayer == playerCount - 1)
        {
            NetStartGame sg = new NetStartGame();

            Server.Instance.Broadcast(sg);
        }
    }
    private void OnWelcomeClient(NetMessage message)
    {
        NetWelcome nw = message as NetWelcome;
        _MYINDEX = nw.Index;
        playerCount = nw.playerCount;
        Debug.Log("My turn: " + _MYINDEX + "Number of players: " + playerCount);
        connectButton.SetActive(false);
        IPinput.SetActive(false);
        GameSetup();

    }
    private void OnStartGameClient(NetMessage message)
    {
        NetStartGame msg = message as NetStartGame;
        hotseat = false;

        MainMenu.SetActive(false);          //activate on rematch / quit
        Background.SetActive(false);

        updatePrimaryInfoText();
    }
    private void OnMakeMoveServer(NetMessage message, NetworkConnection connection)
    {
        NetMakeMove mm = message as NetMakeMove;
        //check hax here

        //recv and broadcast
        Server.Instance.Broadcast(mm);
    }
    private void OnMakeMoveClient(NetMessage message)
    {
        NetMakeMove mm = message as NetMakeMove;
        target = mm.target;
        players[mm.player].GetComponent<PlayerScript>().target = mm.target;
        if(mm.ability == 1) players[mm.player].GetComponent<PlayerScript>().Ability_1();
        else if(mm.ability == 2) players[mm.player].GetComponent<PlayerScript>().Ability_2();
        else players[mm.player].GetComponent<PlayerScript>().Ability_Basic();

        next_turn();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OnlineGUI : MonoBehaviour
{
    public static OnlineGUI Instance { set; get; }
    public Server server;
    public Client client;
    public Setup setup;
    [SerializeField] private TMP_InputField addressInput;

    private void Awake()
    {
        Instance = this;
    }

    public void LocalButton()
    {
        Debug.Log("local");
        setup.hotseat = true;
    }

    public void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    public void HostButton()
    {
        server.Init(25565);
        client.Init("127.0.0.1", 25565);
        Debug.Log("hosting on port " + 25565);
        setup.hotseat = false;
    }

    public void ConnectButton()
    {
        client.Init(addressInput.text, 25565);
        Debug.Log("connect button");
        setup.hotseat = false;
    }
    public void HostBackButton()
    {
        server.Shutdown();
        client.Shutdown();
    }

    //HUD

    public void SurrenderButton()
    {
        
    }
}

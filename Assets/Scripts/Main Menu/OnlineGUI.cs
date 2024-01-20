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
    [SerializeField] private TMP_InputField playerNumberInput;
    [SerializeField] private GameObject hostMenu;
    [SerializeField] private GameObject hostWaitMenu;

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
        if (int.TryParse(playerNumberInput.text, out int playerNumber))
        {
            if (playerNumber >= 2 && playerNumber <= 8)
            {
                Debug.Log("Number is between 2 and 8.");
                server.Init(288, playerNumber);
                client.Init("127.0.0.1", 288);
                Debug.Log("hosting on port " + 288);
                setup.hotseat = false;
                hostMenu.SetActive(false);
                hostWaitMenu.SetActive(true);
            }
            else
            {
                Debug.Log("Number is not between 2 and 8.");
            }
        }
        else
        {
            Debug.Log("Invalid input. Please enter a valid number.");
        }
       
    }

    public void ConnectButton()
    {
        client.Init(addressInput.text, 288);
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

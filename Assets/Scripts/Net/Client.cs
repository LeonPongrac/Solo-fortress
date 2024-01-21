using System;
using UnityEngine;
using Unity.Networking.Transport;
using System.IO;

public class Client : MonoBehaviour
{
    public static Client Instance { set; get;}

    private void Awake() {
        Instance = this;
    }

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;

    public Action connectionDropped;

    public void Init(string ip, ushort port){
        driver = NetworkDriver.Create();
        NetworkEndpoint endPoint = NetworkEndpoint.Parse(ip, port);

        connection = driver.Connect(endPoint);
        Debug.Log("Attempting to connect to " + endPoint.Address);

        isActive = true;

        RegisterToEvent();
    }

    public void Shutdown(){
        if(isActive){
            UnregisterToEvent();
            driver.Dispose();
            connection = default(NetworkConnection);
            isActive = false;
        }
    }

    public void OnDestroy()
    {
        Shutdown();
    }

    void Update()
    {
        if(!isActive) return;

        driver.ScheduleUpdate().Complete();
        CheckAlive();

        UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if(!connection.IsCreated && isActive){
            Debug.Log("Connection lost");
            connectionDropped?.Invoke();
            Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        Unity.Collections.DataStreamReader streamReader;
        NetworkEvent.Type cmd;
        while((cmd = connection.PopEvent(driver, out streamReader)) != NetworkEvent.Type.Empty){
            if (cmd == NetworkEvent.Type.Data){

                NetUtility.OnData(streamReader, default(NetworkConnection));
            } else if (cmd == NetworkEvent.Type.Disconnect){

                Debug.Log("Client disconnected");
                connection = default(NetworkConnection);
                connectionDropped?.Invoke();
                Shutdown();
            } else if (cmd == NetworkEvent.Type.Connect){

                NetWelcome nw = new NetWelcome();
                SendToServer(nw);
                Debug.Log("connected to server");
            }
        }
    }

    public void SendToServer(NetMessage msg)
    {
        Unity.Collections.DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
        Debug.Log("msg sent to server");
    }

    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
        Debug.Log("RegisterToEvent()");
    }
    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }
    private void OnKeepAlive(NetMessage netMessage){
        SendToServer(netMessage);
    }
}

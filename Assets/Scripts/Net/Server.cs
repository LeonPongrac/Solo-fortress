using System;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class Server : MonoBehaviour
{
    public static Server Instance { set; get;}

    private void Awake() {
        Instance = this;
    }

    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private const float keepAliveTickRate = 20.0f;
    private float lastKeepAlive;

    public Action connectionDropped;

    public void Init(ushort port){
        driver = NetworkDriver.Create();
        NetworkEndpoint endPoint = NetworkEndpoint.AnyIpv4;
        endPoint.Port = port;

        if(driver.Bind(endPoint) != 0){
            Debug.Log("Failed to bind on port " + endPoint.Port);
            return;
        } else {
            driver.Listen();
            Debug.Log("Listening on port " + endPoint.Port);
        }

        connections = new NativeList<NetworkConnection>(GameObject.Find("Main Camera").GetComponent<Setup>().playerCount, Allocator.Persistent);
        isActive = true;
    }
    public void Shutdown(){
        if(isActive){
            driver.Dispose();
            connections.Dispose();
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

        KeepAlive();

        driver.ScheduleUpdate().Complete();

        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }

    private void KeepAlive()
    {
        if (Time.time - lastKeepAlive > keepAliveTickRate){
            lastKeepAlive = Time.time;
            Broadcast(new NetKeepAlive());
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader streamReader;
        for(int i = 0; i < connections.Length; i++){
            NetworkEvent.Type cmd;
            while((cmd = driver.PopEventForConnection(connections[i], out streamReader)) != NetworkEvent.Type.Empty){
                if (cmd == NetworkEvent.Type.Data){

                    NetUtility.OnData(streamReader, connections[i], this);
                } else if (cmd == NetworkEvent.Type.Disconnect){

                    Debug.Log("Client disconnected");
                    connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    Shutdown();
                }
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while((c = driver.Accept()) != default(NetworkConnection)){
            connections.Add(c);
            Debug.Log("new connection accepted");
        }
    }

    private void CleanupConnections()
    {
        for(int i = 0; i < connections.Length; i++){
            if(!connections[i].IsCreated){
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    //Server specific
    public void SendToClient(NetworkConnection networkConnection, NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(networkConnection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
        Debug.Log($"{msg.Code} sent to client");
    }
    public void Broadcast(NetMessage msg){
        for(int i = 0; i < connections.Length; i++){
            if(connections[i].IsCreated){
                Debug.Log($"Broadcasting {msg.Code} to all connections ({i + 1} / {connections.Length})");
                SendToClient(connections[i], msg);
            }
        }
    }

}

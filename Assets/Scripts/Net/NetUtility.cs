using System;
using UnityEngine;
using Unity.Networking.Transport;

public enum OpCode{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    MAKE_MOVE = 4
}

public static class NetUtility
{
    public static void OnData(Unity.Collections.DataStreamReader reader, NetworkConnection connection, Server server = null){
        NetMessage msg = null;
        var opCode = (OpCode)reader.ReadByte();
        switch(opCode){
            case OpCode.KEEP_ALIVE: msg = new NetKeepAlive(reader); break;
            case OpCode.WELCOME: msg = new NetWelcome(reader); break;
            case OpCode.START_GAME: msg = new NetStartGame(reader); break;
            case OpCode.MAKE_MOVE: msg = new NetMakeMove(reader); break;
            default: Debug.Log("Message recieved had no OpCode"); break;
        }
        if(server != null) msg.RecievedOnServer(connection);
        else msg.RecievedOnClient();
    }

    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
}
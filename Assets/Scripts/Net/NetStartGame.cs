using Unity.Networking.Transport;

public class NetStartGame : NetMessage
{
    public int whiteclass, blackclass;
    public NetStartGame(){
        Code = OpCode.START_GAME;
    }
    public NetStartGame(Unity.Collections.DataStreamReader reader){   //recieve
        Code = OpCode.START_GAME;
        Deserialize(reader);
    }
    public override void Serialize(ref Unity.Collections.DataStreamWriter writer){
        writer.WriteByte((byte)Code);
    }
    public override void Deserialize(Unity.Collections.DataStreamReader reader){
    }
    public override void RecievedOnClient(){
        NetUtility.C_START_GAME?.Invoke(this);
    }
    public override void RecievedOnServer(NetworkConnection connection){
        NetUtility.S_START_GAME?.Invoke(this, connection);
    }
}

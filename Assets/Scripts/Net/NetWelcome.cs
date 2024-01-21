using Unity.Networking.Transport;

public class NetWelcome : NetMessage
{
    public int Index {set; get;}
    public int playerCount { set; get; }
    public NetWelcome(){              //make
        Code = OpCode.WELCOME;
    }
    public NetWelcome(Unity.Collections.DataStreamReader reader){   //recieve
        Code = OpCode.WELCOME;
        Deserialize(reader);
    }
    public override void Serialize(ref Unity.Collections.DataStreamWriter writer){
        writer.WriteByte((byte)Code);
        writer.WriteInt(Index);
        writer.WriteInt(playerCount);
    }
    public override void Deserialize(Unity.Collections.DataStreamReader reader){
        Index = reader.ReadInt();
        playerCount = reader.ReadInt();
    }
    public override void RecievedOnClient(){
        NetUtility.C_WELCOME?.Invoke(this);
    }
    public override void RecievedOnServer(NetworkConnection connection){
        NetUtility.S_WELCOME?.Invoke(this, connection);
    }
}

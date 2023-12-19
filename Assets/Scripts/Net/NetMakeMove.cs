using Unity.Networking.Transport;

public class NetMakeMove : NetMessage
{

    public int player;
    public int target;
    public int ability;

    public NetMakeMove(){              //make
        Code = OpCode.MAKE_MOVE;
    }
    public NetMakeMove(Unity.Collections.DataStreamReader reader){   //recieve
        Code = OpCode.MAKE_MOVE;
        Deserialize(reader);
    }

    public override void Serialize(ref Unity.Collections.DataStreamWriter writer){
        writer.WriteByte((byte)Code);
        writer.WriteInt(player);
        writer.WriteInt(target);
        writer.WriteInt(ability);
    }
    public override void Deserialize(Unity.Collections.DataStreamReader reader){
        player = reader.ReadInt();
        target = reader.ReadInt();
        ability = reader.ReadInt();
    }
    public override void RecievedOnClient(){
        NetUtility.C_MAKE_MOVE?.Invoke(this);
    }
    public override void RecievedOnServer(NetworkConnection connection){
        NetUtility.S_MAKE_MOVE?.Invoke(this, connection);
    }
}
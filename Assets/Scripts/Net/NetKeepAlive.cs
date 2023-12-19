using Unity.Networking.Transport;

public class NetKeepAlive : NetMessage
{
    public NetKeepAlive(){              //make
        Code = OpCode.KEEP_ALIVE;
    }
    public NetKeepAlive(Unity.Collections.DataStreamReader reader){   //recieve
        Code = OpCode.KEEP_ALIVE;
        Deserialize(reader);
    }

    public override void Serialize(ref Unity.Collections.DataStreamWriter writer){
        writer.WriteByte((byte)Code);
    }
    public override void Deserialize(Unity.Collections.DataStreamReader reader){
        //needless
    }
    public override void RecievedOnClient(){
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }
    public override void RecievedOnServer(NetworkConnection connection){
        NetUtility.S_KEEP_ALIVE?.Invoke(this, connection);
    }
}
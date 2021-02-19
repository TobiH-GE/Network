using System;
using System.Text;

namespace NetworkMessage
{
    public enum MsgType : byte
    {
        Command,
        Data,
        Text
    }
    public enum SubType : byte //TODO: more subtypes
    {
        Login,
        LoginRequest,
        Logout,
        Direct,
        Room,
        Broadcast,
        Info,
        JoinRoom,
        JoinOk,
        LeaveRoom,
        LeaveOk,
        Userlist
    }
    public abstract class Message
    {
        public MsgType Mtype;
        public SubType Stype;
        public string Parameter;
        public string Username;
        public int MessageSize;
        public Message(MsgType MessageType, SubType Stype, string Parameter, string Username)
        {
            this.Mtype = MessageType;
            this.Stype = Stype;
            this.Parameter = Parameter;
            this.Username = Username;
        }
        public Message() {}

        public abstract byte[] getBytes();
    }
    public class MessageCommand : Message
    {
        public string Command;
        public MessageCommand(MsgType MessageType, SubType Stype, string Parameter = "", string Username = "", string Command = "") : base(MessageType, Stype, Parameter, Username)
        {
            this.Command = Command;
        }
        public MessageCommand(byte[] Data)
        {
            Mtype = (MsgType)Data[0];
            Stype = (SubType)Data[1];
            byte ParamterLenght = Data[2];
            byte UsernameLenght = Data[3];
            MessageSize = BitConverter.ToInt32(Data, 4) + 8;

            int offset = 8;
            string message = Encoding.ASCII.GetString(Data, offset, Data.Length - offset);
            Parameter = message.Substring(0, ParamterLenght);
            Username = message.Substring(ParamterLenght, UsernameLenght);
            Command = message.Substring(ParamterLenght + UsernameLenght);
        }
        public override byte[] getBytes() //TODO: create more message types
        {
            string datastring = $"        {Parameter}{Username}{Command}";
            byte[] data = Encoding.ASCII.GetBytes(datastring);
            data[0] = (byte)Mtype;
            data[1] = (byte)Stype;
            data[2] = (byte)Parameter.Length;
            data[3] = (byte)Username.Length;
            byte[] databytes = BitConverter.GetBytes(Command.Length * 2); // TODO: better converter
            data[4] = databytes[0];
            data[5] = databytes[1];
            data[6] = databytes[2];
            data[7] = databytes[3];
            return data;
        }
    }
    public class MessageData : Message
    {
        public byte[] Data;
        public MessageData(MsgType MessageType, SubType Stype, string Parameter, string Username, byte[] Data) : base (MessageType, Stype, Parameter, Username)
        {
            this.Data = Data;
        }
        public MessageData(byte[] Data)
        {
            Mtype = (MsgType)Data[0];
            Stype = (SubType)Data[1];
            byte ParamterLenght = Data[2];
            byte UsernameLenght = Data[3];
            MessageSize = BitConverter.ToInt32(Data, 4) + 8;

            int offset = 8;
            string message = Encoding.ASCII.GetString(Data, offset, ParamterLenght + UsernameLenght);
            Parameter = message.Substring(0, ParamterLenght);
            Username = message.Substring(ParamterLenght, UsernameLenght);
            int fileoffset = offset + ParamterLenght + UsernameLenght;
            this.Data = Data[fileoffset..];
        }
        public override byte[] getBytes() //TODO: correct {Data}
        {
            string datastring = $"        {Parameter}{Username}{Data}";
            byte[] data = Encoding.ASCII.GetBytes(datastring);
            data[0] = (byte)Mtype;
            data[1] = (byte)Stype;
            data[2] = (byte)Parameter.Length;
            data[3] = (byte)Username.Length;
            byte[] databytes = BitConverter.GetBytes(Data.Length); // TODO: better converter
            data[4] = databytes[0];
            data[5] = databytes[1];
            data[6] = databytes[2];
            data[7] = databytes[3];
            return data;
        }
    }
    public class MessageText : Message
    {
        public string Text;
        public MessageText(MsgType MessageType, SubType Stype, string Parameter, string Username, string Text) : base(MessageType, Stype, Parameter, Username)
        {
            this.Text = Text;
        }
        public MessageText(byte[] Data)
        {
            Mtype = (MsgType)Data[0];
            Stype = (SubType)Data[1];
            byte ParamterLenght = Data[2];
            byte UsernameLenght = Data[3];
            MessageSize = BitConverter.ToInt32(Data, 4) + 8;

            int offset = 8;
            string message = Encoding.ASCII.GetString(Data, offset, Data.Length - offset);
            Parameter = message.Substring(0, ParamterLenght);
            Username = message.Substring(ParamterLenght, UsernameLenght);
            Text = message.Substring(ParamterLenght + UsernameLenght);
        }
        public override byte[] getBytes() //TODO: create more message types
        {
            string datastring = $"        {Parameter}{Username}{Text}";
            byte[] data = Encoding.ASCII.GetBytes(datastring);
            data[0] = (byte)Mtype;
            data[1] = (byte)Stype;
            data[2] = (byte)Parameter.Length;
            data[3] = (byte)Username.Length;
            byte[] databytes = BitConverter.GetBytes(Text.Length * 2); // TODO: better converter
            data[4] = databytes[0];
            data[5] = databytes[1];
            data[6] = databytes[2];
            data[7] = databytes[3];
            return data;
        }
    }
}

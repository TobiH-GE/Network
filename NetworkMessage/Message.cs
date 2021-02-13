using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace NetworkMessage
{
    public enum MessageType : byte
    {
        Login,
        Logout,
        LoginSuccessful,
        LoginFail,
        Group
    }
    public enum DataType : byte
    {
        Text,
        File
    }
    public class Message
    {
        public MessageType MessageType;
        public DataType DataType;
        public string Parameter;
        public string Username;
        public string Text;
        public byte[] Data;

        public Message(MessageType MessageType, DataType DataType, string MessageParameter, string Text)
        {
            this.MessageType = MessageType;
            this.DataType = DataType;
            this.Parameter = MessageParameter;
            this.Text = Text;
        }
        public Message(byte[] data)
        {
            MessageType MessageType = (MessageType)data[0];
            DataType DataType = (DataType)data[1];
            byte ParamterLenght = data[2];
            byte UsernameLenght = data[3];

            if (DataType == DataType.Text)
            {
                int offset = 4;
                string message = Encoding.ASCII.GetString(data, offset, data.Length - offset);
                Parameter = message.Substring(0, ParamterLenght);
                Username = message.Substring(ParamterLenght, UsernameLenght);
                Text = message.Substring(ParamterLenght + UsernameLenght);
            }
            else if (DataType == DataType.File) //TODO: file handling
            {
                int offset = 4;
                string message = Encoding.ASCII.GetString(data, offset, ParamterLenght + UsernameLenght);
                Parameter = message.Substring(0, ParamterLenght);
                Username = message.Substring(ParamterLenght, UsernameLenght);
                int fileoffset = offset + ParamterLenght + UsernameLenght;
                Data = data[fileoffset..];
            }
        }
        public byte[] getBytes() //TODO: create more message types
        {
            string datastring = $"    {Parameter}{Username}{Text}";
            byte[] data = Encoding.ASCII.GetBytes(datastring);
            data[0] = (byte)MessageType.Group;
            data[1] = (byte)DataType.Text;
            data[2] = (byte)Parameter.Length;
            data[3] = (byte)Username.Length;
            return data;
        }
    }
}

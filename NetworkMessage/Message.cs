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
        public string MessageParameter;
        public string Data;

        public Message(MessageType MessageType, DataType DataType, string MessageParameter, string Data)
        {
            this.MessageType = MessageType;
            this.DataType = DataType;
            this.MessageParameter = MessageParameter;
            this.Data = Data;
        }
        public byte[] getBytes() //TODO: correct
        {
            string temp = $"{(char)MessageType}{(char)DataType}{MessageParameter}" + Data;
            return Encoding.ASCII.GetBytes(temp);
        }
    }
}

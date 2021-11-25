using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    public enum PacketType { ChatMessage, PrivateMessage, ClientName };

    [Serializable]
    public class Packet
    {
        protected PacketType m_PacketType;

        public PacketType GetPacketType()
        {
            return m_PacketType;
        }

        public void SetPacketType(PacketType packetType)
        {
            m_PacketType = packetType;
        }
    }

    [Serializable]
    public class ChatMessagePacket : Packet
    {
        public string m_message;

        public ChatMessagePacket(string message)
        {
            m_message = message;
            m_PacketType = PacketType.ChatMessage;
        }
    }
}

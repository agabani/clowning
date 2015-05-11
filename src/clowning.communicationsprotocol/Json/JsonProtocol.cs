using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace clowning.communicationsprotocol.Json
{
    public class JsonProtocol
    {
        public JsonProtocol()
        {
        }

        public int GetPacketType(byte[] packet)
        {
            return packet[0];
        }

        public int GetPacketLength(byte[] packet)
        {
            return BitConverter.ToInt32(packet, 1);
        }

        public string GetPacketContents(byte[] packet, int length)
        {
            return Encoding.UTF8.GetString(packet.Skip(5).Take(length).ToArray());
        }

        public byte[] SetPacketContents(int type, string content)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(new[] {(byte) type}, 0, 1);
                memoryStream.Write(BitConverter.GetBytes(content.Length), 0, 4);
                memoryStream.Write(Encoding.UTF8.GetBytes(content), 0, content.Length);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Marked for deletion
        /// </summary>
        public string Serialize<T>(T content)
        {
            return JsonConvert.SerializeObject(content);
        }

        /// <summary>
        /// Marked for deletion
        /// </summary>
        public T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}

/*
 * Container : Byte Array
 * 
 * [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ]
 *  a   b   b   b   b   c ############# c
 * 
 * a ) Packet Type
 * b ) Packet Length
 * c ) UTF8 encoded json string
 * 
 * Stream : memory stream for incoming files larger than max buffer
 * Event  : Fire event when complete packet is ready
 */
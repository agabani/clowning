using System.Collections.Generic;
using Newtonsoft.Json;

namespace clowning.communicationsprotocol.Json.Packets
{
    [JsonObject]
    public class JsonIdentityPacket
    {
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "devices", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Device> Devices { get; set; }
    }

    [JsonObject]
    public class Device
    {
        [JsonProperty(PropertyName = "deviceId")]
        public int DeviceId { get; set; }

        [JsonProperty(PropertyName = "deviceType")]
        public string DeviceType { get; set; }
    }
}

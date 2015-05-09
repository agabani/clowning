using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace clowning.communicationsprotocol.jsonprotocol
{
    [JsonObject]
    public class BlyncJsonStatus
    {
        public string Name { get; set; }

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

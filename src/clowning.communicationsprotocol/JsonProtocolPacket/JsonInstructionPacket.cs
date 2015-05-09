using System.Collections.Generic;
using Newtonsoft.Json;

namespace clowning.communicationsprotocol.JsonProtocolPacket
{
    [JsonObject]
    public class JsonInstructionPacket
    {
        [JsonProperty(PropertyName = "deviceId", NullValueHandling = NullValueHandling.Ignore)]
        public int? DeviceId { get; set; }

        [JsonProperty(PropertyName = "deviceIds", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<int> DeviceIds { get; set; }

        [JsonProperty(PropertyName = "flash", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Flash { get; set; }

        [JsonProperty(PropertyName = "flashSpeed", NullValueHandling = NullValueHandling.Ignore)]
        public int? FlashSpeed { get; set; }

        [JsonProperty(PropertyName = "dim", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Dim { get; set; }

        [JsonProperty(PropertyName = "rgb", NullValueHandling = NullValueHandling.Ignore)]
        public BlyncRgb Rgb { get; set; }

        [JsonProperty(PropertyName = "color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "reset", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Reset { get; set; }
    }

    [JsonObject]
    public class BlyncRgb
    {
        [JsonProperty(PropertyName = "red")]
        public int Red { get; set; }

        [JsonProperty(PropertyName = "green")]
        public int Green { get; set; }

        [JsonProperty(PropertyName = "blue")]
        public int Blue { get; set; }
    }
}
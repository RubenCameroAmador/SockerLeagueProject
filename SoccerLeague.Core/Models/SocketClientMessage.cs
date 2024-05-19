using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoccerLeague.Core.Models
{
    public class SocketClientMessage
    {
        public enum MessageType
        {
            Undefined,
            OnMessage
        }

        public enum MainType
        {
            Undefined,
            Team,
            TeamMatch
        }

        [JsonProperty("type")]
        public MessageType Type { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("payloadtype")]
        public MainType PayloadType { get; set; }

        [JsonProperty("payload")]
        public object? Payload { get; set; }

    }
}
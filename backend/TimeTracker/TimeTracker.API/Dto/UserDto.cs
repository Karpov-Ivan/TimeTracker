using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TimeTracker.DTOModels
{
    [JsonObject]
    public class UserDto
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}


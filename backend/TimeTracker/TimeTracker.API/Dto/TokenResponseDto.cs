using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TimeTracker.Dto
{
    [JsonObject]
	public class TokenResponseDto
	{
        [JsonPropertyName("access_token")]
        public string? access_token { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? refresh_token { get; set; }
    }
}


using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TimeTracker.Dto
{
    [JsonObject]
    public class IssueDto
	{
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Title { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("time_spent")]
        public long? TimeSpent { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}


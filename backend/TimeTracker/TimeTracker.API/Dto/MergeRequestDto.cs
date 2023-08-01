using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TimeTracker.Dto
{
    [JsonObject]
	public class MergeRequestDto
	{
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("total_time_stats")]
        public long? TotalTimeStats { get; set; }
    }
}


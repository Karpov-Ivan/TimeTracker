using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TimeTracker.Dto
{
	[JsonObject]
	public class CommentDto
	{
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("id_issue_or_mr")]
        public int IdIOrMR { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("time_spent")]
        public long? TotalTimeStats { get; set; }
    }
}


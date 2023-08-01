using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using TimeTracker.Models.Models;

namespace TimeTracker.Dto
{
    [JsonObject]
    public class ProjectDto
	{
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Name { get; set; }

        [JsonPropertyName("contributors_num")]
        public int NumberOfDevelopers { get; set; }

        [JsonPropertyName("issue_num")]
        public int NumberOfIssues { get; set; }

        [JsonPropertyName("mr_num")]
        public int NumberOfMergeRequests { get; set; }

        [JsonPropertyName("time_spent")]
        public long? TimeSpentOnProject { get; set; }
    }
}


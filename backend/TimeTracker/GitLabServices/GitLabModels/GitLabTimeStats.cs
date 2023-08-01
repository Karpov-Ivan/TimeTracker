using System;
using Newtonsoft.Json;

namespace GitLabServices.GitLabModels
{
    public class GitLabTimeStats
    {
        [JsonProperty("total_time_spent")]
        public long? TotalTimeSpent { get; set; }
    }
}


using System;
using Newtonsoft.Json;

namespace GitLabServices.GitLabModels
{
	public class GitLabComment
	{
        public int Id { get; set; }

        public string? Body { get; set; }

        [JsonProperty("created_at")]
        public string? CreatedAt { get; set; }

        public GitLabUser? Author { get; set; }

        public int IdIOrMR { get; set; }

        public string? Title { get; set; }

        public string? Type { get; set; }

        public long? TotalTimeStats { get; set; }
    }
}


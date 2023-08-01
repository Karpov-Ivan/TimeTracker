using System;
using Newtonsoft.Json;

namespace GitLabServices.GitLabModels
{
	public class GitLabUser
	{
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("username")]
        public string? Username { get; set; }
    }
}


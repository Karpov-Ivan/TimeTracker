using System;

namespace GitLabServices.GitLabModels
{
	public class GitLabTokenResponse
	{
        public string? access_token { get; set; }

        public string? refresh_token { get; set; }
    }
}


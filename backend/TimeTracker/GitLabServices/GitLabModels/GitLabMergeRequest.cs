using System;
using Newtonsoft.Json;

namespace GitLabServices.GitLabModels
{
    public class GitLabMergeRequest
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        [JsonProperty("created_at")]
        public string? CreatedAt { get; set; }

        [JsonProperty("time_stats")]
        public GitLabTimeStats? TimeStats { get; set; }

        public long? TotalTimeStats
        {
            get { return TimeStats?.TotalTimeSpent; }

            set
            {
                if (value != null)
                {
                    if (TimeStats == null)
                    {
                        TimeStats = new GitLabTimeStats();
                    }

                    TimeStats.TotalTimeSpent = value;
                }
                else
                {
                    if (TimeStats != null)
                    {
                        TimeStats.TotalTimeSpent = null;
                    }
                }
            }
        }
    }
}


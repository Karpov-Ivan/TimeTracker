using System;
using TimeTracker.Models.Models;

namespace GitLabServices.GitLabModels
{
	public class GitLabProject
	{
        public int Id { get; set; }

        public string? Name { get; set; }

        public List<GitLabDeveloper>? Developers { get; set; }

        public List<GitLabIssue>? Issues { get; set; }

        public List<GitLabMergeRequest>? MergeRequests { get; set; }

        public List<GitLabComment>? Comments { get; set; }

        public int NumberOfDevelopers => Developers?.Count ?? 0;

        public int NumberOfIssues => Issues?.Count ?? 0;

        public int NumberOfMergeRequests => MergeRequests?.Count() ?? 0;

        public long? TimeSpentOnProject
        {
            get
            {
                if (Comments == null || Comments.Count == 0)
                    return null;

                long? totalTime = 0;
                if (Comments != null)
                {
                    if (Comments.Count != 0)
                    {
                        foreach (var comment in Comments)
                        {
                            if (comment.TotalTimeStats != null)
                                totalTime += comment.TotalTimeStats;
                        }
                    }
                }

                return (long)totalTime;
            }
        }
    }
}


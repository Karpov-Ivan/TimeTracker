using System;
namespace TimeTracker.Models.Models
{
	public class Issue
	{
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Author { get; set; }

        public int AuthorId { get; set; }

        public string? Date { get; set; }

        public long? TimeSpent { get; set; }

        public string? Type { get; set; }

        public int MergeRequestsCount { get; set; }
    }
}


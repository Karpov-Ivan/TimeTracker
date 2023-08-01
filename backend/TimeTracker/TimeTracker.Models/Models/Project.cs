using System;
namespace TimeTracker.Models.Models
{
	public class Project
	{
        public int Id { get; set; }

        public string? Name { get; set; }

        public int NumberOfDevelopers { get; set; }

        public int NumberOfIssues { get; set; }

        public int NumberOfMergeRequests { get; set; }

        public long? TimeSpentOnProject { get; set; }
    }
}


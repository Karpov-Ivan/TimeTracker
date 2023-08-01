using System;
namespace TimeTracker.Models.Models
{
	public class MergeRequest
	{
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? CreatedAt { get; set; }

        public long? TotalTimeStats { get; set; }
    }
}


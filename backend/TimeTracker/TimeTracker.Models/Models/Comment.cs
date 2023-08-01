using System;
namespace TimeTracker.Models.Models
{
	public class Comment
	{
        public int Id { get; set; }

        public string? Body { get; set; }

        public string? CreatedAt { get; set; }

        public string? Author { get; set; }

        public int IdIOrMR { get; set; }

        public string? Title { get; set; }

        public string? Type { get; set; }

        public long? TotalTimeStats { get; set; }
    }
}


using System;

namespace TimeTracker.Models
{
    public class User
    {
        public int Id { get; set; }

        public string? Login { get; set; }

        public string? Password { get; set; }

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Username { get; set; }
    }
}

